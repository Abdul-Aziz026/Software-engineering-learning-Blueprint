# Day 02 — my notes

## Self-check (answer in my own words, don't peek at the session file)

> A class is fully encapsulated — every field private, every door guarded. How can its
> abstraction still be terrible? Give an example.

**My answer:** abstraction break or wrong abstraction can break the whole application.


---

## Task 1 — the three leaks in `ILeakyPaymentProcessor`

| # | The leak | Why it hurts | What it should have been |
|---|---|---|---|
| 1 | StripeChargeResponse: vendor leak |  |  |
| 2 | amountInCents: domain leak |  |  |
| 3 | stripeCustomerId: user identity leak |  |  |

---

## Task 5 — breaking it on purpose

Added `Task<StripeCustomer> GetStripeCustomerAsync(string id)` to the interface, then tried to
implement it on `BankTransferPaymentProcessor`.

- **What I did** (throw / return null / return a fake):
- **Why it felt wrong:**
- **The name of this smell** (I'll meet it again on Day 12 and Day 15):

---

## Task 6 — leak hunt in my own codebase

Find one real interface in Orbitax that leaks a vendor/technology detail.

- **Interface name:**
- **Method or type that leaks:**
- **What it leaks** (SDK type? unit convention? vendor id? `IQueryable` / `FilterDefinition<T>` / `BsonDocument`? an OECD `XElement`?):
- **Who now knows about that technology and shouldn't:**
- **What it could have been instead** (name + signature):

### The golden test, applied

> Reading only the interface name and its parameter/return types — can I tell which library is
> behind it?

- Answer: yes / no
- If yes → it's a wrapper, not an abstraction.

### Clean Architecture check

> If I deleted the Infrastructure project, would Application still compile?

- Answer:
- If no, the leaks are:

---

## Judgment call — where I should NOT have abstracted

One interface in my code with exactly one implementation and no realistic second one:

- **Interface:**
- **Is it earning its keep, or is it ceremony?**
- **Was it created only to enable mocking?**

---

## Questions that came up while coding

- Interface এর নাম আর ওর সব parameter/return type পড়ে কি বলা যায় কোন library টা ব্যবহার হচ্ছে? বলা গেলে সেটা abstraction না, সেটা একটা wrapper.

