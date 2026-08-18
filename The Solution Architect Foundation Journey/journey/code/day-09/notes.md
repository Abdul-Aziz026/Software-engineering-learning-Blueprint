# Day 09 — my notes

Topic: **SRP practice — চার ধাপের audit একটা আসল MediatR handler-এর উপর।**

আজকের এক লাইন: *call গোনো না, **decision** গোনো।*

---

## Task 1 — প্রথম আন্দাজ (bad handler টা লেখার পরে, table বানানোর আগে)

> "এই handler টা বদলাতে বলার ক্ষমতা কতজন মানুষের আছে?"

**আমার প্রথম আন্দাজ:** _____ জন

*(এখন table টা করো। পরে ফিরে এসে মিলিয়ে দেখবে — আন্দাজ আর audit-এর দূরত্বটাই
আজকের শেখার পরিমাপ।)*

---

## Task 2 — STEP 1 + 2: ACTOR + CLOCK TABLE

⚠️ session file-এর উত্তর দেখার **আগে** নিজে পূরণ করো।

| # | কাজ | কে বদলাতে বলে (actor) | কত ঘন ঘন বদলায় (clock) |
|---|---|---|---|
| 1 | logging | | |
| 2 | TIN + period validation | | |
| 3 | tax calculation | | |
| 4 | DTO → entity mapping | | |
| 5 | GIR XML payload | | |
| 6 | Mongo insert | | |
| 7 | audit trail | | |
| 8 | SignalR push | | |
| 9 | email | | |

**মোট আলাদা actor:** _____
**আমার প্রথম আন্দাজ ছিল:** _____ → **পার্থক্য কেন হলো?**

---

## Task 3 — STEP 3: গন্তব্য

প্রতিটার পাশে একটা: `⬇️Domain` / `⬇️Infra` / `⬆️Pipeline` / `➡️Handler`

| # | কাজ | গন্তব্য | কোন প্রশ্নে ঠিক করলাম |
|---|---|---|---|
| 1 | logging | | |
| 2 | validation | | |
| 3 | tax calculation | | |
| 4 | mapping | | |
| 5 | GIR XML | | |
| 6 | Mongo insert | | |
| 7 | audit trail | | |
| 8 | SignalR push | | |
| 9 | email | | |

### 🛑 audit trail — দুই দিকের যুক্তি

**Pipeline behaviour-এর পক্ষে:**

**Domain event-এর পক্ষে:**

**আমি বেছেছি:** ______ **কারণ:** ______

---

## Task 4–5 — Good version + mock ছাড়া test

`TaxCalculator` test:

| input | expected | পেলাম |
|---|---|---|
| IE · revenue 1000 · deductions 200 | 100 | |
| HU · revenue 500 · deductions 900 | 0 | |

**কয়টা mock লাগল?** _____

এক লাইনে লেখো:

```
এই test টা আগের design-এ লেখা সম্ভব ছিল না, কারণ ______________________
```

`TaxCalculator.cs`-এর `using` list পড়ো — কোনো infra নাম আছে?  ______

---

## Task 6 — PART B: ভুয়া refactor

সব কোড `FilingService.CreateFiling()`-এ সরানোর পরে:

| | মান |
|---|---|
| Handler-এ actor সংখ্যা | |
| `FilingService`-এ actor সংখ্যা | |
| Handler-এ লাইন সংখ্যা কমল? | |
| System-এ মোট actor-বিভ্রান্তি কমল? | |

```
লাইন সরানো refactor না, কারণ ______________________________________
```

---

## Task 7 — 🎯 আমার নিজের Orbitax handler-এর audit

| | |
|---|---|
| File (বা ছদ্মনাম) | |
| `Handle()` কত লাইন | |
| Constructor dependency কয়টা | |
| `using` কয়টা আলাদা জগৎ থেকে | |
| কয়টা আলাদা কাজ | |
| **কয়জন আলাদা actor** | |
| কাটার পরে handler কত লাইন হতো (আন্দাজ) | |

আমার handler-এর কাজ → actor → গন্তব্য:

| # | কাজ | actor | গন্তব্য |
|---|---|---|---|
| 1 | | | |
| 2 | | | |
| 3 | | | |
| 4 | | | |
| 5 | | | |
| 6 | | | |
| 7 | | | |

**কোন একটা কাজ সরালে সবচেয়ে বেশি লাভ হতো, আর কেন?**

---

## Task 8 — git দিয়ে actor-table যাচাই

```
git log --oneline --since="6 months ago" -- <path>
git shortlog -sn --since="6 months ago" -- <path>
```

| | মান |
|---|---|
| ৬ মাসে commit সংখ্যা | |
| আলাদা author সংখ্যা | |
| commit message-এ কয়টা আলাদা বিষয় দেখা যায় | |

**git আমার actor-table সমর্থন করল, না ভুল প্রমাণ করল?**

*(ভুল প্রমাণ করলে সেটাই বেশি দামি — লিখে রাখো কোথায় আন্দাজ ভুল ছিল।)*

---

## Task 9 — 🔁 উল্টো drill: যেটাকে ছুঁতে নেই

আমার সবচেয়ে ছোট handler: ______

| trigger | আছে? |
|---|---|
| (1) ঘন ঘন বদলায়? | |
| (2) আলাদা টিম থেকে PR আসে? | |
| (3) test করতে infra লাগে? | |

**তিন লাইনে যুক্তি দাও কেন এটাকে refactor করা over-engineering হতো:**

---

## Task 10–11 — Stretch

**Pipeline behaviour যেটা পড়লাম:** ______
**Behaviour না হলে কয়টা file-এ একই কোড থাকত:** ______

**কাল Day 10-এর জন্য মার্ক করা `switch`/`if` সিঁড়িটা:**

```csharp
```

---

## আজকের এক লাইনে (নিজের ভাষায়, session file না দেখে)

> ৪টা dependency থাকা handler SRP মানতে পারে, কিন্তু ২টা tax rate জানা handler
> পারে না — কেন?

**My answer:**

> Handler-এ যা থেকে যায় সেটা "শূন্যতা" না — সেটা কী?

**My answer:**

---

## Self-rating (Day 14-এ এটা আবার জিজ্ঞেস করা হবে)

- [ ] **weak** — audit চালাতে আমাকে session file দেখতে হয়েছে
- [ ] **ok** — নিজে চার ধাপ চালাতে পারি, কিন্তু কোনটা কোথায় যাবে তাতে দ্বিধা হয়
- [ ] **strong** — নিজের handler-এ চালিয়েছি, **আর** কোন handler-এ চালানো
      উচিত *না* সেটাও যুক্তি দিয়ে বলতে পারি
