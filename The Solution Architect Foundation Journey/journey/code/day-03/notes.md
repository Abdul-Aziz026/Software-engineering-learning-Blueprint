# Day 03 — my notes

Topic: **Inheritance — when to use, when NOT. The "is-a" test.**

---

## Self-check (answer in my own words, don't peek at the session file)

> It sounds like "is-a" in plain English. When is inheritance still the wrong call?

**My answer:**

---

## Task 1 — what `BrokenStack : List<T>` inherited that it should never have had

| # | Inherited member | Which invariant it breaks | Would a teammate plausibly call it? |
|---|---|---|---|
| 1 | | | |
| 2 | | | |
| 3 | | | |

- After `Insert(0, "sneaked-in")` and `Reverse()`, `Pop()` returned: ______
- What LIFO said it should return: ______
- Who is at fault here — the caller, or the person who wrote `: List<T>`?

---

## Task 2 — composition version

- Lines of forwarding code I had to write by hand: ______
- Public members on `BrokenStack` (inherited included) vs on `SafeStack`: ______ vs ______
- **Was that forwarding worth it?** (answer honestly, this is the trade-off of the whole day)

---

## Task 3 — the fragile base class

I added `Delete()` to `LegacyReport`.

- Files I edited: ______
- Files whose behaviour changed: ______
- **What this means for base classes I don't own** (framework / NuGet / another team's):

---

## The three questions, applied to a real base class in my code

Pick one real `Base*` class from the Orbitax codebase.

- **Base class name:**
- **Public + protected members on it:** ______
- **Subclass I checked:** ______
- **How many of those members that subclass actually uses:** ______

| # | Question | Yes / No |
|---|---|---|
| 1 | Is every public member of the base meaningful on this subclass? | |
| 2 | If the base gains a new method tomorrow, am I relaxed about it? | |
| 3 | Do I own the base class? | |

**Verdict** (keep the inheritance / turn it into an injected service / turn it into an interface):

---

## Substitutability check (seed for Day 12 — LSP)

Find one place where a subclass surprises the caller: it throws where the base doesn't,
returns null where the base never does, ignores a parameter, or silently does less.

- **Base type:**
- **Subclass:**
- **How the caller can tell the difference:**
- **Would swapping them in production break anything?**

---

## Where inheritance IS earning its keep in my code

(Exception hierarchies are the usual honest example — `catch (DomainException)` is a real win.)

- **Hierarchy:**
- **What I gain that composition would not give me** (type identity? polymorphic dispatch? an enforced rule?):
- **How deep is it?** ______ levels. *(More than 2 → look again.)*

---

## Questions that came up while coding

-
