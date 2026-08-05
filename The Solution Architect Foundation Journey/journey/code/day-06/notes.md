# Day 06 — my notes

Topic: **Coupling & Cohesion — একটা class কতটা "একটা জিনিস", আর অন্যদের কতটা চেনে.**

---

## Self-check (answer in my own words, don't peek at the session file)

> Cohesion আর coupling — দুইটার পার্থক্য এক লাইনে। (ইঙ্গিত: একটা ভেতরের প্রশ্ন, একটা বাইরের।)

**My answer:**

> "সব coupling খারাপ" — এই বাক্যটা কেন ভুল?

**My answer:**

---

## Task 1 — the bad `FilingService`, counted by hand

| Question | Number |
|---|---|
| `Submit()` কয়টা আলাদা কাজ করে | ______ |
| কয়টা বাইরের নাম চেনে (concrete type, string, path, static) | ______ |
| কয়জন আলাদা মানুষ এই file বদলাতে বলতে পারে | ______ |

**One line describing the class — কয়বার "আর" লাগল?** ______

সেই লাইনটা এখানে হুবহু লিখে রাখো:

```

```

---

## Task 1e — 🔴 the moment I got stuck (আজকের সবচেয়ে দামি বাক্স)

Test টা ছিল: *"BD এর filing এ tax = Amount × 0.25"* — bad version এর বিরুদ্ধে।

- **যেই লাইনে আটকে গেলাম** (হুবহু):

```

```

- চালাতে কী কী দাঁড় করাতে হতো? (SQL / SMTP / `C:\logs\` / অন্য কিছু)
- সত্যিকারের email কি চলে গেল? ______
- assert করার মতো কিছু ফেরত এল? ______

- **এক লাইনে: এই কষ্টটা testing এর দোষ, নাকি design এর? কেন?**

---

## Task 2 — after step 1 (কাঁচি only)

| | Bad version | After extracting `TaxCalculator` |
|---|---|---|
| Test এর লাইন সংখ্যা | ______ | ______ |
| দাঁড় করাতে হওয়া জিনিস | ______ | ______ |

- **কোনো business logic কি বদলেছে?** ______
- **তাহলে test টা সম্ভব হলো কীভাবে?** *(এক লাইন — এটাই আজকের পুরো পাঠ)*

---

## Task 3 — after step 2 (contracts)

- `FilingService` এর ভেতরে কয়টা লাইন বদলাতে হলো নতুন store বসাতে? ______ *(0 হওয়ার কথা)*
- **"0 লাইন" ব্যাপারটা কেন গুরুত্বপূর্ণ?**

**নির্ভরতার স্থিতিশীলতা যাচাই — "এটা কি আমার চেয়ে ধীরে বদলায়?"**

| Dependency | ধীরে বদলায়? | কেন |
|---|---|---|
| `FilingValidator` | | |
| `TaxCalculator` | | |
| `IFilingStore` | | |
| `INotifier` | | |
| ~~`SqlConnection` + connection string~~ (bad version) | | |

- **এক লাইনে সাধারণ নিয়ম:** কীসের দিকে নির্ভর করা নিরাপদ?

---

## Task 4 — over-splitting (উল্টো ভুল)

`FilingValidator` কে তিনটা এক-নিয়মের class এ ভাঙার পর, নতুন একটা নিয়ম যোগ করতে:

| Question | Number |
|---|---|
| কয়টা file ছুঁতে হলো | ______ |
| কয়টা নতুন class | ______ |
| কয়টা জায়গায় register | ______ |

- **রায়: এটা কি উন্নতি?** yes / no — এক লাইনে যুক্তি:
- **তিনটা নিয়ম কি আলাদা কারণে বদলায়, নাকি একই কারণে?** ______
- **এক লাইনে: রেখাটা কোন বরাবর টানতে হয়?**

---

## The judgment call (the architect bit)

- **আমার কোডে এমন একটা জায়গা যেখানে আমি ভাগ করেছিলাম কিন্তু করা উচিত ছিল না:**
- **এমন একটা class যেটা এখনো ভাগ করিনি কিন্তু করা উচিত** — আর কোন *কারণ*-রেখা বরাবর কাটব:
- **Coupling শূন্য করা যায় না। তাহলে আজ আমি আসলে কী করলাম?** *(এক লাইন)*
- **এই refactor টা কখন over-engineering হতো?** *(তিনটা শর্তের একটাও না ঘটলে — কোন তিনটা?)*

---

## Hunting this in the Orbitax codebase

**Hunt 1 — the biggest handler.**

- **File / handler নাম:** ______  (লাইন সংখ্যা: ______)

| ও কী কী কাজ করে | কে এটা বদলাতে বলবে |
|---|---|
| | |
| | |
| | |
| | |

- **আলাদা "বদলাতে বলা" নাম কয়টা?** ______
- দুইয়ের বেশি হলে → আমি bad example টা নিজের codebase এ পেয়েছি।
- *(Keep this — Day 9 এ এই handler টাই SRP দিয়ে কাটব.)*

**Hunt 2 — তীরচিহ্ন উল্টো দিকে যাচ্ছে কি?**

Domain project এ infra namespace খোঁজো: `SqlClient`, `MongoDB.Driver`, `HttpClient`, `SmtpClient`, `System.IO`।

- **পেলাম / পাইনি:** ______
- পেলে file নাম: ______
- **এক লাইনে: Clean Architecture এ তীরচিহ্ন সবসময় কোন দিকে, আর কেন?**

**Hunt 3 — যেগুলো ইতিমধ্যেই ঠিক করা আছে।**

- FluentValidation আজকের **কোন ধাপ** টা করে দিচ্ছে (ধাপ ১ না ধাপ ২)? ______
- একটা Pipeline behaviour যেটা handler এর cohesion বাঁচাচ্ছে: ______
- Polly কোথায় বসানো — call-site এ ছড়ানো, নাকি এক জায়গায়? ______

---

## Questions that came up while coding

-
