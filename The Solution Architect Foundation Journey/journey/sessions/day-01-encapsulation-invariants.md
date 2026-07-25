# Day 1 of 90 — Encapsulation as *Invariant Protection*

**Block:** Month 1 (Foundation: OOD + SOLID) · Week 1 (OOP in depth)
**Date:** 2026-07-25 · **START_DATE recorded.**

আজকে শুরু। ৯০ দিন পরে target একটাই — **"এখন আমার বেসিক শক্তিশালী।"**

আজকের একটা লাইনের thesis:

> **Encapsulation মানে field লুকানো না। Encapsulation মানে — একটা rule (invariant) কে এমনভাবে পাহারা দেওয়া যেন object টা কোনোদিন ভুল অবস্থায় (invalid state) থাকতে না পারে।**

---

## 1. The problem first — একটা naive version যা ব্যথা দেয়

তোমাকে কেউ বলল: "একটা BankAccount বানাও, balance কখনো negative হবে না।" প্রথম চেষ্টা সবাই এটাই করে:

```csharp
// ❌ Version 0 — public field
public class BankAccount
{
    public decimal Balance;   // "খোলা মাঠ"
}
```

```csharp
var acc = new BankAccount();
acc.Balance = -5000;   // ✅ compiles. ✅ runs. ❌ business rule ধ্বংস।
```

তো সবাই বলে — "private করে দাও, property দাও।" ঠিক আছে:

```csharp
// ❌ Version 1 — private field + public setter. "encapsulated" বলে মানুষ ভাবে।
public class BankAccount
{
    private decimal _balance;                       // private! 🎉
    public decimal Balance                          // property! 🎉
    {
        get => _balance;
        set => _balance = value;                    // ...কিন্তু এখানে কোনো পাহারা নেই
    }
}
```

```csharp
acc.Balance = -5000;   // ❌ আবার একই বিপদ।
```

**এটাই আজকের আসল শিক্ষা।** Version 1 এ field private, property আছে, দেখতে "proper OOP" — কিন্তু **encapsulation zero**। কারণ বাইরের যে কেউ balance যা খুশি বসিয়ে দিতে পারে। আমরা শুধু একটা tunnel বানিয়েছি, gate বসাইনি.

আরেকটা আরও ধূর্ত failure — setter এ validation আছে, কিন্তু rule ফাঁকি দেওয়া যায়:

```csharp
// ❌ Version 2 — validation আছে, তবুও ভাঙে
public class BankAccount
{
    public decimal Balance { get; private set; }

    public void Withdraw(decimal amount)
    {
        if (Balance - amount < 0) throw new InvalidOperationException("Insufficient funds");
        Balance -= amount;
    }

    public void Deposit(decimal amount) => Balance += amount;   // amount = -5000 দিলে? 💀
}
```

`Deposit(-5000)` — withdraw এর guard কে পাশ কাটিয়ে balance negative। **একটা invariant একটা জায়গায় guard করলে হয় না; object এ ঢোকার প্রত্যেকটা দরজায় guard লাগবে।**

---

## 2. The idea in plain language — analogy

**নাইটক্লাবের bouncer.**

দুইটা আলাদা জিনিস মেলাও না:

- **Private field** = ক্লাবের ভেতরটা রাস্তা থেকে দেখা যায় না → এটা শুধু *information hiding*.
- **Encapsulation** = প্রত্যেকটা দরজায় bouncer দাঁড়ানো, আর **দরজাই শুধু ঢোকার পথ**। ID না দেখিয়ে কেউ ঢুকতে পারবে না — সামনের দরজা দিয়েও না, পেছনের দরজা দিয়েও না, জানালা দিয়েও না।

দেয়াল উঁচু করলেই নিরাপদ না। **নিরাপদ তখনই, যখন সব দরজা তোমার হাতে আর প্রত্যেক দরজায় guard আছে।**

আর "ID check" টাই হলো **invariant** — যে সত্যটা object এর জন্মের প্রথম মুহূর্ত থেকে মৃত্যু পর্যন্ত সবসময় সত্য থাকতে হবে। এখানে: `Balance >= 0`, সবসময়।

তিনটা প্রশ্ন যেটা encapsulation design করার সময় করবে:

1. **এই object এর invariant কী?** (এক লাইনে লেখো — না লিখতে পারলে তুমি জানো না।)
2. **State বদলানোর কতগুলো পথ আছে?** (constructor + প্রত্যেকটা public method + প্রত্যেকটা setter + কোনো mutable object যা তুমি বাইরে দিয়ে দিয়েছ)
3. **প্রত্যেকটা পথে guard আছে?** একটা বাদ পড়লেই encapsulation শেষ।

---

## 3. Minimal runnable example — the invariant-protected version

```csharp
public sealed class BankAccount
{
    // INVARIANT: _balance >= 0, always. জন্ম থেকে শেষ পর্যন্ত।
    private decimal _balance;

    public BankAccount(decimal openingBalance)   // দরজা #1: জন্মেই valid
    {
        if (openingBalance < 0)
            throw new ArgumentOutOfRangeException(nameof(openingBalance),
                "Opening balance cannot be negative.");
        _balance = openingBalance;
    }

    public decimal Balance => _balance;          // read-only বাইরে। কোনো setter নেই।

    public void Deposit(decimal amount)          // দরজা #2
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Deposit must be positive.");
        _balance += amount;
    }

    public void Withdraw(decimal amount)         // দরজা #3
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Withdrawal must be positive.");
        if (amount > _balance)
            throw new InvalidOperationException("Insufficient funds.");
        _balance -= amount;
    }
}
```

কী কী বদলাল, খেয়াল করো:

| Choice | কেন |
|---|---|
| `Balance` এর public setter নেই | State বদলানোর কোনো unguarded পথ নেই |
| Constructor এ validation | Object কখনো invalid হয়ে জন্মাবে না |
| `Deposit`/`Withdraw` দুইটাতেই amount check | পেছনের দরজা বন্ধ |
| `sealed` | কেউ subclass করে rule টা override করতে পারবে না (Day 3, 12 এ এটা আবার আসবে — LSP) |
| Method এর নাম business ভাষায় (`Withdraw`, না `SetBalance`) | Object টা *কী করতে পারে* সেটা বলে, *কী ধরে রাখে* সেটা না |

**Level up (মনে রাখো, Day 19 এ কাজে লাগবে):** সবচেয়ে শক্ত encapsulation হলো — invalid state কে **অসম্ভব** করে দেওয়া, শুধু throw করা না। `decimal amount` মানে `-5000` টাইপ-লেভেলে বৈধ, তাই runtime check লাগছে। একটা `PositiveAmount` value type বানালে ভুলটা আর `BankAccount` এ ঢুকতেই পারে না:

```csharp
public readonly record struct Money
{
    public decimal Value { get; }
    private Money(decimal v) => Value = v;

    public static Money Positive(decimal v) =>
        v > 0 ? new Money(v) : throw new ArgumentOutOfRangeException(nameof(v));
}
// এখন Deposit(Money amount) — negative deposit কে representable-ই না।
```

> "Make illegal states unrepresentable." এটা encapsulation এর শেষ ধাপ। আজ শুধু চিনে রাখো।

### সবচেয়ে common leak — collection টা ফাঁস হয়ে যাওয়া

এই bug টা production এ অনেক দেখবে:

```csharp
// ❌ invariant ফাঁস
public class Order
{
    private readonly List<OrderLine> _lines = new();
    public List<OrderLine> Lines => _lines;   // 💀 caller .Clear() / .Add() করতে পারে
}                                             //    তোমার total/discount rule বাইপাস

// ✅ ভেতরটা তোমার হাতেই থাকল
public class Order
{
    private readonly List<OrderLine> _lines = new();
    public IReadOnlyList<OrderLine> Lines => _lines;      // read-only view

    public void AddLine(OrderLine line)                    // guarded একমাত্র দরজা
    {
        if (_lines.Count >= 100) throw new InvalidOperationException("Order line limit reached.");
        _lines.Add(line);
    }
}
```

`IReadOnlyList<T>` return করলেও মনে রাখো — element গুলো mutable হলে সেগুলোর ভেতরের state এখনও বদলানো যায়। গভীর সুরক্ষা চাইলে element গুলোও immutable করো।

---

## 4. Apply it in your world — Orbitax .NET stack

Clean Architecture তে **domain layer এর entity গুলোই invariant এর আসল মালিক** — validator না, handler না।

**এখানে হান্ট করো (আজই, ১৫ মিনিট):**

1. **তোমার domain entity গুলো খোলো।** যেগুলোর সব property `{ get; set; }` — ওরা entity না, ওরা **anemic DTO**. প্রশ্ন করো: এই object এর invariant কী, আর সেটা কে enforce করছে?

2. **FluentValidation vs entity invariant — এই পার্থক্যটা আজ পরিষ্কার করো।**
   - FluentValidation একটা **request/command এর edge guard** — বাইরের নোংরা input তাড়াতাড়ি reject করার জন্য, ভালো error message সহ।
   - কিন্তু validator টা **entity এর invariant না**. Validator বাইপাস করে (background job, data migration, অন্য কোনো handler) কেউ entity বানালে rule টা কে পাহারা দিচ্ছে? উত্তর যদি "কেউ না" হয় — invariant টা আসলে enforce হচ্ছে না, শুধু *সাধারণত* respect হচ্ছে।
   - **দুইটাই রাখো।** Validator = ভদ্রতা (ভালো UX, 400 response)। Entity guard = সত্য (রক্ষাকবচ)।

3. **MongoDB এর একটা বিশেষ ফাঁদ — এটা খুব important তোমার জন্য।** Mongo C# driver deserialization এ প্রায়ই private setter বা field এ সরাসরি লেখে, constructor validation চালায় না। মানে DB তে আগে থেকে বসে থাকা একটা corrupt document তোমার guard এর ভেতর দিয়ে না গিয়েও invalid entity তৈরি করে ফেলবে। তাই:
   - Encapsulation টা **নতুন corruption** ঠেকায়, পুরোনোটা আপনা-আপনি ঠিক করে না।
   - Persistence layer টা তোমার invariant এর একটা **পেছনের দরজা** — জেনে রাখো, আর দরকার হলে load এর পরে সচেতনভাবে validate করো।

4. **GIR XML tooling:** ওখানে নিশ্চয়ই "এই element থাকলে ওই element must থাকতে হবে" টাইপ rule আছে। এগুলো invariant। এখন কি এগুলো XML লেখার সময় ছড়িয়ে ছিটিয়ে check হচ্ছে, নাকি এমন একটা model আছে যেটা invalid combination কেই বানাতে দেয় না? (Day 31, Builder — এই প্রশ্নের উত্তরটাই।)

---

## 5. "Is there a simpler way?" — সবসময় এই প্রশ্নটা

আজকের কৌশলটা সস্তা, কিন্তু তবুও প্রশ্ন করো:

**কখন এটা over-engineering:**
- **সত্যিকারের DTO / API contract / config object** — কোনো invariant নেই, শুধু ডেটা বইছে। `{ get; set; }` একেবারে ঠিক। এগুলোকে জোর করে rich domain object বানানো মূর্খতা।
- **Serialization boundary** — যেখানে framework কে property লিখতে দিতেই হবে।
- **প্রত্যেক primitive এর জন্য value object** — `Money` চমৎকার। কিন্তু ৪০টা wrapper type বানালে সেটা আর design না, ceremony। Value object সেখানেই বানাও যেখানে rule টা বারবার ভুল হচ্ছে বা unit mix হওয়ার আসল ঝুঁকি আছে (currency, tax rate, period)।

**কখন এটা non-negotiable:**
- Money, balance, tax amount, filing status, state transition — অর্থাৎ যেখানে ভুল state এর মানে **ভুল টাকা বা ভুল compliance**। তোমার domain টা ঠিক এই ধরনের।

সাধারণ নিয়ম: **encapsulation এর দাম দাও invariant এর ওজন অনুযায়ী।** কোনো rule নেই → simple property। টাকা/compliance জড়িত → পাহারা বসাও।

---

## 6. আজকের hands-on task (লিখতে হবে, পড়লে হবে না)

`journey/code/day-01/` তে scaffold রেডি আছে (`Day01.cs`)। **নিজে টাইপ করো, copy না।**

1. `NaiveBankAccount` টা দিয়ে balance negative করে ফেলো — নিজের চোখে ভাঙতে দেখো।
2. `BankAccount` টা নিজে লিখো, তারপর এগুলো force করো — প্রত্যেকটায় exception পেতে হবে:
   - opening balance negative
   - `Deposit(-5000)`
   - `Withdraw(0)`
   - balance এর চেয়ে বেশি withdraw
3. **আসল কাজ — একটা invariant যোগ করো যেটা একাধিক দরজা জোড়া লাগায়:**
   > *"দিনে মোট withdrawal ৫০,০০০ এর বেশি হতে পারবে না।"*
   এখন তোমাকে ভেতরে extra state রাখতে হবে (আজকের total, কোন তারিখ)। প্রশ্নগুলো নিজেকে করো: কে ওই counter reset করে? Caller নিশ্চয়ই না — তাহলে সে rule টা বাইপাস করতে পারবে। এই তৃতীয় ধাপেই encapsulation কেন *design*, শুধু syntax না, সেটা টের পাবে।
4. `Order.Lines` leak টা fix করো — `List<T>` → `IReadOnlyList<T>` + guarded `AddLine`.
5. শেষে **তোমার নিজের একটা Orbitax entity** বেছে নাও, উপরে থাকা comment এ এক লাইনে ওর invariant লেখো, আর গুনো state বদলানোর কতগুলো unguarded পথ আছে। সংখ্যাটা `journey/code/day-01/notes.md` তে লিখে রাখো।

চালাতে: `dotnet run` (একটা console project এ ফেলে) অথবা `dotnet script Day01.cs`।

---

## 7. One-line self-check

> **নিজের ভাষায় বলো: একটা class এর সব field private, তবুও তার encapsulation ভাঙা থাকতে পারে — কীভাবে?**

উত্তরটায় এই তিনটা থাকা চাই: unguarded setter, unguarded constructor, আর ফাঁস হয়ে যাওয়া mutable reference (collection)।

---

## কালকের প্রস্তুতি (Day 2)

**Abstraction vs Encapsulation** — মানুষ এই দুইটা মিলিয়ে ফেলে। এক লাইনে পার্থক্য:
- Encapsulation = *ভেতরটা পাহারা দেওয়া* (কে state বদলাতে পারবে)
- Abstraction = *কী দেখাব সেটা ঠিক করা* (caller কোন ধারণাটা নিয়ে কাজ করবে)

আমরা একটা `IPaymentProcessor` design করব, আর তখন টের পাবে কেন abstraction লিক করলে সেটা encapsulation ভাঙার চেয়েও বেশি খরচ করায়।

---

*Day 1 of 90 · টার্গেট: "এখন আমার বেসিক শক্তিশালী।"*
