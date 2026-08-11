# Day 7 of 90 — 🔁 Retrieval Day (Week 1: OOP in depth)

**Block:** Month 1 (Foundation: OOD + SOLID) · Week 1 · **Covers Days 1–6**
**Date:** 2026-08-11

আজকে নতুন কিছু শেখানো হবে না। আজ শুধু **বের করে আনা** — retrieval।

---

## কীভাবে ব্যবহার করবে (এটা আগে পড়ো)

1. **উত্তরগুলো নিচে আছে। এখন স্ক্রল কোরো না।** পড়ে ফেলা মানেই দিনটা নষ্ট — recognition আর recall এক জিনিস না। চিনতে পারা সোজা, মনে করা কঠিন, আর কাজে লাগে শুধু দ্বিতীয়টা।
2. প্রতিটা প্রশ্নের উত্তর **লিখে ফেলো** (খাতায় বা `journey/code/day-07/answers.md` তে)। মুখে "হ্যাঁ জানি" বলাটা ফাঁকি — লিখতে গেলেই বোঝা যায় কতটা জানো।
3. আটকে গেলে **৬০ সেকেন্ড কষ্ট করো**, তারপরও না এলে `?` লিখে পরেরটায় যাও। ওই কষ্টটুকুই memory তৈরি করে; উত্তরটা সাথে সাথে দেখে ফেললে কিছুই বসে না।
4. সব শেষে উত্তর মিলিয়ে **নিজেকে rate করো**, আর PART F অনুযায়ী SKILLS_MATRIX আপডেট করো।

সময় লাগবে ~৪৫–৬০ মিনিট। **সৎ থাকাটাই আজকের একমাত্র শর্ত।**

---

# ⬇️ প্রশ্ন

## PART A — Rapid fire (এক-দুই লাইনে উত্তর)

**A1.** Encapsulation এর সংজ্ঞাটা এক লাইনে দাও — কিন্তু "private field" শব্দটা ব্যবহার না করে।

**A2.** Encapsulation আর Abstraction — কোনটা "কে ভেতরে ঢুকতে পারবে" আর কোনটা "বাইরে থেকে কী দেখা যাবে"?

**A3.** Inheritance ধার নেওয়া না দত্তক নেওয়া? কেন?

**A4.** `M × N` আর `M + N` — কোনটা inheritance, কোনটা composition, আর কীসের সংখ্যা?

**A5.** `override` আর `new` — vtable এর ভাষায় পার্থক্যটা এক বাক্যে বলো। ("hide করে" বললে ০ পাবে।)

**A6.** Cohesion কোন দিকের প্রশ্ন — ভেতরের না বাইরের? Coupling?

**A7.** SRP আর DIP — এই দুইটার মধ্যে কোনটা cohesion বাড়ায়, কোনটা coupling কমায়?

**A8.** Overload কে বেছে নেয় — compiler না CLR? Override?

**A9.** "Make illegal states unrepresentable" — এক লাইনে মানে কী?

**A10.** কোন একটা class "hard to test" — এটা কীসের অভিযোগ?

---

## PART B — Day-by-day, গভীর প্রশ্ন

### Day 1 — Encapsulation as invariant protection

**B1.** একটা class এর **সব** field private, তবুও তার encapsulation ভাঙা থাকতে পারে। **তিনটা** আলাদা উপায় বলো।

**B2.** Encapsulation design করার সময় যে তিনটা প্রশ্ন করতে বলা হয়েছিল — সেগুলো কী?

**B3.** নিচের code এ invariant `Balance >= 0`. কোন দরজাটা পাহারাহীন?

```csharp
public class BankAccount
{
    public decimal Balance { get; private set; }
    public void Withdraw(decimal amount)
    {
        if (Balance - amount < 0) throw new InvalidOperationException();
        Balance -= amount;
    }
    public void Deposit(decimal amount) => Balance += amount;
}
```

**B4.** FluentValidation আর entity invariant — পার্থক্যটা বলো। কেন **দুইটাই** লাগে?

**B5.** MongoDB driver নিয়ে যে ফাঁদটা বলা হয়েছিল — সেটা কী? এক বাক্যে মনে রাখার লাইনটা কী ছিল?

**B6.** `public List<OrderLine> Lines => _lines;` — এখানে সমস্যা কী, আর fix কী? Fix করার পরেও কোন ফাঁকটা থেকে যেতে পারে?

**B7.** কখন `{ get; set; }` একদম সঠিক উত্তর — অর্থাৎ কখন encapsulation চাপানো over-engineering?

---

### Day 2 — Abstraction vs Encapsulation

**B8.** এই interface টার **তিনটা** leak নাম ধরে বলো:

```csharp
public interface IPaymentProcessor
{
    Task<StripeChargeResponse> ChargeAsync(long amountInCents, string stripeCustomerId);
}
```

**B9.** একটা interface ভালো abstraction কিনা — "golden test" টা কী ছিল?

**B10.** Encapsulation ভাঙলে কী নষ্ট হয়? Abstraction ভাঙলে কী নষ্ট হয়? (পার্থক্যটা scale এর।)

**B11.** "ভুল abstraction, কোনো abstraction না থাকার চেয়ে খারাপ" — কেন? Duplicate code এর সাথে তুলনা করে বলো।

**B12.** কখন interface বানাবেই না? কখন বানাবেই?

**B13.** একটা repository method যদি `FilterDefinition<T>` বা `IQueryable` return করে — সেটা কীসের লক্ষণ?

---

### Day 3 — Inheritance: কখন হ্যাঁ, কখন না

**B14.** `class Stack<T> : List<T>` — এটা compile করে, test ও pass করে। তাহলে ভুলটা কোথায়? তিন সপ্তাহ পরে কে কীভাবে ভাঙে?

**B15.** "Fragile base class problem" — এক বাক্যে কী?

**B16.** `:` লেখার আগের **তিনটা** প্রশ্ন কী ছিল? একটাও "না" হলে কী করবে?

**B17.** "is-a" test এ ইংরেজি বাক্য দিয়ে যাচাই করলে ঠকবে কেন? দুইটা উদাহরণ দাও যেখানে ইংরেজি ঠিক কিন্তু code ভুল।

**B18.** আসল test টা তাহলে কী? (এক বাক্যে, আর এটার নাম Day 12 এ কী হবে?)

**B19.** *ভালো* inheritance দেখতে কেমন — চারটা লক্ষণ বলো। `TaxFiling` base টা কেন ঠিক ছিল?

---

### Day 4 — Composition over Inheritance

**B20.** ২ format × ৩ destination — inheritance এ কয়টা class? JSON যোগ হলে? S3 যোগ হলে?

**B21.** Composition এ **আজকের দিনে** file বেশি না কম? তাহলে লাভটা কোথায়? Junior আর architect এখানে কী আলাদা করে গোনে?

**B22.** যে জিনিসটা inheritance দিয়ে **লেখাই সম্ভব ছিল না** — সেটা কী ছিল, আর কেন সম্ভব ছিল না?

**B23.** Composition নেওয়ার তিনটা লক্ষণ কী? তিনটার একটাও না থাকলে কী করবে?

**B24.** Day 3 এর `TaxFiling` base টা Day 4 এর পরেও কেন ঠিক আছে? এক লাইনের নিয়মটা কী?

**B25.** আজকের composition টার দুইটা pattern-নাম আছে (Month 2 এ আসবে)। কোন দুইটা, আর কোনটা কীসের জন্য?

---

### Day 5 — Polymorphism: subtype vs ad-hoc, vtable

**B26.** এই দুই লাইন কেন দুইটা আলাদা উত্তর দেয়? **একই object** কিন্তু।

```csharp
SavingsAccount savings = new SavingsAccount(100_000m);
Account        asBase  = savings;
savings.MonthlyInterest();   // 416.67
asBase.MonthlyInterest();    //   0.00
```

**B27.** vtable ছবিটা মাথা থেকে আঁকো: `override` দিলে slot গুলো কেমন দেখায়, `new` দিলে কেমন? তারপর এক বাক্যে বলো `Account a` দিয়ে call করলে কী হয়।

**B28.** ECMA-335 এ যে দুইটা আক্ষরিক flag এর কথা বলা হয়েছিল — নাম কী? C# এর কোন keyword কোনটায় অনুবাদ হয়?

**B29.** `virtual` / `abstract` / `override` / `new` — চারটার প্রত্যেকটায় vtable এ ঠিক কী ঘটে?

**B30.** **আসল culprit** টা কে ছিল — `new` keyword টা, নাকি অন্য কিছু? উত্তরটাই Day 5 এর সবচেয়ে দামি লাইন।

**B31.** `Log(acc)` আর `acc.MonthlyInterest()` — একই object, দুইটা আলাদা নিয়ম। কে কোনটা ঠিক করে, আর **কী দেখে** ঠিক করে?

**B32.** `if (x is SavingsAccount)` — এটা কখন ভুল, কখন ঠিক? প্রশ্নটা কী জিজ্ঞেস করে ঠিক করবে?

**B33.** Performance এর অজুহাতে `virtual` এড়ানো কি ঠিক? তাহলে `virtual` এড়ানোর *ভালো* কারণটা কী?

**B34.** MediatR এর handler dispatch কি vtable দিয়ে হয়? না হলে কী দিয়ে হয়, আর তাতে failure টা কখন ধরা পড়ে?

---

### Day 6 — Coupling & Cohesion

**B35.** Low cohesion ধরার "আর" test টা কী?

**B36.** Coupling মাপার **সৎ যন্ত্র** টা কী? কেন ওটাই সবচেয়ে নির্ভরযোগ্য?

**B37.** Day 6 এর fix টা **ইচ্ছাকৃতভাবে দুই ধাপে** করা হয়েছিল। ধাপ দুইটা কী কী, আর আলাদা রাখা হয়েছিল কেন?

**B38.** Step 1 এ `FilingValidator` আর `TaxCalculator` বানানোর সময় **ইচ্ছা করে interface দেওয়া হয়নি**। কেন?

**B39.** Coupling কি শূন্য করা যায়? না গেলে আসল প্রশ্নটা কী?

**B40.** "Stability rule" টা কী? এটা Month 1 এর কোন দিনের বীজ?

**B41.** `FilingValidator` কে তিনটা এক-নিয়ম-এর class এ ভাঙলে কী রোগ হয়, আর কেন?

**B42.** একটা fat class ভাঙার খরচ **কখন justified** — তিনটা শর্তের যেকোনো একটা?

**B43.** একটা class কে কোন রেখা ধরে কাটবে — *"code টা কী করে"* ধরে, নাকি অন্য কিছু ধরে?

---

## PART C — Code judgment (প্রতিটার জন্য: কী ভুল, কোন দিনের নিয়ম, fix কী)

**C1.**
```csharp
public class Order
{
    public List<OrderLine> Lines { get; set; } = new();
    public decimal Total { get; set; }
}
```

**C2.**
```csharp
public interface IFilingRepository
{
    IQueryable<Filing> Query();
    void Save(BsonDocument doc);
}
```

**C3.**
```csharp
public class AuditLog : List<string>
{
    public void Record(string msg) => Add($"{DateTime.UtcNow:O} {msg}");
}
```

**C4.**
```csharp
public class Report
{
    public virtual string Render() => "";     // base: ধরে নিলাম কিছু render হয় না
}
public class PdfReport : Report
{
    public new string Render() => "%PDF-1.7...";
}
```

**C5.**
```csharp
public class OrderService
{
    public void Place(Order order)
    {
        // 1. validate
        // 2. calculate tax
        // 3. new SqlConnection("Server=prod;...").Execute(...)
        // 4. new SmtpClient("smtp.orbitax.com").Send(...)
        // 5. File.AppendAllText(@"C:\logs\orders.txt", ...)
    }
}
```

**C6.** *(ফাঁদ — সব প্রশ্নের উত্তর "refactor করো" না)*
```csharp
public record CreateFilingRequest
{
    public string JurisdictionCode { get; set; } = "";
    public decimal Amount { get; set; }
    public DateTime PeriodEnd { get; set; }
}
```

---

## PART D — Architect judgment (এগুলোর "সঠিক" উত্তর একটা না — যুক্তিটা দেখো)

**D1.** এক junior বলল: *"সব class এর জন্য interface বানিয়ে ফেলি, তাহলে future-proof থাকবে।"* Week 1 এর কোন কোন দিন থেকে যুক্তি টেনে ওকে থামাবে?

**D2.** আরেকজন বলল: *"Composition সবসময় inheritance এর চেয়ে ভালো।"* এই বাক্যটা কোথায় ভুল? একটা counter-example দাও Week 1 থেকেই।

**D3.** তোমার কাছে একটা ৪০০-লাইনের handler আছে। ভাঙার আগে **কোন প্রশ্নটা** করবে, আর কোন উত্তর পেলে ভাঙবে **না**?

**D4.** এই তিনটা কি একই রোগের তিনটা নাম, নাকি আলাদা রোগ — (i) Stack : List, (ii) leaky `IPaymentProcessor`, (iii) `public List<T> Lines`? প্রত্যেকটায় ঠিক কোন জিনিসটা ফাঁস হচ্ছে?

**D5.** Week 1 এর ছয়টা দিনকে **একটা** বাক্যে বাঁধো। (ইঙ্গিত: Day 6 এ বলা হয়েছিল বাকি Month 1 টা কীসের উপর commentary।)

---

## PART E — লেখার কাজ (এটা বাদ দিও না)

**E1.** কোনো reference না দেখে, **নিজের হাতে** লেখো:
- একটা `BankAccount` — প্রত্যেকটা দরজায় guard, invariant টা comment এ এক লাইনে।
- একটা `Stack<T>` — composition দিয়ে, ৪টার বেশি public method না।
- একটা `abstract Account` + দুইটা subclass — `MonthlyInterest()` **abstract**, `virtual` না।

**E2.** তোমার Orbitax codebase এ Week 1 এর হান্টগুলো এখনো বাকি থাকলে আজ শেষ করো:
- `grep "public new "` — কী পেলে?
- Domain project এ infra namespace আছে কি? (তীর উল্টো দিকে দেখাচ্ছে?)
- সবচেয়ে বড় handler টার jobs vs "কে এই পরিবর্তনটা চায়" — table টা বানাও। **Day 9 এ এটাই লাগবে।**

---

## PART F — নিজেকে rate করো (উত্তর মেলানোর পরে)

| স্কোর | মানে | Matrix rating |
|---|---|---|
| প্রশ্নের উত্তর দেখতে হয়েছে | দেখে চিনলাম, নিজে বের করতে পারলাম না | **weak** |
| বলতে পারলাম, কিন্তু "কখন না" বলতে পারিনি | অর্ধেক | **ok** |
| বললাম, code করলাম, **আর কখন এটা over-engineering সেটাও বললাম** | পুরোটা | **strong** |

**নিয়ম:** যেকোনো দিন যেটা `weak` এ নামল — সেটার session file টা আগামীকাল লেসনের **আগে** ১০ মিনিট আবার পড়ো। Day 14 এ ওটা আবার জিজ্ঞেস করা হবে।

---
---

# ⬇️⬇️⬇️ উত্তর — নিজে চেষ্টা করার পরে পড়ো ⬇️⬇️⬇️

---
---

## PART A — উত্তর

**A1.** এমনভাবে design করা যাতে object টা কোনোদিন invalid state এ পৌঁছাতেই না পারে — অর্থাৎ state বদলানোর প্রত্যেকটা পথে rule (invariant) টা জোর করে চাপানো। (Field লুকানোটা উপায়, উদ্দেশ্য না।)

**A2.** Encapsulation = কে ভেতরে ঢুকতে পারবে। Abstraction = বাইরে থেকে কী দেখা যাবে।

**A3.** দত্তক। ধার নিলে শুধু যন্ত্রটা পাও (composition); দত্তক নিলে পদবি + সম্পত্তি + **দেনা** + **ভবিষ্যতে বাবা যা ধার করবেন তাও** পাও। শেষটাই fragile base class problem.

**A4.** Class সংখ্যার বৃদ্ধির হার। Inheritance = `M × N` (গুণ), composition = `M + N` (যোগ)। M = format, N = destination টাইপের স্বাধীন axis.

**A5.** `override` base এর **ঐ একই slot** দখল করে (ECMA-335: `reuseslot`); `new` **পাশে নতুন একটা slot** বানায় আর base এর slot টা অক্ষত রেখে দেয় (`newslot`)।

**A6.** Cohesion = ভেতরের প্রশ্ন ("এই জিনিসগুলো কি একে অপরের?")। Coupling = বাইরের প্রশ্ন ("আমি ভাঙলে কে ভাঙে?")।

**A7.** SRP cohesion বাড়ায়। DIP coupling কমায়। (তাই বাকি Month 1 টা Day 6 এর commentary।)

**A8.** Overload → **compiler**, compile time এ, reference এর *ঘোষিত* type দেখে। Override → **CLR**, run time এ, object এর *আসল* type দেখে।

**A9.** ভুল state টাকে runtime এ throw করে ঠেকানো না — টাইপ সিস্টেমে ওটাকে **প্রকাশই করতে না দেওয়া** (যেমন `Money.Positive(...)` — negative deposit লেখাই যায় না)।

**A10.** ওটা testing এর অভিযোগ না, **design এর diagnosis**। Test করতে যা যা দাঁড় করাতে হয়, সেটাই ঐ class এর coupling।

---

## PART B — উত্তর

### Day 1

**B1.** (i) **Unguarded setter** — `public decimal Balance { get; set; }`, tunnel আছে কিন্তু gate নেই। (ii) **Unguarded constructor** — object জন্মেই invalid। (iii) **ফাঁস হওয়া mutable reference** — `List<T>` বাইরে দিয়ে দেওয়া, caller `.Clear()` / `.Add()` করে rule বাইপাস করে।

**B2.**
1. এই object এর invariant কী? (এক লাইনে লিখতে না পারলে তুমি জানো না।)
2. State বদলানোর কতগুলো পথ আছে? (constructor + প্রত্যেক public method + প্রত্যেক setter + বাইরে দেওয়া mutable object)
3. প্রত্যেকটা পথে guard আছে? **একটা বাদ পড়লেই encapsulation শেষ।**

**B3.** `Deposit`. `Deposit(-5000)` withdraw এর guard কে পাশ কাটিয়ে balance negative করে দেয়। শিক্ষা: **একটা invariant এক জায়গায় guard করলে হয় না — প্রত্যেকটা দরজায় লাগবে।**

**B4.** FluentValidation = request/command এর **edge guard** — বাইরের নোংরা input তাড়াতাড়ি reject, ভালো error message, 400 response। Entity invariant = **সত্য** — validator বাইপাস করে (background job, migration, অন্য handler) কেউ entity বানালেও rule টা টিকে থাকে। Validator = ভদ্রতা, entity guard = রক্ষাকবচ। দুইটাই রাখো।

**B5.** Mongo C# driver deserialization এ প্রায়ই private setter/field এ **সরাসরি লেখে, constructor validation চালায় না** — তাই DB তে আগে থেকে বসে থাকা corrupt document তোমার guard এর ভেতর দিয়ে না গিয়েও invalid entity বানিয়ে ফেলে। লাইনটা: *"Private setter prevents new code from corrupting the entity; it does not guarantee that persisted data was never already corrupt."*

**B6.** Caller `.Add()` / `.Clear()` করে তোমার total/discount/limit rule বাইপাস করতে পারে। Fix: `IReadOnlyList<OrderLine> Lines => _lines;` + একটাই guarded দরজা `AddLine(...)`. বাকি ফাঁক: **element গুলো নিজেরা mutable হলে** তাদের ভেতরের state এখনো বদলানো যায় — গভীর সুরক্ষা চাইলে element ও immutable করতে হবে।

**B7.** সত্যিকারের DTO / API contract / config object — কোনো invariant নেই, শুধু ডেটা বইছে। Serialization boundary যেখানে framework কে লিখতে দিতেই হবে। আর প্রত্যেক primitive এর জন্য value object বানানো — ৪০টা wrapper type মানে design না, ceremony। নিয়ম: **encapsulation এর দাম দাও invariant এর ওজন অনুযায়ী।**

### Day 2

**B8.** (i) `StripeChargeResponse` — Stripe এর **type**. (ii) `amountInCents` — Stripe এর **unit**. (iii) `stripeCustomerId` — Stripe এর **id**. Adyen এটা implement করতেই পারবে না।

**B9.** *"Interface টা পড়ে কি বলা যায় কোন library ব্যবহার হচ্ছে? বলা গেলে — ওটা abstraction না, ওটা wrapper."*

**B10.** Encapsulation ভাঙলে **একটা object** নষ্ট হয়। Abstraction ভাঙলে **সবাইকে ইঞ্জিন বুঝতে হয়** — খরচটা পুরো codebase জুড়ে ছড়ায়। তাই abstraction leak বেশি দামি।

**B11.** Duplicate code পরে সহজে ঠিক করা যায় — সব copy এক জায়গায় এনে ফেলো, শেষ। কিন্তু ২০০ জায়গায় বসে যাওয়া একটা **ভুল interface** সরানো নরক, কারণ প্রত্যেক caller ঐ ভুল ধারণাটার উপর দাঁড়িয়ে গেছে। তাই: **দুইটা implementation চোখে না দেখা পর্যন্ত abstract কোরো না।**

**B12.** লাগবে না: একটাই implementation, দ্বিতীয়টার সম্ভাবনা নেই (`IUserMapper` → `UserMapper`), বা শুধু mock করার জন্য বানাচ্ছ। লাগবেই: একাধিক implementation **আজই** আছে, বা বাইরের কিছুর সাথে কথা বলছ — payment, network, filing authority।

**B13.** Mongo leak করছে — caller এখন database এর কথা জানে। নামটা `IFilingRepository` কিন্তু ড্যাশবোর্ডে "Mongo" লেখা। (একই রোগ, শুধু Stripe এর বদলে Mongo।)

### Day 3

**B14.** `Stack` এর একটাই নিয়ম — LIFO। কিন্তু `List<T>` থেকে inherit করায় public surface এ `Insert`, `RemoveAt`, `Reverse`, indexer setter সব চলে এসেছে — যেগুলো তুমি লেখোওনি। `s.Insert(0, "x")` LIFO মেরে ফেলে আর **compiler টুঁ শব্দও করে না**। Day 1 এর সব নিয়ম মেনেও invariant গেল, কারণ **base class সবার জন্য দরজা খোলা রেখে দিয়েছে**।

**B15.** Base class কাল একটা নতুন method যোগ করলে তোমার subclass সেটা পেয়ে যাবে — **তুমি না জেনেই**, আর সেটা তোমার invariant ভাঙতে পারে।

**B16.**
1. Base এর **প্রত্যেকটা** public method কি subclass এ অর্থপূর্ণ?
2. Base কাল নতুন method যোগ করলে আমি কি নিশ্চিন্ত?
3. আমি কি base টার মালিক, নাকি ওটা অন্য কারো/library র?

একটাও "না" হলে → **composition**।

**B17.** কারণ inheritance টা vocabulary এর ব্যাপার না, **আচরণের** ব্যাপার। "Square **is a** Rectangle" — ইংরেজিতে ১০০% সত্যি, code এ ভয়ংকর (Day 12)। "Stack **is a** List" — শুনতে খারাপ লাগে না, উপরে দেখলে কী হয়।

**B18.** *"Base যেখানে যেখানে ব্যবহার হয়, সেখানে subclass টা বসিয়ে দিলে caller কি কিছুই টের পাবে না?"* — এটাই **substitutability**, আর Day 12 এ এর নাম **LSP**।

**B19.** ভালো inheritance: (i) base **abstract**, (ii) base এ প্রায় কোনো state নেই → ভাঙার কিছু নেই, (iii) public surface ছোট, (iv) **এক স্তর গভীর** (দুই স্তরের বেশি হলেই থামো)। `TaxFiling` ঠিক ছিল কারণ ওর কাজ code বিলি করা না — **একটা নিয়ম জোর করে চাপানো**: validate না হলে submit নেই। (এটার নাম Template Method, Day 49।)

### Day 4

**B20.** ২ × ৩ = **৬** class (+ ১ abstract base = ৭ file)। JSON যোগ হলে ৩ × ৩ = **৯**। S3 যোগ হলে ৩ × ৪ = **১২**। আর CSV লেখার code তিন জায়গায় copy, FTP এর code ও তিন জায়গায়।

**B21.** **বেশি** — ৮ vs ৭, আজকের হিসাবে composition এ file বেশি। লাভটা আজকের সংখ্যায় না, **কালকের ঢালে**: JSON = +১ class (৩টা combination free), S3 = +১ class, CSV rule বদল = ১ জায়গায় edit। **Junior আজকের line count গোনে, architect growth rate দেখে।**

**B22.** `exporter.UseDestination(new DiskDestination())` — FTP fail করলে চলতি অবস্থায় disk এ fallback। Inheritance এ অসম্ভব কারণ destination টা object এর কোনো *অংশ* না, ওটা **type এর ভেতরে গাঁথা**, আর type runtime এ বদলায় না। **Inheritance দিয়ে যা জোড়া লাগাও, compile time এ ঝালাই হয়ে যায়।**

**B23.** (i) একের বেশি জিনিস স্বাধীনভাবে বদলায় (class explosion আসছে), (ii) runtime এ behaviour বদলাতে হতে পারে, (iii) base এর অনেক member subclass ব্যবহার করে না (ওটা toolbox, parent না)। **তিনটার একটাও না হলে inheritance রেখে দাও** — কম code, কম indirection।

**B24.** কারণ ওর কাজ behaviour বিলি করা না, নিয়ম চাপানো। নিয়ম: **নিয়ম চাপাতে হলে inheritance। Behaviour বদলাতে হলে composition।**

**B25.** যা বদলায় সেটাকে object বানিয়ে ভেতরে রাখা = **Strategy** (Day 45)। দুইটা স্বাধীন axis কে আলাদা করা = **Bridge** (Day 41)।

### Day 5

**B26.** কারণ `MonthlyInterest()` `new` দিয়ে লেখা, `override` দিয়ে না। `new` base এর slot দখল করেনি — পাশে নতুন slot বানিয়েছে। `Account` type এর reference দিয়ে call করলে compiler **base এর slot নম্বর** চায়, আর সেখানে এখনো base এর `return 0m` বসে আছে। পার্থক্যটা object এর না, **reference এর ঘোষিত type এর**। আর সবচেয়ে খারাপ দিক: **কিছু crash করে না** — শুধু টাকার অঙ্ক ভুল, ধরা পড়ে গ্রাহকের অভিযোগে।

**B27.**
```
override:                          new:
SavingsAccount table               SavingsAccount table
slot0 → Savings.MonthlyInterest    slot0 → Account.MonthlyInterest  ← অপরিবর্তিত
                                   slot1 → Savings.MonthlyInterest  ← নতুন ঘর
```
`Account a` দিয়ে call = "slot 0 এ যা আছে চালাও"। override এ slot 0 এ নতুনটা → ঠিক উত্তর। `new` এ slot 0 এ পুরোনোটা → ভুল উত্তর; slot 1 এর কথা compiler জানেই না, কারণ `Account` type এ slot 1 বলে কিছু নেই।

**B28.** `reuseslot` (base এর slot টাই ব্যবহার করো) আর `newslot` (সবসময় নতুন slot নাও)। `override` → `reuseslot`, `new` → `newslot`।

**B29.** `virtual` = slot টা "দখলযোগ্য" চিহ্নিত হয় ("চাইলে বদলাতে পারো")। `abstract` = slot আছে, ভেতরে কিছু নেই ("তোমাকে বদলাতেই হবে")। `override` = ঐ slot এ নিজের method বসল (দখল)। `new` = নতুন slot, পুরোনোটা অক্ষত (দখল **না**)।

**B30.** `new` না — **base এর ভুয়া default**, `return 0m`. কোনো sensible default না থাকলে method টা `virtual` হওয়াই উচিত ছিল না, `abstract` হওয়া উচিত ছিল — তাহলে **compiler তোমাকে থামাত, গ্রাহক থামাত না**। (আর `abstract` এর দাম: প্রতিটা subclass এ লিখতেই হবে, এমনকি যেখানে ০ ই উত্তর — আর ওটাই feature, কারণ *"ভাবিনি"* আর *"ভেবে ০ দিয়েছি"* দুইটা আলাদা জিনিস।)

**B31.** `Log(acc)` → **compiler** ঠিক করে, **ঘোষিত** type (`Account`) দেখে, সিদ্ধান্ত binary তে পাকাপাকি লেখা হয়ে যায়। `acc.MonthlyInterest()` → **CLR** ঠিক করে, object এর **আসল** type দেখে, method table lookup দিয়ে। মনে রাখার বাক্য: **Overload দেখে তুমি কী *বলেছ*; override দেখে জিনিসটা কী *আছে*।**

**B32.** প্রশ্নটা: **আচরণটা কার ব্যবসা?** যদি behaviour টা *type এর নিজের* নিয়ম হয় (interest rate, minimum balance) → polymorphism, `if is` ভুল। যদি behaviour টা *consumer এর* ব্যবসা হয় (যেমন `RenderHtml` — domain type এর ভেতরে HTML ঢোকানো আরও খারাপ) → বাইরে switch করাই ঠিক। (এটা Visitor এর বীজ, Day 53।)

**B33.** না। Virtual dispatch এর খরচ একটা extra indirection — nanosecond, আর `sealed` হলে JIT প্রায়ই ওটাও সরিয়ে দেয় (devirtualization)। **ভালো কারণটা design এর:** প্রতিটা `virtual` একটা **চিরস্থায়ী public প্রতিশ্রুতি** — কেউ override করে ফেললে তুমি আর base এর আচরণ বদলাতে পারবে না। তাই **sealed by default**।

**B34.** না — MediatR এর dispatch হলো **container এ একটা dictionary lookup** (request type → handler type), vtable না। মানে হাতটা startup এ configure করা যায়, কিন্তু handler register না থাকলে ভুলটা **compile time এ না, runtime এ** ধরা পড়ে। (Days 47–48 এর বীজ।)

### Day 6

**B35.** Class টা কী করে সেটা এক বাক্যে বর্ণনা করতে গিয়ে যদি **"আর"** শব্দটা লাগে — cohesion ইতিমধ্যেই ফাঁস হয়েছে। ("Filing validate করে **আর** tax হিসাব করে **আর** save করে **আর** email পাঠায়…")

**B36.** **Unit test.** Class টা test করতে যা যা দাঁড় করাতে হয়, সেটাই আক্ষরিক অর্থে তার coupling — `0.25m` টা যাচাই করতে live SQL Server + SMTP + `C:\logs\` write permission লাগলে সেটাই মাপ। তাই **"এটা test করা কঠিন" কখনোই testing এর অভিযোগ না, ওটা design এর diagnosis।**

**B37.** ধাপ ১ = **শুধু কাঁচি** — `FilingValidator`, `TaxCalculator` কেটে আলাদা করা (cohesion ঠিক করা)। ধাপ ২ = **শুধু contract** — `IFilingStore`, `INotifier` (coupling ঠিক করা)। আলাদা রাখা হয়েছিল যাতে **দুইটা কারণ ঘেঁটে না যায়** — কোন সমস্যাটা কোন ওষুধে সারল সেটা পরিষ্কার থাকে।

**B38.** কারণ ওদের কোনো **বিকল্প implementation নেই**। Interface তখনই দাও যখন সত্যিকারের alternative আছে (DB, SMTP), *"abstraction ভালো জিনিস"* বলে না। (Day 2 এর একই নিয়ম।)

**B39.** না — **শূন্য coupling মানে শূন্য কাজ**। প্রশ্নটা কখনোই *"coupling আছে কি না"* না, প্রশ্নটা **"কীসের সাথে coupling"**.

**B40.** **নিচের দিকে depend করো — যা তোমার চেয়ে ধীরে বদলায় তার উপর।** এটা **DIP (Day 17)** এর বীজ।

**B41.** **Low cohesion, শুধু অন্য পোশাকে।** তিনটা validator ই একই কারণে বদলায় — একই compliance rule change — তাই ওরা আলাদা class হওয়ার যোগ্য না। (অতি-ভাঙাটা অতি-জোড়ার মতোই একটা রোগ।)

**B42.** (i) দ্বিতীয় একটা implementation **সত্যিই** দরকার, (ii) অংশগুলো **আলাদা ঘড়িতে** বদলায়, (iii) test করতে **infra** লাগে। একটাও না থাকলে — private method সহ একটা class ই সহজ সঠিক উত্তর।

**B43.** ***"কে এই পরিবর্তনটা চায়"*** — change reason ধরে কাটো, *"code টা কী করে"* ধরে না। যা একসাথে **পড়া** হয় তা একসাথে **বদলায়** না — এই দুইটা গুলিয়ে ফেলাই সবচেয়ে common ভুল।

---

## PART C — উত্তর

**C1.** Day 1। `Lines` এ public setter **আর** mutable `List<T>` ফাঁস — caller line যোগ/মুছে `Total` কে মিথ্যা বানিয়ে দিতে পারে; `Total` নিজেও public setter, মানে ওটা derived value হয়েও পাহারাহীন। Fix: `IReadOnlyList<OrderLine> Lines`, guarded `AddLine`, আর `Total` কে computed property (`_lines.Sum(...)`) করা।

**C2.** Day 2। Abstraction leak — `IQueryable` আর `BsonDocument` দুইটাই database এর শব্দ; interface পড়েই বলে দেওয়া যায় Mongo। Fix: domain এর ভাষায় intent-বাহী method — `Filing? FindById(FilingId id)`, `void Save(Filing filing)`.

**C3.** Day 3। `List<string>` থেকে inherit করায় audit log এ `Clear()`, `RemoveAt()`, indexer setter পাওয়া যাচ্ছে — **audit log এর একমাত্র নিয়মই ছিল append-only**, সেটা মরে গেল। (Day 3 এর প্রশ্ন ১: base এর প্রত্যেকটা method কি অর্থপূর্ণ? না।) Fix: composition — ভেতরে `private readonly List<string> _entries`, বাইরে `Record(...)` + `IReadOnlyList<string> Entries`.

**C4.** Day 5। দুইটা ভুল, আর **দ্বিতীয়টাই আসল**: (i) `new` লেখায় `Report r = new PdfReport(); r.Render()` নীরবে `""` দেবে — fix হলো `override`. (ii) কিন্তু আসল culprit base এর ভুয়া default `=> ""` — কোনো sensible default নেই, তাই `Render()` **`abstract`** হওয়া উচিত ছিল, তাহলে `new` লেখার সুযোগই থাকত না।

**C5.** Day 6। এক class এ ৬টা job (validate / calculate / persist / notify / log) — **"আর" test** এই বাক্যেই ফেল করেছে; আর `new SqlConnection`, `new SmtpClient`, `File.AppendAllText` — তিনটা concrete infra hard-wired, মানে tax হিসাবটা test করতে live DB + SMTP + disk লাগবে। Fix দুই ধাপে: ধাপ ১ কাঁচি (`OrderValidator`, `TaxCalculator` — interface ছাড়া), ধাপ ২ contract (`IOrderStore`, `INotifier`)।

**C6.** **কিছু ভুল নেই।** এটা একটা DTO / request contract — কোনো invariant নেই, শুধু ডেটা বইছে, আর serialization boundary তে framework কে লিখতেই দিতে হবে। `{ get; set; }` এখানেই সঠিক। এটাকে rich domain object বানানো Day 1 এর "কখন over-engineering" ঘরের উদাহরণ। *(Validation টা এর জায়গায় — FluentValidation edge guard; আসল invariant থাকবে `Filing` entity তে।)*

---

## PART D — উত্তর (যুক্তির কাঠামো, মুখস্থ বাক্য না)

**D1.** Day 2: **ভুল abstraction, কোনো abstraction না থাকার চেয়ে খারাপ** — duplicate পরে সরানো যায়, ২০০ জায়গায় বসা ভুল interface যায় না; তাই দুইটা implementation না দেখা পর্যন্ত abstract কোরো না। Day 6: interface তখনই যখন **সত্যিকারের বিকল্প আছে** (DB, SMTP), "abstraction ভালো" বলে না। Day 4: composition এর দামটাও সৎভাবে গোনা হয়েছিল — extra interface পড়ার খরচ আছে। আর "future-proof" যুক্তিটার আসল দুর্বলতা: তুমি আজ **কল্পনা করা** ভবিষ্যতের জন্য দাম দিচ্ছ, আর প্রায় সবসময় ভুল জায়গায় seam কাটছ।

**D2.** বাক্যটা একটা heuristic কে আইন বানিয়ে ফেলেছে। Counter-example: Day 3 এর `TaxFiling` abstract base — validate ছাড়া submit করা **যাবে না**, এই নিয়মটা composition দিয়ে *চাপানো* যায় না, কারণ caller চাইলে যন্ত্রটা ব্যবহারই না করতে পারে। এক লাইনে: **নিয়ম চাপাতে inheritance, behaviour বদলাতে composition।** আর Day 4 এর প্রথম version (শুধু `CsvExporter`/`XmlExporter`) — destination কখনো না বদলালে ঐ দুই subclass ই সঠিক উত্তর; সেখানে interface যোগ করা over-engineering।

**D3.** প্রশ্ন: **"এই ৪০০ লাইনের কতগুলো আলাদা কারণে বদলায়, আর কে সেই পরিবর্তনগুলো চায়?"** — jobs vs change-driver table বানাও। ভাঙবে না যদি: সব লাইন **একই কারণে, একই লোকের অনুরোধে** বদলায়, দ্বিতীয় implementation দরকার নেই, আর test করতে infra লাগে না। তখন private method সহ একটা লম্বা class-ই সহজ সঠিক উত্তর — ভাঙলে সেটা Day 6 এর সেই তিন-validator রোগ (low cohesion, অন্য পোশাকে)।

**D4.** একই পরিবারের, কিন্তু ফাঁস হচ্ছে তিনটা আলাদা জিনিস:
- **Stack : List** → *অবাঞ্ছিত public surface* ফাঁস (base থেকে উত্তরাধিকারসূত্রে পাওয়া method গুলো) → invariant মরে।
- **leaky `IPaymentProcessor`** → *implementation detail* ফাঁস (Stripe এর type/unit/id) → পুরো codebase কে ইঞ্জিন বুঝতে হয়।
- **`public List<T> Lines`** → *ভেতরের mutable state* ফাঁস → guard বাইপাস।

মিলটা এক বাক্যে: **তিনটাতেই object টা যতটুকু দেখানোর কথা তার চেয়ে বেশি দেখিয়ে ফেলেছে** — আর প্রতিবার দামটা দিয়েছে *caller*, লেখক না।

**D5.** নমুনা উত্তর: **Week 1 আসলে একটাই প্রশ্নের ছয়টা রূপ — "কে কার উপর ক্ষমতা রাখে?"** Day 1–2 এ প্রশ্নটা এক class এর ভেতরে (কে state ছোঁবে, কে কী দেখবে); Day 3–5 এ দুইটা type এর মধ্যে (base কতটা ক্ষমতা দিয়ে দিচ্ছে, কে কোন version চালাচ্ছে); Day 6 এ পুরো system জুড়ে (কে ভাঙলে কে ভাঙে)। আর Day 6 এর framing টাই বাকি Month 1 এর মানচিত্র: **SRP মানে cohesion বাড়াও, DIP মানে coupling কমাও — বাকি সব ওটার commentary।**

---

## আগামীকাল (Day 8)

**SRP — "one reason to change"**, আর একটা fat class ভাঙা। আজকের B35–B43 আর D3 সরাসরি ওখানে কাজে লাগবে — বিশেষ করে **change-driver table** টা। E2 এর handler hunt টা করা থাকলে Day 8–9 অনেক সহজ হয়ে যাবে।

---

*Day 7 of 90 · টার্গেট: "এখন আমার বেসিক শক্তিশালী।"*
