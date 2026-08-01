# Day 04 — my notes

Topic: **Composition over Inheritance — build the same feature both ways, feel the difference.**

---

## Self-check (answer in my own words, don't peek at the session file)

> What does composition give me that inheritance simply cannot?

**My answer:**

---

## Task 1 — the class explosion, counted by hand

I wrote all 6 `*To*Exporter` classes in `Bad Example/`.

| Question | Number |
|---|---|
| Classes needed for 2 formats × 3 destinations | ______ |
| Times I re-wrote the CSV-building logic | ______ |
| Times I re-wrote the FTP-sending logic | ______ |
| Extra classes if I add JSON | ______ |
| Total classes after adding an S3 destination too | ______ |

- Growth formula for inheritance here: `____ × ____`
- **Which duplicated block would bite me first if the CSV escaping rule changed?**

---

## Task 1e — the moment I got stuck (most important box on this page)

I had a `CsvToFtpExporter`. FTP failed. I tried to make **that same object** write to disk.

- **What I tried:**
- **Why it was impossible, in one line:**

> *(If the sentence doesn't mention that the destination is baked into the **type**, keep rewriting it until it does.)*

---

## Task 2 — the composed version

| Question | Number |
|---|---|
| Small classes in `Good Example/` (formatters + destinations) | ______ |
| Combinations they cover | ______ |
| New classes to add JSON | ______ |
| New combinations that came free with it | ______ |
| Same change in `Bad Example/` would have cost | ______ |

- Growth formula for composition here: `____ + ____`
- **Cost I paid for this:** (longer `new`, extra interfaces to read, anything else?)
- **Was it worth it here?** (answer honestly — sometimes the answer is no)

---

## Task 3 — runtime swap, seen with my own eyes

- Console output before the swap:
- Console output after `UseDestination(new DiskDestination())`:
- **Was it the same object instance?** (check the reference — this is the whole point)
- Could I have done this in `Bad Example/`? ______ Why not?

---

## The judgment call (the architect bit)

Composition is not automatically better. Where would it have been over-engineering today?

- **If destination never varied, the right design would be:**
- **Signal that told me composition was actually needed:**
  - [ ] more than one thing varies independently
  - [ ] behaviour must change at runtime
  - [ ] subclasses use only a fraction of the base
- **A place in my own code where I'd add an interface but shouldn't:**

---

## Optional — `readonly` trade-off (TODO 4a)

I made `_destination` readonly and deleted `UseDestination`.

- **What I lost:**
- **What I gained:** *(Day 1 language: which invariant got stronger?)*
- **Which one would I ship, and what does that decision depend on?**

---

## Hunting composition in the Orbitax codebase

Pick one MediatR handler and read only its constructor.

- **Handler:**
- **Dependencies injected ("tools in the bag"):** ______
- Does it inherit from any `Base*` class? ______
- Where is each of those dependencies wired up? (`Program.cs` / a DI extension method): ______
- **One line: why is the DI container just composition at industrial scale?**

**Bonus hunt — GIR XML generation:** are "how it's formatted" and "where it goes"
actually separate there, or is one class doing both?

- **Finding:** *(keep this — Day 8 SRP will use it)*

---

## Questions that came up while coding

-
