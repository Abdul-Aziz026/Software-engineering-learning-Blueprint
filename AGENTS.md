# AGENTS.md

Guidance for Codex (Codex.ai/code) when working in this repository.

> Always answer in English (mixed) language.

## Project

Full-stack, AI-powered learning platform:

- **Backend** — ASP.NET Core (.NET 10), strict Clean Architecture.
- **Frontend** — Angular SPA (`Frontend/Dashboard`).
- **Differentiator** — in-process MCP server + client driving an agentic LLM loop; provider (Gemini or Codex) switchable at runtime.

## Commands

**Backend** (from `Backend/`)
```bash
dotnet build BackendBluePrint.slnx     # build
dotnet run --project API               # run API (Swagger at /swagger in Dev)
dotnet test                            # all xUnit tests
dotnet test --filter "FullyQualifiedName~EmailTests"       # one class/test
```

**Frontend** (from `Frontend/Dashboard/`)
```bash
npm install
npm start        # ng serve → http://localhost:4200
npm run build    # production build
npm test         # vitest unit tests
```

**Full stack:** `docker compose up --build` (frontend → :4200, backend → :5000)

**Test layering:** tests in `Backend/Tests/` (xUnit). Unit tests reference only `Domain` + `Application`. Integration tests (`Backend/Tests/Integration/`) reference `API`, boot the host via `WebApplicationFactory<Program>`, and need Docker (Testcontainers → throwaway MongoDB).

## Architecture

### Backend layering (dependencies flow inward only)

`API` → `Infrastructure` → `Application` → `Domain`. All target `net10.0`.

- `Domain` has **zero** external dependencies.
- `Application`/`Domain` never depend on `Infrastructure`/`API`.
- Cross-layer references go through interfaces, never concrete classes.

| Project | Role | References |
|---|---|---|
| **Domain** | Entities, enums, value objects (`Email`), exceptions, repository interfaces. | nothing |
| **Application** | CQRS slices, DTOs, interfaces (`Common/Interfaces/`), MCP tools (`Tools/TutorialTools.cs`). | Domain, Contracts |
| **Contracts** | Shared MassTransit message contracts (leaf shared kernel). | MediatR |
| **Infrastructure** | MongoDB, Redis, SignalR, LLM clients + `LlmFactory`, MCP, `MessageBus`, email, hashing. | Application, Domain |
| **API** | Thin controllers, middleware, DI (`Program.cs`). | Application, Infrastructure, Contracts |

API registration lives in `Extensions/`: `ServiceCollectionExtensions` (repos/cache/security/bus), `MasstransitAndMediatRExtensions` (MediatR + validators + `ValidationBehavior`), `ConfigurationSettingExtensions` (options + BSON serializers).

### Request flow (CQRS)

Controllers are thin — inject `IMessageBus` and call `SendAsync<TCommand, TResponse>`. Every write is a `Command`, every read a `Query`, each with a handler.

- **Validation** — `ValidationBehavior<,>` MediatR behavior; FluentValidation validators auto-discovered from Application assembly. Throws `ValidationException`.
- **Middleware:** `CorrelationIdMiddleware` → `GlobalExceptionMiddleware` (maps domain exceptions to HTTP; `LlmUnavailableException` → 502).
- **Errors** — RFC 7807 `ProblemDetails` with a `correlationId` extension. The Angular chat UI depends on this — keep new error paths on it.

**Adding a feature:** create `Application/Features/<Area>/{Commands|Queries}/<Name>/`, add request + handler (+ validator), then a controller action dispatching via `IMessageBus`. Handlers/validators are assembly-scanned — no DI registration.

### AI / agentic subsystem

- **Provider strategy** — `ILlmFactory` returns the right `IChatClient` (`GeminiChatClient`/`ClaudeChatClient`) per runtime `LlmProvider`.
- **Resilience** — `ResilientChatClient` wraps providers (per-attempt timeout + bounded retry with backoff/jitter). Sits **inside** `FunctionInvokingChatClient`, so only a single round-trip retries — never the tool-calling loop. Exhausted retries → `LlmUnavailableException` (502).
- **In-process MCP** — app hosts an MCP server (mapped at `/mcp`) and connects from the same process; `McpStartupService` boots the client after Kestrel listens. Tools are `[McpServerTool]` methods in `TutorialTools.cs`.
- **Agentic loop** (chat handler): user message → LLM → tool-call decision → MCP execution → result injection → final response.
- **Chat identity** — via `X-User-Id` header (Angular `user-id.interceptor`), not a token. History persisted through `IChatHistoryStore` (`MongoChatHistoryStore`).
- **Embeddings** — parallel seam: `IEmbeddingGeneratorFactory` maps `LlmProvider` to a cached generator, wrapped by `ResilientEmbeddingGenerator`. `LlmProvider.Codex` throws `NotSupportedException` (no Anthropic embeddings). Chat vs embedding model ids configured separately; vectors only comparable within one model.
- **Prompt-injection boundary** (`Common/Ai/LlmContentGuard.cs`) — `WrapUntrustedContent` fences untrusted text (tool results, user text); `SanitizeDisplayText` allow-lists model output before UI/log/DB. In `SendChatCommandHandler`, tool results are wrapped and a `ToolResultTrustBoundary` system message is prepended to the transient call list only (never persisted).

### Real-time

SignalR `NotificationHub` at `/notifications`; backend pushes via `INotificationService` (`SignalRNotificationService`). Angular `SignalrService` subscribes with auto-reconnect, exposes an RxJS `Observable`.

### Persistence & caching

- **MongoDB** — custom generic `IDatabaseContext` (CRUD, offset + cursor pagination, ACID transactions, pool tuning). Repositories + context are **singletons**. `Email` stored via custom `EmailSerializer`.
- **Redis** — distributed cache backing a **cache-aside** pattern on read-heavy paths (see `BlogCacheAsideTests`).
- **Indexes** — ensured at startup by `MongoIndexInitializer`. Each collection declares indexes in an `IMongoIndexConfiguration` (`Persistence/Indexing/`), assembly-scanned. Idempotent, non-blocking — failures logged but never stop startup.

### Frontend

Angular standalone components (`inject()`, `@if`/`@for`). State via NgRx for subject/course data. Lazy-loaded routes (`Auth`, `Blog`, `Courses`, `dashboard`) under `MainLayout`/`CourseLayout`. Config via `ConfigService` + `environments/`.

## Configuration

Bound via Options pattern (`IOptions<T>`) from `appsettings.json`: `McpServer`, `GeminiOptions`, `ClaudeOptions`, `BrevoEmail`, `Auth:PasswordReset`, plus Mongo/Redis. Provide LLM API keys, Mongo connection string, and Redis connection before running. CORS allows `http://localhost:4200` and the deployed Render frontend.

**Redis per environment:** base `appsettings.json` leaves `ConnectionStrings:Redis` empty (→ in-memory fallback). Dev uses `localhost:6379`. Production sets the `ConnectionStrings__Redis` env var on the host (`__` → `:`).
