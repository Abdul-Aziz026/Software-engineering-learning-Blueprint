# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

> Always answer in English (mixed) language.

## Project

A full-stack, AI-powered learning platform:

- **Backend** — ASP.NET Core (.NET 10), strict Clean Architecture.
- **Frontend** — Angular SPA (`Frontend/Dashboard`).
- **Differentiator** — an in-process MCP server + client driving an agentic LLM loop, with the provider (Gemini or Claude) switchable at runtime.

## Commands

### Backend (from `Backend/`)
```bash
dotnet build BackendBluePrint.slnx     # build the solution
dotnet run --project API               # run the API (Swagger at /swagger in Development)
dotnet test                            # all xUnit tests
dotnet test --filter "FullyQualifiedName~EmailTests"                       # one class
dotnet test --filter "FullyQualifiedName~SignupCommandValidatorTests.Validate_Fails_When_Email_Invalid"   # one test
dotnet test --filter "FullyQualifiedName~Integration"                      # integration tests only
```

**Test layering:** tests live in `Backend/Tests/` (xUnit). Unit tests reference only `Domain` and `Application` — keep them off `Infrastructure`/`API`. Integration tests under `Backend/Tests/Integration/` are the exception: they reference `API` to boot the real host via `WebApplicationFactory<Program>` and require Docker (Testcontainers spins up a throwaway MongoDB).

### Frontend (from `Frontend/Dashboard/`)
```bash
npm install
npm start        # ng serve → http://localhost:4200
npm run build    # production build
npm test         # vitest unit tests
```

### Full stack
```bash
docker compose up --build    # frontend → :4200, backend → :5000
```

## Architecture

### Backend layering (dependencies flow inward only)

`API` → `Infrastructure` → `Application` → `Domain`. All six projects target `net10.0`. Rules:

- `Domain` has **zero** external/framework dependencies.
- Never make `Application` or `Domain` depend on `Infrastructure` or `API`.
- Cross-layer references are always against an interface, never a concrete class.

| Project | Role | References |
|---|---|---|
| **Domain** | Entities (`BaseEntity`, `User`, `BlogPost`, `Chapter`, `Subject`, `ChatThread`…), enums (`LlmProvider`, `NotificationType`), value objects (`Email`), exceptions, repository interfaces. | nothing |
| **Application** | CQRS feature slices, DTOs, interface definitions (`Common/Interfaces/…`), MCP tool authoring (`Tools/TutorialTools.cs`). | Domain, Contracts |
| **Contracts** | Shared MassTransit message contracts. A leaf shared kernel, *not* a layer in the inward rule. | MediatR only |
| **Infrastructure** | Concrete implementations: MongoDB (`Persistence/`, `Repositories/`), Redis cache, SignalR, LLM clients + `LlmFactory`, MCP service, MassTransit `MessageBus`, email, password hashing. | Application, Domain |
| **API** | Thin controllers, middleware, DI wiring (`Program.cs`). | Application, Infrastructure, Contracts |

API service registration is split across `Extensions/`: `ServiceCollectionExtensions.cs` (repos/cache/security/message bus), `MasstransitAndMediatRExtensions.cs` (MediatR + validators + `ValidationBehavior`), `ConfigurationSettingExtensions.cs` (options binding + BSON serializers).

### Request flow (CQRS)

Controllers are thin — no business logic. They inject `IMessageBus` (Infrastructure's `MessageBus`, wrapping MediatR) and call `SendAsync<TCommand, TResponse>(command)`. Every write is a `Command`, every read a `Query`, each with a dedicated handler.

- **Validation** runs automatically via `ValidationBehavior<,>`, a MediatR pipeline behavior. FluentValidation validators are auto-discovered from the Application assembly (`AddValidatorsFromAssembly`) — a new `*CommandValidator` is wired up just by existing. On failure it throws `ValidationException`.
- **Middleware order:** `CorrelationIdMiddleware` (outermost — stamps/propagates a correlation id) → `GlobalExceptionMiddleware` (maps domain exceptions to HTTP: `ValidationException`, `NotFoundException`, `AuthenticationException`, `LlmUnavailableException` → 502).
- **Errors** follow RFC 7807 (`ProblemDetails`/`ValidationProblemDetails`) with the correlation id as a `correlationId` extension. The Angular chat UI depends on this contract — keep new error paths on it.

**Adding a feature:** create `Application/Features/<Area>/{Commands|Queries}/<Name>/`, add the request + handler (+ validator if needed), then a controller action that dispatches via `IMessageBus`. Handlers and validators are assembly-scanned — no DI registration.

### AI / agentic subsystem

- **Provider strategy** — `ILlmFactory` returns the right `IChatClient` (`GeminiChatClient` or `ClaudeChatClient`) for the `LlmProvider` passed at request time; consuming code never changes.
- **Resilience** — provider clients are wrapped by `ResilientChatClient` (`Infrastructure/Llm/`): per-attempt timeout + bounded retry (exponential backoff/jitter) for transient failures (network, 429, 5xx). It sits **inside** `FunctionInvokingChatClient`, so a single provider round-trip is retried — never the whole tool-calling loop (which would re-execute tools). Exhausted retries surface as `LlmUnavailableException` (→ 502).
- **In-process MCP** — the app hosts an MCP server (`AddMcpServer().WithToolsFromAssembly(...)`, mapped at `/mcp`) and connects to it from the same process via an MCP client. `McpStartupService` (`IHostedService`) boots the client after Kestrel is listening. Tools are `[McpServerTool]` methods in `Application/Tools/TutorialTools.cs`.
- **Agentic loop** (in the chat handler): user message → LLM → tool-call decision → MCP tool execution → result injection → final response.
- **Chat identity** flows via the `X-User-Id` header (set by the Angular `user-id.interceptor`), not a token. History is persisted through `IChatHistoryStore` (`MongoChatHistoryStore`).
- **Embeddings** — a parallel seam to chat: `IEmbeddingGeneratorFactory` maps an `LlmProvider` to a cached `IEmbeddingGenerator<string, Embedding<float>>`, wrapped by `ResilientEmbeddingGenerator` (same policy as `ResilientChatClient`). Not every provider implements it — `LlmProvider.Claude` throws `NotSupportedException` (no Anthropic embeddings API). Chat and embedding model ids are configured separately (`GeminiOptions.Model` vs `GeminiOptions.EmbeddingModel`); they version independently and vectors are only comparable within one model.
- **Prompt-injection trust boundary** (`Application/Common/Ai/LlmContentGuard.cs`) — `WrapUntrustedContent` fences untrusted text (tool results, user text fed into one-shot LLM features) so a forged fence marker can't escape into instruction space; `SanitizeDisplayText` allow-lists model *output* before it reaches a UI/log/DB. In `SendChatCommandHandler`, every MCP tool result is wrapped, and a `ToolResultTrustBoundary` system message is prepended to the transient call list only (never to `threadMessages`, so it's never persisted).

### Real-time

SignalR `NotificationHub` mapped at `/notifications`; backend pushes via typed `INotificationService` (`SignalRNotificationService`). The Angular `SignalrService` subscribes with auto-reconnect and exposes an RxJS `Observable`.

### Persistence & caching

- **MongoDB** via a custom generic `IDatabaseContext` (CRUD, offset + cursor pagination, ACID transactions with `IClientSessionHandle`, connection-pool tuning). Repositories and the DB context are registered as **singletons**. The `Email` value object is stored via a custom `EmailSerializer` (registered in `ConfigurationSettingExtensions`).
- **Redis** distributed cache backs a **cache-aside** pattern on read-heavy paths (e.g. blog reads) — see `BlogCacheAsideTests`.
- **Indexes** are ensured at startup by `MongoIndexInitializer` (`IHostedService`). Each collection declares its indexes in an `IMongoIndexConfiguration` under `Infrastructure/Persistence/Indexing/`, assembly-scanned at registration — to index a new collection, add one config class. Creation is idempotent and non-blocking: failures are logged loudly but never stop startup.

### Frontend

Angular standalone components (`inject()` API, `@if`/`@for` control flow). State via NgRx (actions/reducers/effects/selectors) for subject/course data. Lazy-loaded feature routes (`Auth`, `Blog`, `Courses`, `dashboard`) under multiple layouts (`MainLayout`, `CourseLayout`). Config comes from `ConfigService` + `environments/`.

## Configuration

Config is bound via the Options pattern (`IOptions<T>`) from `appsettings.json` sections: `McpServer`, `GeminiOptions`, `ClaudeOptions`, `BrevoEmail`, `Auth:PasswordReset`, plus Mongo and Redis settings. Provide LLM API keys, the MongoDB connection string, and the Redis connection before running. CORS allows `http://localhost:4200` and the deployed Render frontend.

**Redis per environment:** base `appsettings.json` leaves `ConnectionStrings:Redis` empty (empty → falls back to in-memory `IDistributedCache`, so the app still boots). Local dev sets `localhost:6379` in `appsettings.Development.json`. Production never commits a connection string — set the `ConnectionStrings__Redis` environment variable on the host (ASP.NET maps `__` → `ConnectionStrings:Redis`, overriding the empty base).
