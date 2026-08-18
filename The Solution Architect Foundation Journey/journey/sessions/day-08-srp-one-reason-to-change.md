# Day 8 of 90 — SRP: "one reason to change" আসলে "one person to answer to"

**Block:** Month 1 (Foundation: OOD + SOLID) · Week 2 (SOLID — first three)
**Date:** 2026-08-14

আজকের পুরো পাঠ এক লাইনে:

> **SRP মানে "একটা class একটা কাজ করবে" না।
> SRP মানে — একটা class এর জবাবদিহি করার লোক একজনই থাকবে।
> "Responsible **for** a task" না, "responsible **to** a person"।**

Day 6 এ তুমি cohesion শিখেছ — *এই জিনিসগুলো কি একে অপরের?* সেটা ছিল **কোডের দিকে
তাকিয়ে** প্রশ্ন। আজ প্রশ্নটা কোড থেকে সরে **মানুষের দিকে** যাচ্ছে: *এই কোড বদলাতে
বলার ক্ষমতা কার আছে?*

পার্থক্যটা ছোট শোনায়, কিন্তু আজ তুমি এমন একটা bug দেখবে যেটা cohesion এর চোখে
ধরা পড়ে না — কারণ কোডটা দেখতে **DRY, পরিষ্কার, সুন্দর**। আর ঠিক সেই সৌন্দর্যটাই
ভুল সংখ্যা পাঠিয়ে দেয়।

---

## 1. Goal

আজকের শেষে তুমি যেকোনো class দেখে বলতে পারবে **কতজন আলাদা মানুষ এটা বদলাতে বলতে
পারে** — এবং বুঝবে কেন *দুইটা একরকম দেখতে code block সবসময় duplication না*।

---

## 2. ❌ The Bad Version

একদম ছোট রাখছি। একটা `Employee` — junior হিসেবে আমরা ঠিক এটাই লিখি, আর
এটা লিখে **গর্বও করি**, কারণ common logic টা একটাই জায়গায় আছে:

```csharp
public class Employee
{
    public int      Id         { get; set; }
    public string   Name       { get; set; }
    public decimal  HourlyRate { get; set; }
    public int[]    DailyHours { get; set; }   // সপ্তাহের ৭ দিনের ঘণ্টা

    // Finance এই সংখ্যাটা চায় — বেতন দিতে
    public decimal CalculatePay()
        => RegularHours() * HourlyRate;

    // HR এই সংখ্যাটাই চায় — timesheet report এ
    public string ReportHours()
        => $"{Name} worked {RegularHours()} hours this week.";

    // DBA/infra এইটা চায়
    public void Save()
    {
        using var db = new SqlConnection("Server=prod;...");
        db.Open();
        new SqlCommand($"UPDATE Employees SET Rate={HourlyRate} WHERE Id={Id}", db)
            .ExecuteNonQuery();
    }

    // ⭐ দুইজনেই এইটা ব্যবহার করছে — "DRY" 
    private int RegularHours()
    {
        int total = 0;
        foreach (var h in DailyHours) total += h;
        return total > 40 ? 40 : total;
    }
}
```

দেখো — `RegularHours()` একবারই লেখা। Copy-paste নেই। Code review এ কেউ কিছু
বলবে না। **এইটাই আজকের ফাঁদ।**

---

## 3. Why it is bad — জবাবদিহি কার কাছে?

class টার পাশে একটা কলাম যোগ করো: **এই method বদলাতে বলার ক্ষমতা কার?**

| Method | কে বদলাতে বলে | কেন |
|---|---|---|
| `CalculatePay()` | **Finance / CFO** | payroll policy, overtime rule, tax |
| `ReportHours()` | **HR / COO** | labour report, attendance format |
| `Save()` | **Infra / DBA** | schema, DB, connection |
| `RegularHours()` | **Finance আর HR — দুইজনেই** | 💣 |

তিনজন আলাদা মানুষ, এক class। কিন্তু আসল বিস্ফোরকটা শেষ সারিতে।

### 💣 আসল ঘটনাটা — accidental duplication

একদিন **CFO** বলল:

> "Contractor দের overtime এখন থেকে 40 না, 45 ঘণ্টা পর্যন্ত regular ধরা হবে।"

Developer ঠিক কাজটাই করল — `RegularHours()` এ `40` কে `45` করল। Build হলো।
`CalculatePay()` এর test পাশ করল। PR merge হলো। **কেউ কিছু ভাঙেনি।**

তিন সপ্তাহ পরে **HR** এর labour-ministry report এ ধরা পড়ল — প্রত্যেক কর্মীর
সাপ্তাহিক ঘণ্টা ৫ ঘণ্টা করে বেশি দেখাচ্ছে। **HR কখনো এই পরিবর্তন চায়নি। HR জানেই
না এমন কোনো পরিবর্তন হয়েছে।**

> কোনো compiler error না। কোনো failing test না। কোনো exception না।
> শুধু একটা **ভুল সংখ্যা**, একটা সরকারি report এ।

**Analogy:** এক বাড়িতে দুই ভাড়াটে, একটাই আলোর সুইচ। একজন রাতে পড়ে, আরেকজন ঘুমায়।
সুইচটা "একটাই" — সুন্দর, DRY, কম তার। কিন্তু একজনের সিদ্ধান্ত আরেকজনের উপর
জোর করে চেপে বসে। **তারা তার ভাগাভাগি করেনি — তারা নিয়ন্ত্রণ ভাগাভাগি করে ফেলেছে।**

### এইটাই SRP এর আসল সংজ্ঞা

Uncle Bob নিজেই পরে সংজ্ঞাটা শুধরে দিয়েছেন, কারণ সবাই ভুল বুঝছিল:

> **"A module should be responsible to one, and only one, actor."**
> — *responsible for one thing* না। **Actor** = যে মানুষ/দল পরিবর্তনের অনুরোধ করে।

তাই "one reason to change" এর *reason* মানে কোনো technical কারণ না —
**reason মানে একজন মানুষ।** কারণ কোড নিজে নিজে বদলায় না; কেউ বদলাতে বলে।

---

## 4. Problems it causes

1. **নীরব breakage (আজকের বড়টা)** — একজন actor এর অনুরোধে আরেকজন actor এর
   output বদলে যায়। Test ধরে না, কারণ দুই actor এর কারো test ই মিথ্যা হয়নি —
   একজনের *প্রত্যাশা* মিথ্যা হয়েছে।
2. **Merge conflict এর কারখানা** — Finance টিম আর HR টিম একই file এ PR দিচ্ছে।
3. **Review অকেজো হয়ে যায়** — "এই PR টা তো শুধু payroll এর" ভেবে কেউ HR এর
   অংশটা পড়ে না।
4. **Test করতে infra লাগে** — `Save()` ওই class এ থাকায় `Employee` কে instantiate
   করলেই SQL এর গন্ধ পাশে থাকে *(Day 6 এর রোগ, ফিরে এসেছে)*।
5. **Deployment জোড়া লেগে যায়** — HR এর ছোট report fix দিতে গেলে payroll কোডও
   একসাথে deploy হয়।

---

## 5. ✅ The Good Version — সবচেয়ে সহজ সঠিক রূপ

Interface লাগবে না। DI container লাগবে না। শুধু **actor বরাবর কাঁচি**:

```csharp
// শুধু data + যে invariant সবার জন্য সত্য (Day 1)
public class Employee
{
    public int     Id         { get; }
    public string  Name       { get; }
    public decimal HourlyRate { get; }
    public IReadOnlyList<int> DailyHours { get; }

    public Employee(int id, string name, decimal rate, int[] dailyHours)
    {
        if (rate < 0) throw new ArgumentException("rate cannot be negative");
        Id = id; Name = name; HourlyRate = rate; DailyHours = dailyHours;
    }
}

// ── Finance এর জিনিস ─────────────────────────────
public class PayCalculator
{
    private const int PayableWeeklyCap = 45;      // CFO এর নিয়ম

    public decimal CalculatePay(Employee e)
    {
        int hours = Math.Min(Sum(e.DailyHours), PayableWeeklyCap);
        return hours * e.HourlyRate;
    }

    private static int Sum(IReadOnlyList<int> hours)
    {
        int t = 0; foreach (var h in hours) t += h; return t;
    }
}

// ── HR এর জিনিস ──────────────────────────────────
public class HourReporter
{
    private const int ReportableWeeklyCap = 40;   // labour law এর নিয়ম

    public string ReportHours(Employee e)
    {
        int hours = Math.Min(Sum(e.DailyHours), ReportableWeeklyCap);
        return $"{e.Name} worked {hours} hours this week.";
    }

    private static int Sum(IReadOnlyList<int> hours)
    {
        int t = 0; foreach (var h in hours) t += h; return t;
    }
}

// ── Infra এর জিনিস ───────────────────────────────
public class EmployeeRepository
{
    public void Save(Employee e) { /* SQL এখানে */ }
}
```

---

## 6. What changed and why

তোমার চোখ এখন নিশ্চয়ই একটা জিনিসে আটকে আছে — **`Sum()` দুইবার লেখা হলো!
এটা তো DRY ভাঙছে!**

ভালো। এটাই আজকের সবচেয়ে দামি লাইন:

> **DRY এর মানে "একরকম দেখতে কোড দুইবার থাকবে না" — এটা ভুল পাঠ।
> DRY এর মানে "একই *জ্ঞান* দুই জায়গায় থাকবে না"।
> দুইটা লাইন হুবহু এক দেখালেও, যদি তারা দুইজন আলাদা মানুষের কাছে জবাবদিহি করে —
> তাহলে ওটা duplication না, ওটা **coincidence**।**

Finance এর "45" আর HR এর "40" আজ হয়তো একই সংখ্যাও হতে পারত। তাতে কিছু যায়
আসে না — **তারা একই কারণে বদলাবে না**। তাদের এক করা মানে দুইজন মানুষকে জোর করে
একটা সুইচে বেঁধে দেওয়া।

এটাকে বলে **accidental duplication** (দুর্ঘটনাজনিত মিল) — vs **real duplication**
(এক জ্ঞান, দুই জায়গায়)। প্রথমটা আলাদা রাখতে হয়, দ্বিতীয়টা এক করতে হয়।
পার্থক্য করার একটাই প্রশ্ন:

> **"একজনের অনুরোধে এটা বদলালে অন্যজনও কি সেই বদলটাই চাইত?"**
> হ্যাঁ ⇒ real duplication, এক করো। না ⇒ accidental, আলাদা রাখো।

---

## 7. Bad vs Good — পাশাপাশি

| | ❌ Bad | ✅ Good |
|---|---|---|
| কতজন actor একে বদলাতে পারে | ৩ | প্রতিটায় ১ |
| CFO নিয়ম বদলালে HR এর report | **নীরবে বদলে যায়** | অক্ষত |
| `CalculatePay` test করতে | `Employee` + SQL সহ পুরো class | `new PayCalculator()` |
| Finance + HR একসাথে কাজ করলে | একই file, conflict | আলাদা file |
| DB বদলালে payroll code | একই file এ আছে | ছোঁয়াই লাগে না |
| দেখতে | DRY, সংক্ষিপ্ত | সামান্য "repetitive" |
| আসলে | **এক সুইচ, দুই মালিক** | প্রত্যেকের নিজের সুইচ |

---

## 8. Architect's reasoning — "আরো সহজ উপায় আছে কি?"

### ভুল পাঠ ১ — "SRP মানে ছোট class"

সবচেয়ে সাধারণ junior overcorrection: SRP পড়ে এসে সব কিছু ভেঙে ফেলা।

```csharp
public class BankAccount
{
    public void Deposit(decimal amt)  { ... }
    public void Withdraw(decimal amt) { ... }
    public decimal Balance            { get; }
}
```

তিনটা method। SRP কি বলে এটাকে `Depositor`, `Withdrawer`, `BalanceReader` এ ভাঙতে?
**না।** কারণ তিনটাই **একই actor** — account holder / banking rules — এর কাছে
জবাবদিহি করে, আর তিনটাই একই invariant (balance ≥ 0) রক্ষা করে *(Day 1)*।
ভাঙলে invariant টাই ছড়িয়ে পড়বে, মানে **encapsulation ধ্বংস করে SRP মানা** —
যেটা অর্থহীন।

> **SRP class ছোট করার নিয়ম না। SRP হলো কাঁচি *কোথায়* বসাবে তার নিয়ম।
> Method গুনে সিদ্ধান্ত নিও না — actor গুনে নাও।**
> *(Day 6 এর তিনটা এক-লাইনের validator class — মনে আছে? ওটা ছিল ঠিক এই ভুলটা।)*

### ভুল পাঠ ২ — "তাহলে entity তে কোনো behaviour থাকবে না?"

এটা সত্যিকারের trade-off, এবং এর মূল্য আছে। উপরে `Employee` এখন প্রায়
data-only — DDD একে **anemic domain model** বলে সমালোচনা করবে, ঠিকই করবে।

সমাধানের রেখাটা এই:

> **Data এর সাথে behaviour রাখো — যখন behaviour টা ওই data এরই *একই actor* এর।
> Invariant (Day 1) entity র ভেতরে থাকে — "rate কখনো negative না" সবার জন্য সত্য।
> Policy (যেটা নিয়ে বিভাগ দুইটা তর্ক করে) বাইরে যায়।**

`rate < 0` চেক টা `Employee` এর ভেতরেই আছে, খেয়াল করো — ওটা কোনো বিভাগের মতামত না,
ওটা object টার অস্তিত্বের শর্ত।

### কখন এই split টা over-engineering?

যদি actor **সত্যিই একজন** হয় — একটা internal tool, যেখানে তুমিই CFO, তুমিই HR —
তাহলে তিনটা class তিনটা file, শূন্য লাভ। **Actor গোনো বাস্তবে, কল্পনায় না।**
"ভবিষ্যতে হয়তো HR আলাদা হবে" — এটা যথেষ্ট কারণ না। যেদিন দ্বিতীয় actor আসবে,
সেদিন কাঁচি চালাও; ততদিন এক class + private method সহজতর সঠিক উত্তর।

### দাম যেটা দিতে হলো

Caller কে এখন তিনটা object নিয়ে ঘোরাফেরা করতে হচ্ছে। যদি সেটা বিরক্তিকর হয়ে ওঠে,
একটা পাতলা মুখোশ দাঁড় করানো যায় (`PayrollService` — ভেতরে তিনজনকে ডাকে,
নিজে কোনো নিয়ম জানে না)। **এটাই Facade — Day 38।** কিন্তু আগে ভেঙো, পরে মুখোশ —
উল্টোটা করলে তুমি শুধু bad version টাকে নতুন নাম দিলে।

### Architect bridge

লক্ষ্য করো, আজকের class-boundary গুলো তোমার **org chart** এর মতো দেখতে —
Finance / HR / Infra। এটা কাকতালীয় না। এটাই **Conway's Law** এর ব্যবহারিক রূপ:
system এর সীমারেখা যদি প্রতিষ্ঠানের সীমারেখার সাথে না মেলে, তাহলে প্রতিটা
পরিবর্তনে দুইটা দলের অনুমতি লাগবে।

> **Microservice এর সীমা কোথায় টানব — এই প্রশ্নের উত্তরও ঠিক আজকের প্রশ্নটাই,
> শুধু class এর বদলে deployment unit এ।** এই কারণেই SRP কে "junior দের নিয়ম"
> ভাবা ভুল — এটা architecture এর প্রথম কাটার নিয়ম।

---

## 9. Real-world usage — তোমার Orbitax stack

- **MediatR handler = SRP এর একক।** এক command, এক use case, এক actor। যেদিন
  `CreateFilingCommandHandler` এ ঢুকে দেখো compliance rule *আর* audit log
  *আর* notification — তিন actor এক handler এ। *(Day 9 এ ঠিক এইটাই করব — আসল
  handler নিয়ে। Day 6 এর ৭ নম্বর hunt টা লাগবে, ফেলে দিও না।)*
- **GIR XML tooling — আজকের bug টার নিখুঁত জায়গা।** ভাবো একটাই formatter/helper
  দিয়ে OECD schema আউটপুট আর internal reconciliation report — দুইটাই বানানো হচ্ছে।
  OECD schema বদলাল ⇒ helper বদলাল ⇒ **internal report নীরবে বদলে গেল**, অথচ
  finance টিম কিছুই চায়নি। দুই master, এক সুইচ।
- **DTO ≠ Domain entity।** Domain entity সরাসরি API response হিসেবে ফেরত দিলে —
  API consumer (client team) আর domain rule (business) দুইজন আলাদা actor একটা
  class এ বসে যায়। এই কারণেই DTO টা "অতিরিক্ত কাজ" না।
- **FluentValidation** = compliance actor এর নিজস্ব file — handler এর বাইরে।
- **Pipeline behaviour** = ops actor (logging, retry) কে business actor এর
  handler থেকে দূরে রাখা। *(Day 50।)*
- **Repository** = infra actor। এই কারণেই আজকের `Save()` বেরিয়ে গেছে।

**আজকের শিকার:** repo তে এমন একটা `private` helper method খোঁজো যেটা **দুই বা
তার বেশি public method** ডাকে, আর সেই public method গুলো **আলাদা feature/দল**
serve করে। ওটাই তোমার `RegularHours()`। পেলে লিখে রাখো — ওটা আজ পর্যন্ত ফাটেনি
মানে এই না যে ফাটবে না।

---

## 10. Key takeaway

> **"One reason to change" এর reason মানে একজন **মানুষ**, একটা কাজ না।
> Class এর method গুনে সিদ্ধান্ত নিও না — কতজন মানুষ এটা বদলাতে বলতে পারে সেটা গোনো।
> আর একরকম দেখতে দুইটা কোড যদি দুইজনের কাছে জবাবদিহি করে — ওটা duplication না,
> ওটা coincidence। এক করলে তুমি দুইজন মানুষকে একটা সুইচে বেঁধে দিলে।**

---

## 11. Hands-on exercise — আজ রাতে নিজে হাতে

কাজ করো `HandsOnPractice/SingleResponsibility/` project এ (`Bad Example/` +
`Good Example/`), উত্তরগুলো `journey/code/day-08/notes.md` তে।

1. **উপরের `Employee` টা হুবহু হাতে টাইপ করো** — খারাপ রূপটাই। copy-paste না।
2. **Actor table বানাও** — প্রতিটা public method এর পাশে *কে বদলাতে বলে*। কয়টা
   আলাদা নাম পেলে?
3. **🔴 Bug টা নিজে ঘটাও (আজকের আসল কাজ):**
   - `CalculatePay()` এর জন্য একটা test লেখো, আর `ReportHours()` এর জন্য একটা।
     দুইটাই পাশ করাও।
   - এখন CFO এর অনুরোধ পালন করো — `RegularHours()` এ `40` ⇒ `45`।
   - **কয়টা test fail করল?** উত্তরটা লিখে রাখো। তারপর `ReportHours()` এর
     আউটপুটটা নিজের চোখে দেখো।
   - এক লাইনে লেখো: *"HR এর সংখ্যা বদলে গেল, কিন্তু আমার build সবুজ ছিল, কারণ ______"*
4. **Good version লেখো** — তিনটা আলাদা class, `Employee` শুধু data + invariant।
5. **আবার CFO এর অনুরোধ চালাও** (`PayCalculator` এ 45)। `HourReporter` এর একটা
   অক্ষরও ছুঁতে হলো কি? লিখে রাখো।
6. **DRY এর সাথে তর্ক করো:** `Sum()` দুইবার আছে। এক লাইনে যুক্তি দাও কেন এটা
   duplication *না*। তারপর উল্টো দিকটাও লেখো — **কোন ক্ষেত্রে এটা সত্যিই
   duplication হতো** এবং এক করা উচিত ছিল?
7. **উল্টো drill (over-correction ধরার জন্য):** `BankAccount` (Day 1 এর টা) নাও।
   SRP এর নামে ওটাকে `Depositor` / `Withdrawer` / `BalanceReader` এ ভাঙো।
   এখন প্রশ্ন: **balance ≥ 0 invariant টা কে রক্ষা করছে?** দুই লাইনে লেখো এটা
   কেন খারাপ, আর কোন নিয়মটা তোমাকে থামানো উচিত ছিল।
8. **Orbitax hunt:** সেই `private` helper — দুই আলাদা feature কে সেবা দিচ্ছে।
   File + method নাম + দুইজন actor এর নাম লিখে রাখো।

**Stretch (ঐচ্ছিক):** তোমার সবচেয়ে বড় handler এর প্রতিটা `using` দেখো। কয়টা
আলাদা বিভাগের জগৎ ওখানে ঢুকেছে? *(কালকের কাঁচামাল।)*

---

## 12. আগামীকাল

**Day 9 — SRP practice on a real Orbitax handler.** নতুন তত্ত্ব না; আজকের
actor-কাঁচি নিয়ে তোমার নিজের কোডে ঢুকব। Day 6 এর handler table আর আজকের ৮ নম্বর
hunt — দুইটাই সাথে রেখো।
