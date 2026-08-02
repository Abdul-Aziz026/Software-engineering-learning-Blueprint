# Day 5 of 90 — Polymorphism: subtype vs ad-hoc, আর override এর পেছনের vtable

**Block:** Month 1 (Foundation: OOD + SOLID) · Week 1 (OOP in depth)
**Date:** 2026-08-02

আজকের পুরো পাঠ এক লাইনে:

> **Override এ সিদ্ধান্তটা নেয় CLR, object টা আসলে কী তা দেখে।
> Overload এ সিদ্ধান্তটা নেয় compiler, reference টা কী বলে ঘোষণা করা আছে তা দেখে।
> দুইটাকেই "polymorphism" বলে — আর এই দুইটা গুলিয়ে ফেলাই বেশিরভাগ রহস্যময় bug এর উৎস।**

Day 4 এ তুমি লিখেছিলে `_formatter.Format(report)` — জানোই না ভেতরে CSV না XML,
তবুও ঠিক জিনিসটা চলেছে। **আজ প্রশ্ন: ওটা চলল কেন?**
আর তার চেয়ে জরুরি — **কখন ওটা নীরবে চলা বন্ধ করে দেয়।**

---

## 1. The problem first

Day 1 এর `BankAccount` এ ফিরি। এখন account এর ধরন এসেছে, আর মাসিক interest দিতে হবে।
Junior হিসেবে সবচেয়ে স্বাভাবিক লেখা:

```csharp
public class Account
{
    public decimal Balance { get; protected set; }
    public Account(decimal opening) => Balance = opening;

    public decimal MonthlyInterest()
    {
        return 0m;                          // base: ধরে নিলাম কোনো interest নেই
    }
}

public class SavingsAccount : Account
{
    public SavingsAccount(decimal opening) : base(opening) { }

    // compiler একটা warning দিয়েছিল: "hides inherited member".
    // আমি 'new' লিখে warning টা চুপ করিয়ে দিয়েছি। ✅ build clean!
    public new decimal MonthlyInterest()
    {
        return Balance * 0.05m / 12;
    }
}
```

Build clean। Unit test ও লিখলাম:

```csharp
var savings = new SavingsAccount(100_000m);
Assert.Equal(416.67m, Math.Round(savings.MonthlyInterest(), 2));   // ✅ PASS
```

Test সবুজ। Deploy। **মাস শেষে interest run হলো — সব savings account এ ০ টাকা।**

### ধাক্কা ১ — একই object, দুইটা উত্তর

Month-end job টা এরকম:

```csharp
List<Account> accounts = LoadFromMongo();      // list এর type: Account

decimal total = 0m;
foreach (Account a in accounts)
    total += a.MonthlyInterest();              // 😱 প্রতিবার 0
```

সবচেয়ে অস্বস্তিকর demo টা দুই লাইনে:

```csharp
SavingsAccount savings = new SavingsAccount(100_000m);
Account        asBase  = savings;              // ⚠️ নতুন object না — হুবহু একই object

Console.WriteLine(savings.MonthlyInterest());  // 416.67
Console.WriteLine(asBase.MonthlyInterest());   //   0.00
```

**একই object। একই method এর নাম। দুইটা আলাদা উত্তর।**
পার্থক্য শুধু — তুমি কোন *ধরনের variable* দিয়ে ধরেছ।

আর সবচেয়ে খারাপ দিকটা: **কিছুই crash করেনি।** কোনো exception নেই, কোনো red log নেই।
শুধু টাকার অঙ্কটা ভুল। এই bug টা production এ ধরা পড়ে **গ্রাহকের অভিযোগে**, আপনার alert এ না।

> Crash হওয়া bug সস্তা। **নীরবে ভুল উত্তর দেওয়া bug দামি।**

### ধাক্কা ২ — "ঠিক আছে, আমি নিজেই ঠিক করে দিচ্ছি"

Junior এর দ্বিতীয় সহজাত সমাধান — inheritance কে বিশ্বাস না করে নিজে হাতে দিক ঠিক করা:

```csharp
public class InterestService
{
    public decimal Calculate(Account a)
    {
        if (a is SavingsAccount)  return a.Balance * 0.05m / 12;
        if (a is FixedDeposit)    return a.Balance * 0.08m / 12;
        if (a is CurrentAccount)  return 0m;
        return 0m;
    }
}
```

এটা **কাজ করে**। আজ। সমস্যা হলো এই `if` chain টা কখনো একা থাকে না:

```csharp
MonthlyFee(Account a)        { if (a is SavingsAccount) ... }   // switch #2
StatementLabel(Account a)    { if (a is SavingsAccount) ... }   // switch #3
MinimumBalance(Account a)    { if (a is SavingsAccount) ... }   // switch #4
```

এখন `StudentAccount` যোগ করো। **Compiler একটা শব্দও বলবে না।**
চারটা switch ই নিঃশব্দে শেষ লাইনে গড়িয়ে গিয়ে `return 0m` করে দেবে।

> নতুন type যোগ করলে যদি **compiler তোমাকে না থামায়**, তাহলে তোমার design
> তোমাকে ভুল করার অনুমতি দিয়ে রেখেছে।

---

## 2. The idea — analogy

**অফিসের নাম-ফলক (nameplate) ভাবো।**

তুমি ডেস্কে গিয়ে বললে: **"রিপোর্টটা জমা দাও।"**
তুমি বলো না — "যদি এ করিম হয় তাহলে email করো, যদি রহিম হয় তাহলে print করো।"
তুমি শুধু **হুকুমটা দাও**; কে কীভাবে করবে সেটা তার নিজের জানা।

এটাই polymorphism। আর ধাক্কা ২ এর `if` chain টা হলো তুমি নিজে ডেস্কের পাশে দাঁড়িয়ে
প্রত্যেকের কাজ প্রত্যেকের হয়ে ঠিক করে দিচ্ছ। **নতুন লোক ঢুকলেই তোমার মাথা update করতে হয়।**

এবার ধাক্কা ১ টা একই analogy তে:

- করিম নিজের নিয়ম শিখেছে। কিন্তু **সে দরজার নাম-ফলকটা বদলায়নি** —
  পুরোনো ফলকটা ("কর্মচারী: default নিয়ম") ঝুলিয়ে রেখে **পাশে নিজের আরেকটা ফলক** লাগিয়েছে।
- তুমি যখন "কর্মচারী" খুঁজে এলে, তুমি প্রথম ফলকটাই পড়লে — default নিয়ম চলল।
- তুমি যখন সরাসরি "করিম" খুঁজে এলে, তার নিজের ফলকটা পড়লে — ঠিক নিয়ম চলল।

**`override` = পুরোনো ফলকটা নামিয়ে সেই একই জায়গায় নিজেরটা টাঙানো।**
**`new` = পুরোনোটা রেখে দিয়ে পাশে আরেকটা টাঙানো।**

এই "একই জায়গা" কথাটা রূপক না — নিচে দেখবে ওটা **আক্ষরিক অর্থে একটা slot নম্বর**।

---

## 3. Minimal example — ঠিক করা

দুইটা শব্দ। এটুকুই।

```csharp
public abstract class Account
{
    public decimal Balance { get; protected set; }
    protected Account(decimal opening) => Balance = opening;

    public abstract decimal MonthlyInterest();      // ← "default নেই। তোমাকে বলতেই হবে।"
}

public class SavingsAccount : Account
{
    public SavingsAccount(decimal opening) : base(opening) { }
    public override decimal MonthlyInterest() => Balance * 0.05m / 12;
}

public class FixedDeposit : Account
{
    public FixedDeposit(decimal opening) : base(opening) { }
    public override decimal MonthlyInterest() => Balance * 0.08m / 12;
}

public class CurrentAccount : Account
{
    public CurrentAccount(decimal opening) : base(opening) { }
    public override decimal MonthlyInterest() => 0m;    // ← ইচ্ছাকৃত ০, দুর্ঘটনাজনিত ০ না
}
```

Month-end job এর **একটা লাইনও বদলায়নি**:

```csharp
foreach (Account a in accounts)
    total += a.MonthlyInterest();          // ✅ প্রতিটা account নিজের নিয়মে
```

দুইটা জিনিস আলাদা করে খেয়াল করো:

**(ক) `virtual` না, `abstract` করলাম কেন?**
কারণ মূল অপরাধী `new` keyword টা না — মূল অপরাধী ছিল base class এর ওই `return 0m`।
ওটা একটা **মিথ্যা default** ছিল। "জানি না" কে "শূন্য" বলে চালিয়ে দেওয়া হয়েছিল।

> **যেখানে সত্যিকারের কোনো sensible default নেই, সেখানে `virtual` + fake default দিও না — `abstract` করো।**
> তাহলে `StudentAccount` লেখার দিন **compiler তোমাকে থামাবে**, গ্রাহক না।

**(খ) `CurrentAccount` এর `0m` কিন্তু ঠিক আছে** — কারণ ওটা লেখা আছে, ভাবা হয়েছে, আর
একটা সচেতন সিদ্ধান্ত। ভুলে যাওয়ার সুযোগ নেই।

---

## 4. ভেতরে কী হচ্ছে — method table (vtable)

এবার নাম-ফলকের আসল রূপটা।

.NET এ প্রতিটা object এর ভেতরে একটা লুকানো pointer থাকে, যেটা তার **type এর method table**
কে দেখায়। ওই table টা মূলত একটা তালিকা — **নম্বর করা কতগুলো slot**, প্রতিটা slot এ
একটা method এর ঠিকানা।

```
Account এর table
┌──────┬──────────────────────────────┐
│ slot0│ Account.MonthlyInterest()    │
└──────┴──────────────────────────────┘
```

এখন দুইভাবে subclass লেখা যায়:

```
✅ override দিলে                        ❌ new দিলে
SavingsAccount এর table                 SavingsAccount এর table
┌──────┬─────────────────────────────┐  ┌──────┬─────────────────────────────┐
│ slot0│ Savings.MonthlyInterest()   │  │ slot0│ Account.MonthlyInterest()   │ ← অপরিবর্তিত!
└──────┴─────────────────────────────┘  │ slot1│ Savings.MonthlyInterest()   │ ← নতুন ঘর
                                        └──────┴─────────────────────────────┘
```

এখন পুরো রহস্যটা এক বাক্যে খুলে যায়:

> **`Account a` দিয়ে call করলে compiler জানে "slot 0 এ যা আছে সেটা চালাও"।**
> `override` এ slot 0 এ নতুন method বসেছে → নতুনটা চলল।
> `new` এ slot 0 এ পুরোনোটাই বসে আছে → পুরোনোটা চলল। slot 1 এর কথা compiler জানেই না,
> কারণ `Account` type এ slot 1 বলে কিছু নেই।

তিনটা keyword এর মানে এবার আক্ষরিক:

| keyword | vtable এ যা ঘটে | মানে |
|---|---|---|
| `virtual` | slot টা "দখলযোগ্য" চিহ্নিত হয় | "চাইলে বদলাতে পারো" |
| `abstract` | slot আছে, ভেতরে কিছু নেই | "তোমাকে বদলাতেই হবে" |
| `override` | ঐ slot এ নিজের method বসল | দখল |
| `new` | নতুন slot, পুরোনোটা অক্ষত | দখল **না** — শুধু পাশে বসা |

*("slot" কথাটা আমার বানানো রূপক না — CLI standard (ECMA-335) এ method এর উপর
আক্ষরিক দুইটা flag আছে: **`reuseslot`** (default — base এর slot টাই ব্যবহার করো) আর
**`newslot`** (সবসময় নতুন slot নাও)। `override` = reuseslot, `new` = newslot।
তোমার C# keyword দুইটা সরাসরি এই দুইটা IL flag এ অনুবাদ হয়।)*

**সৎ থাকার জন্য দুইটা nuance** (আজ মুখস্থ করার দরকার নেই, ভুল ধারণা যেন না জন্মায়):

1. C# compiler প্রায় সব instance call এর জন্যই IL এ `callvirt` লেখে — এমনকি non-virtual
   method এর জন্যও — কারণ `callvirt` free তে null-check দিয়ে দেয়। কিন্তু method টা
   virtual না হলে **JIT target টা আগেই জানে**, table এ খোঁজার দরকার হয় না।
2. Virtual dispatch এর খরচ একটা extra indirection — nanosecond এর ব্যাপার, আর `sealed`
   হলে JIT অনেক সময় ওটাও সরিয়ে দেয় (devirtualization)। **Performance এর অজুহাতে
   `virtual` এড়িও না।** `virtual` এড়ানোর ভালো কারণ আছে (নিচে ৭ নম্বরে), কিন্তু গতি সেটা না।

---

## 5. দুই রকম polymorphism — subtype vs ad-hoc

এতক্ষণ যা দেখলে সেটা **subtype polymorphism** (বইয়ের ভাষায় inclusion polymorphism)।
কিন্তু C# এ "polymorphism" নামে আরেকটা জিনিসও আছে, আর ওটা **সম্পূর্ণ ভিন্ন যন্ত্রে চলে**।

**Ad-hoc polymorphism = overloading.** একই নাম, ভিন্ন parameter:

```csharp
static void Log(Account a)        => Console.WriteLine("Account লগ করলাম");
static void Log(SavingsAccount s) => Console.WriteLine("Savings লগ করলাম");
```

এবার পাশাপাশি দুইটা লাইন — **একই object দিয়ে**:

```csharp
Account acc = new SavingsAccount(100_000m);

Log(acc);                      // "Account লগ করলাম"     ← 😳
acc.MonthlyInterest();         // Savings এরটা চলল        ← 🙂
```

একই object, একই দুই লাইনের ভেতরে, দুইটা আলাদা নিয়ম:

| | Ad-hoc (overload) | Subtype (override) |
|---|---|---|
| কে সিদ্ধান্ত নেয় | **Compiler** | **CLR / runtime** |
| কখন | Compile time | Run time |
| কী দেখে সিদ্ধান্ত নেয় | Reference এর **ঘোষিত** type | Object এর **আসল** type |
| যন্ত্র | Overload resolution | Method table lookup |
| Runtime এ বদলানো যায়? | না, binary তে গাঁথা | হ্যাঁ, object যা সেটাই চলে |

`Log(acc)` লাইনে compiler শুধু জানে `acc` এর ঘোষিত type হলো `Account` —
ভেতরে আসলে কী আছে সেটা compile time এ জানার কোনো উপায়ই নেই। তাই সে `Log(Account)`
বেছে নেয়, আর সেই সিদ্ধান্ত **binary তে পাকাপাকি লেখা হয়ে যায়**।

> **মনে রাখার বাক্য: Overload দেখে তুমি কী *বলেছ*। Override দেখে জিনিসটা কী *আছে*।**

*(তৃতীয় একটা প্রকারও আছে — **parametric polymorphism**, মানে generics: `List<T>`.
একই code, অনেক type, আবারও compile time এ। আজ শুধু নামটা চিনে রাখো।)*

---

## 6. Bad vs Good — পাশাপাশি

| | `new` / `if is` chain | `abstract` + `override` |
|---|---|---|
| `Account` reference দিয়ে call | ভুল উত্তর, নীরবে | ঠিক উত্তর |
| একই object, দুই reference type | **দুইটা আলাদা উত্তর** | সবসময় একটাই উত্তর |
| নতুন `StudentAccount` যোগ করলে | compiler চুপ, silently `0` | **compile error — লিখতেই হবে** |
| interest নিয়মটা থাকে | Service class এ, type থেকে দূরে | যে type এর নিয়ম, তার ভেতরেই |
| ৪টা behaviour, ৫টা type | ৪টা switch × ৫টা branch = ২০ জায়গা | ৫টা class, প্রতিটায় ৪টা method |
| Test এ fake account | সম্ভব না — switch টা concrete type চেনে | subclass বানিয়ে দাও |
| Bug ধরা পড়ে | production এ, গ্রাহকের অভিযোগে | build এ |

**সৎ কথাটা:** `abstract` করলে প্রতিটা নতুন subclass এ method টা লিখতে *বাধ্য* হবে —
এমনকি যেখানে `0` ই উত্তর সেখানেও। ওই বাড়তি তিন লাইন লেখাটাই দাম।
আর ওটাই আসলে feature — **"ভাবিনি" আর "ভেবে ০ দিয়েছি" দুইটা আলাদা জিনিস।**

---

## 7. "Is there a simpler way?" — আজকের architect অংশ

আজকের পাঠ থেকে ভুল শিক্ষা নেওয়া খুব সহজ: *"তাহলে সব method `virtual` করে দিই!"*
**না।** তিনটা জায়গায় থামতে হবে।

### (ক) প্রতিটা `virtual` একটা চিরস্থায়ী প্রতিশ্রুতি

Day 3 এর fragile base class মনে আছে? `virtual` লেখার মানে তুমি বলছ:
*"এই method টা যে কেউ বদলাতে পারবে, আর আমি ভবিষ্যতেও সেটা চালু রাখব।"*
এটা **public API surface** — একবার দিলে ফেরত নেওয়া breaking change।

> .NET এর guidance: **default এ `sealed`, প্রয়োজনে খুলো।**
> `virtual` দাও তখনই যখন তুমি সত্যিই একটা extension point *চাও* — "হয়তো কাজে লাগবে" বলে না।

### (খ) `if (x is ...)` সবসময় ভুল না — প্রশ্নটা "আচরণটা কার ব্যবসা?"

`account.MonthlyInterest()` — interest এর নিয়ম account এর **নিজের ব্যবসা**। ওটা virtual হবে।

কিন্তু `account.RenderHtml()`? এখন তুমি UI কে domain এর ভেতর টেনে আনলে। **এটা আরও খারাপ।**

> **আচরণটা যদি type এর নিজের ব্যবসা হয় → ভেতরে নাও, virtual করো।**
> **আচরণটা যদি বাইরের কারো (UI, export, report) ব্যবসা হয় → বাইরেই থাকুক।**

আর type টা যদি তোমার নিজের না হয় (third-party, generated GIR class) — তখন switch ছাড়া
উপায়ই নেই। *(এরকম বাইরের switch যখন অনেকগুলো হয়, তার একটা organized রূপ আছে —
**Visitor pattern**, Day 53। আজ শুধু জেনে রাখো যে সমস্যাটার একটা নাম আছে।)*

### (গ) Polymorphism সিদ্ধান্তটা মুছে দেয় না — সরিয়ে দেয়

`if (a is SavingsAccount)` তুমি সরালে ঠিকই, কিন্তু **কোথাও না কোথাও কাউকে তো
`new SavingsAccount()` করতেই হবে।** সেটা করে deserializer, DI registration, বা একটা factory।

তুমি যা জিতলে সেটা হলো: **সিদ্ধান্তটা ২০ জায়গা থেকে ১ জায়গায় এল।**
*(ওই "১ জায়গা" টার নামই Factory — Day 29।)*

---

## 8. Apply it — তোমার Orbitax stack

আজ **তিনটা** ছোট শিকার, ২০ মিনিটের বেশি না।

**১. তুমি প্রতিদিন vtable ব্যবহার করছ, নাম দাওনি।**
`object` এর table এ জন্ম থেকেই তিনটা virtual slot বসানো: `ToString()`, `Equals()`,
`GetHashCode()`। `Console.WriteLine(myObject)` কাজ করে **কারণ ওটা তোমার class এর
override করা `ToString()` slot এ গিয়ে পড়ে**। আজকের পাঠের সবচেয়ে দৈনন্দিন প্রমাণ।

**২. Moq কেন interface mock করতে বলে — উত্তরটা আজ পেলে।**
`Mock<T>` কেবল **virtual / abstract / interface** member intercept করতে পারে —
কারণ ওগুলোরই slot দখল করা যায়। একটা concrete class এর non-virtual method mock করতে
গেলে Moq যে exception টা ছোঁড়ে সেটা হুবহু এই কথাটাই বলছে:

> `Invalid setup on a non-virtual (overridable in VB) member: x => x.Calculate()`

**Interface হলো ১০০% virtual একটা table — তাই ওটাই mock করা সবচেয়ে সহজ।**
Third-party library mock করা যাচ্ছে না বলে যে বিরক্তিটা হয়, ওটার কারণও এটাই।

**৩. MediatR এর dispatch কিন্তু vtable না — এই পার্থক্যটা ধরো।**
`_mediator.Send(command)` এক call site থেকে বহু behaviour চালায়, ঠিক polymorphism এর মতোই।
কিন্তু যন্ত্রটা আলাদা: এখানে **runtime এ একটা dictionary lookup** হয় —
request type → handler type — আর তালিকাটা বানায় DI container।

| | vtable dispatch | container dispatch (MediatR) |
|---|---|---|
| তালিকা তৈরি হয় | type load এর সময়, CLR করে | app startup এ, তোমার registration করে |
| বদলানো যায়? | না | হ্যাঁ — registration বদলে দাও |
| ভুল হলে ধরা পড়ে | compile time | **runtime** |

*(এই কথাটা Day 47 (Command) আর Day 48 (Mediator) এ ফিরে আসবে।)*

**হাতে-কলমে শিকার (এটা করো, শুধু পড়ো না):**

- Codebase এ `grep` করো: `public new ` — কিছু পেলে? পেলে ওটা **একটা জ্যান্ত মাইন**।
  কী পেলে notes.md এ লিখো।
- একটা `switch` বা `if (x is ...)` খুঁজে বের করো যেটা type বা enum এর উপর চলছে।
  নিজেকে প্রশ্ন করো: **এই আচরণটা কি ওই type এর নিজের ব্যবসা?**
  হ্যাঁ হলে ওটা refactor candidate — Day 10 (OCP) এ এই finding টা লাগবে, লিখে রাখো।

---

## 9. আজকের hands-on task

`journey/code/day-05/Day05.cs` এ scaffold আছে। **হাতে টাইপ করবে, copy-paste না।**
`HandsOnPractice/` এ একটা নতুন `Polymorphism` project বানাও (বাকিগুলোর মতো করেই),
ভেতরে `Bad Example/` আর `Good Example/`।

চারটা কাজ, এর বেশি না:

1. **টাকার bug টা নিজের চোখে দেখো।** `new` দিয়ে `SavingsAccount` লেখো। তারপর
   ছাপাও `savings.MonthlyInterest()` আর `((Account)savings).MonthlyInterest()`।
   **দুইটা সংখ্যা notes.md এ লিখে রাখো।** এক object, দুই উত্তর — এই ধাক্কাটাই আজকের পাঠ।
2. **`virtual`/`override` দিয়ে ঠিক করো।** ঐ একই দুই লাইন এখন এক সংখ্যা দেবে।
   **`foreach` loop এর একটা অক্ষরও বদলাবে না** — সেটাই মূল কথা।
3. **`abstract` করে compiler কে দিয়ে কাজ করাও।** এবার `StudentAccount : Account`
   লেখো, কিন্তু `MonthlyInterest()` **ইচ্ছা করে লিখো না**। Compiler কী বলল — error টা
   হুবহু notes.md এ লিখে রাখো। এটাই আজকের সবচেয়ে গুরুত্বপূর্ণ output।
4. **Overload ফাঁদটা প্রমাণ করো।** `Log(Account)` আর `Log(SavingsAccount)` লেখো,
   তারপর একটা `Account`-typed variable দিয়ে call করো। যা ছাপল সেটা লিখে রাখো,
   আর **এক লাইনে কারণটা লিখো।**

**সময় থাকলে (optional):**

5. Switch-based `InterestService` টা লেখো, তারপর `StudentAccount` যোগ করো।
   **গোনো — কয় জায়গায় হাত দিতে হলো, আর compiler কয়বার সাহায্য করল?**
6. `sealed override` লিখে দেখো। প্রশ্ন: কেউ কেন ইচ্ছা করে দরজা বন্ধ করতে চাইবে?
   *(হিন্ট: Day 3 এর fragile base class।)*

---

## 10. One-line self-check

> **নিজের ভাষায় বলো: `override` আর `new` — CLR এর দিক থেকে ঠিক কোন জায়গায় আলাদা?**

সহজ উত্তর: **`override` base এর ঐ vtable slot টাই দখল করে, `new` পাশে একটা নতুন slot
বানায় আর base এর slot টা অক্ষত রেখে দেয়।** তাই base type এর reference দিয়ে call করলে —
আর সে সবসময় base এর slot নম্বরই চায় — `override` এ নতুন code চলে, `new` এ পুরোনোটা চলে।
**একই object, ভিন্ন উত্তর।**

আর দ্বিতীয় অর্ধেকটা: **overload compiler বেছে দেয় ঘোষিত type দেখে (compile time),
override CLR বেছে দেয় আসল type দেখে (runtime)।**

---

## কালকের প্রস্তুতি (Day 6)

**Coupling & Cohesion — high cohesion, low coupling; আর নিজের একটা পুরোনো class কে নম্বর দেওয়া।**

Week 1 এর শেষ শেখার দিন। এতদিন শিখেছ *একটা* class কীভাবে গড়তে হয় —
কাল শিখবে **class গুলো একে অপরের সাথে কীভাবে জড়ানো উচিত, আর কতটা**।
আজকের `InterestService` টা মনে রেখো — কাল ওটাকে নম্বর দেব।

*(Day 7 retrieval day — Day 1–6 এর সব কিছুর self-test।)*

---

*Day 5 of 90 · টার্গেট: "এখন আমার বেসিক শক্তিশালী।"*
