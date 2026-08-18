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
| 1 | Encapsulation as invariant protection | ok | 2026-07-26 | Taught Day 1. Key shift: private fields ≠ encapsulation; the invariant is the point. Re-test on Day 7. |
| 2 | Abstraction vs Encapsulation (`IPaymentProcessor`) | ok | 2026-07-31 | Encapsulation = who may touch the state; abstraction = what the outside sees. Golden test: can you name the library from the interface signature? Session was rewritten simpler on request — `IdempotencyKey` / `Pending` moved to a bonus note (revisit Day 12 & 15). Re-test on Day 7. |
| 3 | Inheritance: when to use / when NOT ("is-a" test) | ok | 2026-07-31 | Taught Day 3. Core shift: inheritance = adoption (full public surface + all future base changes), not code reuse. English "is-a" is unreliable — test behavioural substitutability. Fragile base class demoed via `LegacyReport.Delete()`. Good-inheritance shape: abstract base, ~no state, enforces one rule, 1 level deep. Seeds LSP (Day 12) and Template Method (Day 49). Re-test on Day 7. |
| 4 | Composition over Inheritance | ok | 2026-08-01 | Taught Day 4. Two-axis exporter (format × destination) shows the `M × N` vs `M + N` explosion; the decisive win is runtime behaviour swap (`UseDestination`), impossible with inheritance because the behaviour is welded into the type. Decision rule taught: compose when >1 thing varies independently, when behaviour must change at runtime, or when subclasses use only a fraction of the base — otherwise plain subclassing is the simpler correct answer. Counter-balance kept explicit: Day 3's `TaxFiling` base still right, because its job is *enforcing a rule*, not distributing behaviour. Seeds Strategy (Day 45) and Bridge (Day 41); the `readonly _destination` drill seeds the Day 1 invariant-vs-flexibility trade-off. Re-test on Day 7. |
| 5 | Polymorphism: subtype vs ad-hoc; vtable | ok | 2026-08-02 | Taught Day 5. Core demo: one object, two answers — `new` vs `override` on `Account.MonthlyInterest()`, a bug that never crashes and just pays the wrong money. Mechanism taught literally, not as metaphor: `override` = ECMA-335 `reuseslot` (seizes the base's vtable slot), `new` = `newslot` (adds one beside it, base slot untouched); a base-typed reference always asks for the base's slot number. Second half — ad-hoc (overload, chosen by the **compiler** from the *declared* type) vs subtype (override, chosen by the **CLR** from the *actual* type); proved with `Log(acc)` vs `Log((SavingsAccount)acc)` on the same instance. Parametric/generics named only. Architect points: (i) the real culprit was the base's fake `return 0m` — no sensible default ⇒ `abstract`, not `virtual`, so the compiler stops you instead of the customer; (ii) `virtual` is a permanent public promise — sealed by default; (iii) `if (x is ...)` is right when the behaviour is a *consumer's* business, not the type's (`RenderHtml` on a domain type is worse) — seeds Visitor (Day 53); (iv) polymorphism moves the decision to one place rather than deleting it — seeds Factory (Day 29). Orbitax hooks: `object`'s virtual `ToString`/`Equals`/`GetHashCode`; why Moq only intercepts virtual/abstract/interface (*"Invalid setup on a non-virtual … member"*); and MediatR's dispatch is a **container dictionary lookup, not vtable** — configurable at startup, fails at runtime not compile time (seeds Days 47–48). Hunts assigned: `grep "public new "`, and one type/enum switch to carry into Day 10 (OCP). Re-test on Day 7 — check specifically whether he can state the `new`/`override` difference in *slot* terms and not just "it hides it".
| 6 | Coupling & Cohesion | ok | 2026-08-05 | Taught Day 6. First day the question moves from *inside one class* to *between classes* — and the explicit framing given: SRP = raise cohesion, DIP = lower coupling, so the rest of Month 1 is commentary on today. Bad example: a 6-job `FilingService` (validate → calculate → XML → SQL insert → SMTP → file log) with 11 concrete external names hard-wired in. Two diseases named separately: **low cohesion** diagnosed by the "আর" test (if describing the class needs the word *and*, cohesion has already leaked) and by the change-driver table (6 different people, 6 different clocks, 1 file); **high coupling** diagnosed by trying to unit-test `0.25m` and discovering you need a live SQL Server + SMTP + `C:\logs\` write permission — the taught line is *"hard to test" is never a testing complaint, it's a design diagnosis; the unit test is the honest ruler for coupling.* Fix given in two deliberately separate steps so the causes don't blur: step 1 = scissors only (`FilingValidator`, `TaxCalculator` — **no interfaces**, on purpose), step 2 = contracts only (`IFilingStore`, `INotifier`). Key restraint taught: interface only where a real alternative exists (DB, SMTP), not because "abstraction is good". Architect points: (i) coupling can never be zero — zero coupling is zero work; the question is never *is there coupling* but *coupling to what*; (ii) the stability rule — depend downward, on what changes more slowly than you (named as the seed of DIP, Day 17); (iii) the counter-error is taught as its own drill (PART 4) — splitting `FilingValidator` into three one-rule classes is low cohesion in a different costume, because all three change for the same compliance reason; (iv) the three conditions that justify paying for the split (second implementation actually needed / parts change on different clocks / infra required to test) — none present ⇒ one class with private methods is the simpler correct answer. Orbitax hooks: Clean Architecture layers = coupling direction enforced by project references; MediatR handler = the unit of cohesion; FluentValidation = step 1 pre-built; Pipeline behaviours = protecting handler cohesion (seeds Day 50); Polly (seeds Day 37). Hunts assigned: biggest handler → table of jobs vs who-asks-for-the-change (**carry into Day 9 SRP**), and grep the Domain project for infra namespaces (arrow pointing the wrong way). Re-test on Day 7 — check specifically that he cuts along *change reason*, not along *what the code does*, and that he can argue when the refactor would be over-engineering.
| 7 | 🔁 **Retrieval Day — Week 1 (Days 1–6)** | *pending self-rate* | 2026-08-11 | Day 7 issued. Non-interactive self-test, no new material: PART A rapid-fire (10) · PART B day-by-day (33 questions across Days 1–6) · PART C six code-judgment snippets · PART D five architect-judgment prompts · PART E write-from-scratch (guarded `BankAccount`, composed `Stack<T>`, `abstract Account` + 2 subclasses) · PART F self-rating rubric. Answers quarantined at the bottom behind an explicit "এখন স্ক্রল কোরো না" instruction plus a 60-second-struggle rule — because recognition ≠ recall, and reading the answers early converts a memory test into a reading exercise. **Deliberate design choices:** (i) **C6 is a trap** — a plain `CreateFilingRequest` DTO with `{ get; set; }` that is *already correct*; after six days of "here's the bad version," the reflex to refactor everything is itself the next junior mistake, so one question had to have "nothing is wrong" as the answer. (ii) **A5 explicitly rejects "it hides it"** and demands the *slot* formulation (`reuseslot`/`newslot`) — discharges the standing Day 5 re-test flag. (iii) **B30 / C4** re-test that the real Day 5 culprit is the base's fake default, not the `new` keyword. (iv) **B43 + D3** discharge the Day 6 flag — cut along change reason, and argue when the refactor would be over-engineering. (v) **D5** asks him to bind all six days into one sentence — the actual measure of whether Week 1 consolidated (offered answer: Week 1 = six versions of *"কে কার উপর ক্ষমতা রাখে?"* — inside one class → between two types → across the system). (vi) **D4** tests whether he sees Stack:List, the leaky `IPaymentProcessor`, and the leaked `List<T>` as three *different* leaks (public surface / implementation detail / mutable state) rather than one blur. **Rubric taught:** `strong` requires stating when the technique is over-engineering, not merely what it is — so "I can explain it" caps at `ok` by design. **Rule set:** anything self-rated `weak` ⇒ re-read that session file for 10 min before the next lesson, and it is re-asked on Day 14. Rows 1–6 intentionally left at `ok` — they must be re-rated from Aziz's actual written answers, not assumed to have held. |

### Week 2 — SOLID (first three)
| Day | Topic | Rating | Last touched | Notes |
|---|---|---|---|---|
| 8 | SRP — "one reason to change" | *pending self-rate* | 2026-08-14 | Taught Day 8. **The one shift:** Day 6 asked the cohesion question *of the code* ("do these belong together?"); Day 8 asks it *of the people* ("who can order this changed?"). Uncle Bob's corrected definition taught explicitly — **"responsible *to* one actor"**, not "responsible *for* one thing" — so the *reason* in "one reason to change" is **a person**, never a technical category. Bad example: `Employee` with `CalculatePay()` (CFO) + `ReportHours()` (HR) + `Save()` (DBA) sharing one `private RegularHours()` — deliberately chosen because it looks **DRY and would pass code review**. Killer demo (the whole day): CFO asks for 40 ⇒ 45, dev changes the shared helper, build green, payroll test passes — and HR's labour-ministry report silently reports 5 extra hours per employee for three weeks. No compiler error, no failing test, just a wrong number. Analogy: two tenants, one light switch — they didn't share a wire, they shared *control*. **Concept named:** accidental duplication vs real duplication, with the test given as a single question — *"if one actor asks for this change, would the other want the same change?"* yes ⇒ merge, no ⇒ keep apart. Hence the deliberate `Sum()` written twice in the good version, and the deliberate refusal to merge `40` and `45`; taught as **DRY is about duplicated *knowledge*, not duplicated *characters*.** Fix kept minimal on purpose — three plain classes (`PayCalculator` / `HourReporter` / `EmployeeRepository`), **no interfaces, no DI** (those are Day 17's job). **Two mis-readings pre-empted, both as their own drills:** (i) *"SRP = small classes"* — `BankAccount` has 3 methods but **1 actor**, so splitting it into `Depositor`/`Withdrawer`/`BalanceReader` destroys the Day 1 invariant; PART 5 makes him perform the bad split and discover nobody owns `balance >= 0` anymore (direct callback to Day 6's three one-line validators). (ii) *anemic domain model* — answered with the line **invariant stays inside the entity, policy goes outside** (`rate < 0` kept in the ctor, the 40/45 cap moved out). Cost acknowledged: caller now juggles three objects ⇒ seeds Facade (Day 38), with the warning *split first, mask later — the reverse is just the bad version renamed*. Architect bridge: the boundaries came out looking like the org chart, which is **Conway's Law**, and the microservice-boundary question is this same question at deployment scale. Orbitax hooks: MediatR handler = the unit of SRP; **GIR XML tooling named as the exact place this bug lives** (one shared formatter serving OECD schema *and* internal reconciliation = two masters, one switch); DTO ≠ domain entity because API consumers and business rules are two actors; FluentValidation = compliance's own file; Pipeline behaviours = ops actor kept out of the business handler (Day 50). Hunts assigned: find a `private` helper called by 2+ public methods serving different features (**his own `RegularHours()`**), and count how many departmental worlds appear in the biggest handler's `using` list — **both feed Day 9 directly**, along with Day 6's handler/change-driver table. Re-test on Day 14 — check specifically that he counts **actors, not methods**, and that he can argue the `BankAccount` case where SRP says *don't split*. |
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
