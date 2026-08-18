# Day 08 — my notes

Topic: **SRP — "one reason to change" আসলে "one actor to answer to".**

---

## Self-check (নিজের ভাষায়, session file না দেখে)

> Day 6 এর cohesion আর আজকের SRP — পার্থক্যটা এক লাইনে।
> (ইঙ্গিত: একটা কোডের দিকে তাকিয়ে প্রশ্ন, একটা মানুষের দিকে তাকিয়ে।)

**My answer:**

> "SRP মানে class ছোট রাখা" — এই বাক্যটা কেন ভুল?

**My answer:**

---

## Task 2 — ACTOR TABLE (bad `Employee`)

| Method | কে বদলাতে বলতে পারে | কেন |
|---|---|---|
| `CalculatePay()` |hr | need to calculate salary |
| `ReportHours()` | finance dept | need to report employee work hours |
| `Save()` | db dept | for save employee data |
| `RegularHours()` |hr, finance, db dept | ⬅️ এই ঘরে কয়টা নাম বসল? |

**মোট কতজন আলাদা actor এই এক class টা বদলাতে পারে?**  3 person.

---

## Task 3–5 — 🔴 নীরব bug টা নিজে ঘটানো (আজকের সবচেয়ে দামি বাক্স)

`DailyHours = [8, 8, 8, 8, 8, 6, 0]` (মোট 46) · `HourlyRate = 100`

**পরিবর্তনের আগে:**

| | মান |
|---|---|
| `CalculatePay()` | ______ |
| `ReportHours()` | ______ |

**CFO এর অনুরোধ চালানোর পরে (`RegularHours()` এ 40 ⇒ 45):**

| | মান |
|---|---|
| `CalculatePay()` | ______ |
| `ReportHours()` | ______ |
| Compiler error | ______ |
| Fail করা test সংখ্যা | ______ |

**HR কি এই পরিবর্তন চেয়েছিল?** ______

এক লাইনে লেখো — আজকের পুরো পাঠ এই বাক্যে:

```
HR এর সংখ্যা বদলে গেল, অথচ আমার build সবুজ ছিল, কারণ ______________________
```

**কঠিন প্রশ্ন:** বাস্তবে HR এর ওই test টা কি আদৌ লেখা থাকত? না থাকলে
ভুলটা কে, কবে ধরত?

**My answer:**

---

## Task 6 — invariant vs policy এর রেখা

`rate < 0` চেকটা `Employee` এর ভেতরে রাখলাম, কিন্তু 40/45 নিয়মটা বের করে দিলাম।
পার্থক্যটা কী?

**My answer:**

---

## Task 10 — good version এ CFO এর অনুরোধ

`PayCalculator` এ cap 45 ⇒ 50 করার পরে:

| Question | উত্তর |
|---|---|
| `HourReporter` এ কয়টা লাইন বদলাতে হলো | ______ |
| `Employee` এ কয়টা লাইন | ______ |
| HR এর test fail করল কি | ______ |
| CFO এর test fail করল কি (করা উচিত ছিল কি) | ______ |

---

## Task 11 — DRY এর সাথে তর্ক

**(a) 40 আর 45 এক করা কেন ভুল হতো?**

**My answer:**

**(b) `Sum()` কি এক করা যায়? এটা 40/45 এর চেয়ে আলাদা কেন?**
*(ইঙ্গিত: "যোগফল বের করা" — এটা কার মতামত? Finance এর? HR এর? নাকি কারো না?)*

**My answer:**

**(c) নিজের ভাষায় একটা পরীক্ষা লেখো, যেটা দিয়ে যেকোনো দুইটা একরকম কোড দেখে
বলতে পারবে ওটা real না accidental duplication:**

```

```

---

## Task 12 — 🔁 উল্টো drill: SRP এর অতি-প্রয়োগ

`BankAccount` কে `Depositor` / `Withdrawer` / `BalanceReader` এ ভাঙার পরে —

| Question | উত্তর |
|---|---|
| `balance` এখন কোথায় থাকে | ______ |
| তিনজনকে সেটা দিতে কী করতে হলো | ______ |
| `balance >= 0` কি এখনো অলঙ্ঘনীয় | ______ |
| কতজন actor আসলে এই তিনটা method বদলাতে বলে | ______ |

দুই লাইনে — এটা কেন খারাপ, আর কোন নিয়মটা আমাকে আগেই থামানো উচিত ছিল:

```

```

---

## Task 13 — 🎯 Orbitax hunt (Day 9 এ লাগবে)

আমার নিজের codebase এর `RegularHours()`:

| | |
|---|---|
| File | |
| `private` helper এর নাম | |
| ডাকে যে public method গুলো | |
| Actor #1 | |
| Actor #2 | |

**এটা আজ পর্যন্ত ফাটেনি — কেন? (ভাগ্য, নাকি সত্যিই একই actor?)**

**My answer:**

---

## Task 14 (stretch) — সবচেয়ে বড় handler এর `using` গণনা

| | |
|---|---|
| Handler নাম | |
| মোট `using` | |
| কয়টা আলাদা বিভাগের জগৎ (business / infra / logging / auth / …) | |

---

## আজকের এক লাইন (নিজের ভাষায়, session file দেখে না)

```

```

---

## Self-rating (SKILLS_MATRIX এর জন্য)

- **weak** — বলার আগে দেখে নিতে হয়।
- **ok** — বোঝাতে পারি, কিন্তু review তে design টা defend করতে দ্বিধা করব।
- **strong** — বোঝাতে পারি, ঠান্ডা মাথায় লিখতে পারি, **আর কখন এটা over-engineering
  সেটাও যুক্তি দিয়ে বলতে পারি।**

**Day 8 — SRP:** ______

*(`strong` লিখতে হলে Task 12 এর উত্তরটা পরিষ্কার হতে হবে — কখন কাটা যাবে **না**,
সেটা না জানলে SRP জানা হয়নি।)*
