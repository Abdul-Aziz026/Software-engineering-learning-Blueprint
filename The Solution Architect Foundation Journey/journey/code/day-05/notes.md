# Day 05 — my notes

Topic: **Polymorphism — subtype vs ad-hoc, and the vtable behind `override`.**

---

## Self-check (answer in my own words, don't peek at the session file)

> From the CLR's point of view, what exactly is the difference between `override` and `new`?

**My answer:**

---

## Task 1 — one object, two answers

```csharp
SavingsAccount savings = new SavingsAccount(100_000m);
Account        asBase  = savings;   // same object
```

| Call | Number printed |
|---|---|
| `savings.MonthlyInterest()` | ______ |
| `asBase.MonthlyInterest()` | ______ |
| `total` from the `foreach (Account a in ...)` loop | ______ |

- **Was it the same object instance?** (check the reference — the whole point) ______
- **In one line: what actually decided which method ran?**

> *(If the sentence doesn't mention the **declared type of the reference**, rewrite it until it does.)*

- **Why would a unit test have passed while production paid ৳0?**

---

## Task 1e — the exact compiler warning

The warning I got before I typed `new`:

```

```

- **What was the compiler actually trying to tell me?**

---

## Task 2 — `virtual` / `override`

| Call | Number printed now |
|---|---|
| `savings.MonthlyInterest()` | ______ |
| `asBase.MonthlyInterest()` | ______ |

- Lines I changed inside the `foreach` loop: ______ *(should be 0)*
- **Why is "I didn't touch the loop" the important part?**

---

## Task 3 — the compiler as a guard (most important box on this page)

I wrote `StudentAccount : Account` and deliberately left out `MonthlyInterest()`.

**Exact compiler error:**

```

```

- **Why was `abstract` the right call here instead of `virtual` with `return 0m`?**
- **In one line: what was the lie in the original base class?**

---

## Task 4 — the switch version, counted by hand

| Question | Number |
|---|---|
| Switches I wrote (interest, fee, statement label) | ______ |
| Places I had to edit to add `StudentAccount` | ______ |
| Times the compiler stopped me | ______ |
| Places I *forgot* on the first pass | ______ |

- **Which of the three switches actually belonged outside the type, and why?**
  *(Whose business is `StatementLabel` — the account's, or the UI's?)*

---

## Task 5 — ad-hoc vs subtype, proved

- `Log(acc)` printed: ______
- `acc.MonthlyInterest()` ran: ______
- `Log((SavingsAccount)acc)` printed: ______

**One line each:**

- Who decided the `Log` call, and based on what? ______
- Who decided the `MonthlyInterest` call, and based on what? ______

- **Nothing about the object changed between line 1 and line 3. So what did change?**

---

## The vtable, drawn from memory

Draw the two tables without looking at the session file:

```
Account:                     SavingsAccount with `override`:   SavingsAccount with `new`:

slot0 →                      slot0 →                            slot0 →
                                                                slot1 →
```

- **Why does a `Account`-typed reference always ask for slot 0?**

---

## The judgment call (the architect bit)

- **A method in my code I was tempted to make `virtual` but shouldn't:**
  *(what promise would I be making forever?)*
- **A behaviour that does NOT belong inside the type, even though a switch on type is ugly:**
- **Where did the `if (a is ...)` decision actually go after I removed it?** *(it didn't vanish)*

---

## Optional — `sealed override`

- Compiler error when I tried to override a sealed method:
- **Why would I deliberately close the door?**

---

## Hunting this in the Orbitax codebase

**Hunt 1 — live landmines.** `grep -rn "public new "` (also try `protected new `).

- **Found:** ______
- If found: is the base member virtual? Would calling through the base type break?

**Hunt 2 — a type/enum switch.**

- **File / method:**
- **Is this behaviour the type's own business?** yes / no
- **If yes → refactor candidate.** *(Keep this — Day 10 OCP will use it.)*

**Hunt 3 — the vtable I use every day.**

- One class where I've overridden `ToString()`: ______
- One place Moq forced me to mock an interface instead of a concrete class: ______
- **One line: why can Moq only intercept virtual/abstract/interface members?**

**Hunt 4 — MediatR is *not* vtable dispatch.**

- Where does the request→handler mapping actually get built? ______
- **If I forget to register a handler, when do I find out — build time or runtime?** ______
- *(Keep this — Days 47/48.)*

---

## Questions that came up while coding

-
