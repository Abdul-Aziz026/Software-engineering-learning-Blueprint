# SKILLS MATRIX

Ratings: **weak** · **ok** · **strong** · `—` = not covered yet.
Updated the day a topic is taught, and re-rated on retrieval days (Days 7, 14, 21, 28, 35, 44, 56, 64, 90).

Rule of thumb for self-rating:
- **weak** — I need to look it up before I can explain it.
- **ok** — I can explain it, but I'd hesitate to defend the design choice in a review.
- **strong** — I can explain it, code it cold, *and* argue when NOT to use it.

---

## Month 1 — OOD + SOLID

### Week 1 — OOP in depth
| Day | Topic | Rating | Last touched | Notes |
|---|---|---|---|---|
| 1 | Encapsulation as invariant protection | ok | 2026-07-25 | Taught today. Key shift: private fields ≠ encapsulation; the invariant is the point. Re-test on Day 7. |
| 2 | Abstraction vs Encapsulation (`IPaymentProcessor`) | — | | |
| 3 | Inheritance: when to use / when NOT ("is-a" test) | — | | |
| 4 | Composition over Inheritance | — | | |
| 5 | Polymorphism: subtype vs ad-hoc; vtable | — | | |
| 6 | Coupling & Cohesion | — | | |

### Week 2 — SOLID (first three)
| Day | Topic | Rating | Last touched | Notes |
|---|---|---|---|---|
| 8 | SRP — "one reason to change" | — | | |
| 9 | SRP practice on an Orbitax handler | — | | |
| 10 | OCP — discount calculator | — | | |
| 11 | OCP via Strategy | — | | |
| 12 | LSP — Square/Rectangle break | — | | |
| 13 | LSP practice — find a broken subtype | — | | |

### Week 3 — SOLID (last two) + design judgment
| Day | Topic | Rating | Last touched | Notes |
|---|---|---|---|---|
| 15 | ISP — split a fat `IRepository` | — | | |
| 16 | ISP practice — review MongoDB repository | — | | |
| 17 | DIP — depend on abstractions; `Program.cs` DI | — | | |
| 18 | DIP vs Dependency Injection | — | | |
| 19 | All SOLID together — small module | — | | |
| 20 | Code smells: long method, god class, feature envy | — | | |

### Week 4 — Consolidation project
| Day | Topic | Rating | Last touched | Notes |
|---|---|---|---|---|
| 22–26 | SOLID + OOD mini-project (tax filing validator / parking lot) | — | | |
| 27 | Self-review: which principle did I break? | — | | |

## Month 2 — 23 GoF Design Patterns

### Creational (5)
| Day | Pattern | Rating | Last touched | Notes |
|---|---|---|---|---|
| 29 | Factory Method | — | | |
| 30 | Abstract Factory | — | | |
| 31 | Builder | — | | Ties to GIR XML generation |
| 32 | Prototype | — | | |
| 33 | Singleton (+ why often anti-pattern) | — | | |
| 34 | Creational practice | — | | |

### Structural (7)
| Day | Pattern | Rating | Last touched | Notes |
|---|---|---|---|---|
| 36 | Adapter | — | | TTS abstraction is one |
| 37 | Decorator | — | | Foundation of Polly |
| 38 | Facade | — | | |
| 39 | Composite | — | | |
| 40 | Proxy | — | | |
| 41 | Bridge | — | | |
| 42 | Flyweight | — | | |
| 43 | Structural practice | — | | |

### Behavioral (11)
| Day | Pattern | Rating | Last touched | Notes |
|---|---|---|---|---|
| 45 | Strategy | — | | Ties back to OCP |
| 46 | Observer | — | | Foundation of SignalR |
| 47 | Command | — | | MediatR |
| 48 | Mediator | — | | Where "MediatR" comes from |
| 49 | Template Method | — | | |
| 50 | Chain of Responsibility | — | | Pipeline behaviors |
| 51 | State | — | | |
| 52 | Iterator | — | | |
| 53 | Visitor | — | | |
| 54 | Memento | — | | |
| 55 | Interpreter | — | | |

## Month 3 — Integration & Depth
| Day | Topic | Rating | Last touched | Notes |
|---|---|---|---|---|
| 57 | Hunt 10 patterns in the Orbitax codebase | — | | |
| 58 | Anti-patterns / over-engineering | — | | |
| 59 | Pattern combinations | — | | |
| 60 | Refactoring to patterns | — | | |
| 61–63 | Mid-size system design (URL shortener / rate limiter) | — | | |
| 64 | Blind test — 23 intents from memory | — | | |
| 65 | Strategy vs State, explained aloud | — | | |
| 66 | Factory vs Abstract Factory vs Builder | — | | |
| 67 | Choose patterns for a given problem + justify | — | | |
| 68–70 | Capstone design + defence | — | | |
| 71–89 | Depth rotation (re-implement weak patterns, trade-off drills, OSS reading) | — | | |
| 90 | Graduation review | — | | |
