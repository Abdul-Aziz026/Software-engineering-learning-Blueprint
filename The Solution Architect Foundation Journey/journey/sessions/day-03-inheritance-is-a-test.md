# Day 3 of 90 — Inheritance: কখন হ্যাঁ, কখন না ("is-a" test)

**Block:** Month 1 (Foundation: OOD + SOLID) · Week 1 (OOP in depth)
**Date:** 2026-07-31

আজকের পুরো পাঠ এক লাইনে:

> **Inheritance দিয়ে code ধার করা যায় না। Inheritance মানে — subclass টা base এর
> সব প্রতিশ্রুতি রাখতে বাধ্য।**

Day 1 এ শিখেছ invariant পাহারা দিতে। Day 2 এ শিখেছ বাইরে কী দেখাবে ঠিক করতে।
আজ দেখবে — **একটা ভুল `:` চিহ্ন দুইটাই একসাথে ভেঙে দেয়।**

---

## 1. The problem first

তোমার একটা `Stack` লাগবে। মনে হলো — `List<T>` তো সব কাজ পারেই, ওটা থেকে inherit করলে
free তে Count, Clear, indexer, enumeration সব পেয়ে যাব।

```csharp
public class Stack<T> : List<T>          // 💀
{
    public void Push(T item) => Add(item);

    public T Pop()
    {
        var item = this[Count - 1];
        RemoveAt(Count - 1);
        return item;
    }
}
```

চলে। Test ও pass করে। তিন সপ্তাহ পর কেউ লিখল:

```csharp
var s = new Stack<string>();
s.Push("a");
s.Push("b");

s.Insert(0, "x");        // ✅ compile করে
s.RemoveAt(0);           // ✅ compile করে
s.Reverse();             // ✅ compile করে
s[0] = "hacked";         // ✅ compile করে
```

Stack এর **একটাই** নিয়ম ছিল — LIFO. **Last in, first out.**
`Insert(0, ...)` সেই নিয়মটা মেরে ফেলল, আর compiler টুঁ শব্দও করল না।

**এখানে দুইটা জিনিস খেয়াল করো:**

1. তুমি Day 1 এর সব নিয়ম মেনেছ — field private, guard আছে। **তবুও invariant গেল।**
   কারণ invariant টা তুমি ভাঙোনি — **base class টা সবার জন্য দরজা খোলা রেখে দিয়েছে।**
2. Day 2 এর ভাষায়: তুমি ৩টা method (`Push`/`Pop`/`Peek`) দেখাতে চেয়েছিলে,
   কিন্তু ড্যাশবোর্ডে ৭০টা method চলে এসেছে। **যেগুলো তুমি লেখোওনি।**

> **Inheritance এ তুমি শুধু যা চাও তা পাও না — যা আছে সব পাও।**

---

## 2. The idea — analogy

**Inheritance মানে ধার নেওয়া না, দত্তক নেওয়া।**

বন্ধুর কাছ থেকে ড্রিল মেশিন ধার নিলে — তুমি শুধু ড্রিলটা পাও। ওর ধারদেনা পাও না।
সেটা **composition**।

কিন্তু কোনো পরিবারে দত্তক গেলে তুমি পাও:
- পরিবারের পদবি (type identity — সবাই তোমাকে base হিসেবে দেখবে)
- পরিবারের সম্পত্তি (methods)
- **পরিবারের দেনা** (base এর সব public API, চিরকাল)
- **আর ভবিষ্যতে বাবা যা ধার করবেন, তাও** (base class কাল একটা method যোগ করলে তোমার class সেটা পেয়ে যাবে — তুমি না জেনেই)

শেষ পয়েন্টটাই সবচেয়ে ভয়ানক, ওর নাম **fragile base class problem**।

**তাই আসল প্রশ্নটা "code share করা যাবে?" না। আসল প্রশ্ন —**

> **আমি কি এই পরিবারের নাম নিয়ে, ওদের সব দেনা মাথায় নিয়ে, চিরকাল বাঁচতে রাজি?**

---

## 3. "is-a" test — কিন্তু সাবধান

সবাই শেখায়: *"is-a হলে inherit করো, has-a হলে compose করো।"*
নিয়মটা ভালো, কিন্তু **ইংরেজি বাক্য দিয়ে test করলে ঠকবে।**

- "Square **is a** Rectangle" — ইংরেজিতে ১০০% সত্যি। Code এ ভয়ংকর ভুল। *(Day 12 এ হাতে-কলমে ভাঙবে।)*
- "Stack **is a** List" — শুনতে খারাপ লাগে না। উপরে দেখলে কী হলো।

**আসল test টা vocabulary এর না, behaviour এর:**

> **Base যেখানে যেখানে ব্যবহার হয়, সেখানে subclass টা বসিয়ে দিলে —
> caller কি কিছুই টের পাবে না?**

এটাই **substitutability**, আর এটাই Day 12 এর LSP এর মূল কথা। আজ শুধু চিনে রাখো।

### তিনটা প্রশ্ন, `:` লেখার আগে

| # | প্রশ্ন | উত্তর "না" হলে |
|---|---|---|
| 1 | Base এর **প্রত্যেকটা** public method কি subclass এ অর্থপূর্ণ? | inherit কোরো না |
| 2 | Base কাল নতুন method যোগ করলে আমি কি নিশ্চিন্ত? | inherit কোরো না |
| 3 | আমি কি base টার মালিক, নাকি ওটা অন্য কারো/library র? | নিজের না হলে খুব সাবধান |

তিনটার একটাও "না" হলে — **composition**।

---

## 4. Minimal example — ঠিক করা

`Stack` **is-a** List না। Stack **has-a** List. এটুকুই বদল:

```csharp
public class Stack<T>
{
    private readonly List<T> _items = new();          // has-a, not is-a

    public int Count => _items.Count;

    public void Push(T item) => _items.Add(item);

    public T Pop()
    {
        if (_items.Count == 0)
            throw new InvalidOperationException("Stack is empty.");

        var item = _items[^1];
        _items.RemoveAt(_items.Count - 1);
        return item;
    }

    public T Peek() =>
        _items.Count > 0 ? _items[^1]
                         : throw new InvalidOperationException("Stack is empty.");
}
```

চারটা method। **`Insert` নেই। `Reverse` নেই। `this[i] = ...` নেই।**
LIFO এখন compile-time এ নিশ্চিত — কেউ চাইলেও ভাঙতে পারবে না।

দাম দিতে হলো: `Count` টা নিজে লিখতে হলো। **এটাই composition এর খরচ —
কিছু forwarding line।** বিনিময়ে যা পেলে: তোমার class এ ঠিক ততটুকুই আছে যতটুকু তুমি রেখেছ।

### তাহলে inheritance কি কখনোই না?

না, আছে। ভালো inheritance দেখতে এরকম — **base টা abstract, আর ওর কাজ একটা নিয়ম চাপানো**, code বিলি করা না:

```csharp
public abstract class TaxFiling
{
    public abstract string JurisdictionCode { get; }

    public abstract ValidationResult Validate();

    // শুধু এই একটা জায়গায় শেয়ার করা নিয়ম, আর কিছু না
    public FilingEnvelope Submit()
    {
        var result = Validate();
        if (!result.IsValid)
            throw new InvalidFilingException(result.Errors);

        return BuildEnvelope();
    }

    protected abstract FilingEnvelope BuildEnvelope();
}
```

কেন এটা ঠিক আছে:
- Base এ প্রায় কোনো state নেই → ভাঙার কিছু নেই
- Public surface ছোট → subclass অবাঞ্ছিত কিছু পাচ্ছে না
- `Submit()` এর নিয়মটা (validate না হলে submit না) **সব subclass এ জোর করে চাপানো হচ্ছে** — এটাই inheritance এর আসল ক্ষমতা
- এক স্তর গভীর। **দুই স্তরের বেশি হলেই থামো।**

*(এটার নাম Template Method pattern — Day 49. আজ শুধু চোখে দেখে রাখো।)*

---

## 5. Apply it — তোমার Orbitax stack

আজ **একটাই** কাজ, ১০-১৫ মিনিট।

Codebase এ `Base` দিয়ে শুরু হওয়া class খোঁজো — `BaseService`, `BaseRepository`,
`BaseController`, `BaseHandler`. একটা বেছে নিয়ে দুইটা সংখ্যা গোনো:

1. Base টায় কয়টা `public`/`protected` member আছে?
2. একটা নির্দিষ্ট subclass আসলে তার কয়টা ব্যবহার করে?

**দ্বিতীয় সংখ্যাটা প্রথমটার অর্ধেকের কম হলে** — ওটা inheritance না, ওটা একটা toolbox
যেটা ভুল করে base class এর ছদ্মবেশে আছে। ওটার সঠিক রূপ: একটা injected service।

দুইটা জিনিস আলাদা করে খেয়াল করো:

- **`DomainException : Exception`, `GirValidationException : DomainException`** — এটা ভালো inheritance।
  কারণ subclass টা সত্যিই *is-a* exception, আর `catch (DomainException)` লিখে সব ধরা যায়।
  Type identity টাই এখানে আসল লাভ, code sharing না।
- **MediatR pipeline behaviour** — লক্ষ্য করো, MediatR তোমাকে `BaseHandler` থেকে
  inherit করতে বলে না। প্রতিটা behaviour পরেরটাকে **ধারণ** করে (`next()`), inherit করে না।
  পুরো cross-cutting জিনিসটা inheritance ছাড়াই হচ্ছে। **এটা কাকতালীয় না।**
  *(এর নাম Decorator/Chain of Responsibility — Day 37 আর Day 50।)*

---

## 6. "Is there a simpler way?"

আজকে প্রশ্নটা উল্টো দিকে ঘুরিয়ে করতে হবে। Inheritance **নিজেই** সাধারণত জটিল উত্তর।

Base class লিখতে বসার আগে এই তিনটা দেখো:

| যা করতে চাইছ | Inheritance ছাড়া উপায় |
|---|---|
| দুই class এ একই helper method | একটা static helper, বা একটা ছোট injected service |
| কিছু class এ একটা করে বাড়তি behaviour | Extension method |
| অনেক class কে একটা নিয়ম মানাতে চাই | Interface (identity লাগে, code না) |
| একটা class কে চারপাশে মুড়ে দিতে চাই | Composition / Decorator |

**Inheritance তখনই নাও যখন `is-a` টা behaviour এর দিক থেকেও সত্যি,
আর তোমার type identity টাই দরকার — শুধু code না।**

**আজকের সবচেয়ে দামি লাইন:**

> **Inheritance হলো OOP এর সবচেয়ে শক্ত coupling।**
> Subclass টা base এর শুধু public API না, ওর *ভেতরের আচরণের* উপরেও নির্ভর করে বসে।
> তাই এটা প্রথম হাতিয়ার না — শেষ হাতিয়ার।

---

## 7. আজকের hands-on task

`journey/code/day-03/Day03.cs` তে scaffold আছে। **তিনটা কাজ, এর বেশি না:**

1. **ভাঙো।** `BrokenStack<T> : List<T>` দিয়ে `Insert(0, x)` চালাও, তারপর `Pop()` করো।
   ভুল জিনিসটা বেরিয়ে আসবে। **নিজের চোখে দেখো** — পড়ে বুঝলে হবে না।
2. **ঠিক করো।** `SafeStack<T>` টা composition দিয়ে নিজে টাইপ করো।
   এবার `Insert` লেখার চেষ্টা করো — compiler তোমাকে থামিয়ে দেবে। ওই মুহূর্তটাই আজকের পাঠ।
3. **Fragile base class অনুভব করো।** `LegacyReport` base টায় একটা নতুন method
   `public void Delete()` যোগ করো। এখন `ReadOnlyAuditReport` subclass টা দেখো —
   সে না চাইতেই `Delete()` পেয়ে গেছে। **তুমি ওর ফাইলে হাতও দাওনি।**

**সময় থাকলে (optional):**

4. `notes.md` তে "তিনটা প্রশ্ন" table টা তোমার codebase এর একটা আসল base class এ প্রয়োগ করো।
5. `SafeStack` এ `IEnumerable<T>` implement করার চেষ্টা করো। ভেবে দেখো — এতে কি LIFO ভাঙে?
   *(উত্তর: ভাঙে না। পড়া যায়, বদলানো যায় না। এই পার্থক্যটাই দামি।)*

---

## 8. One-line self-check

> **নিজের ভাষায় বলো: "is-a" শোনালেও inheritance ভুল হয় কখন?**

সহজ উত্তর: যখন subclass টা base এর **সব প্রতিশ্রুতি রাখতে পারে না** — অর্থাৎ base যেখানে
ব্যবহার হচ্ছে সেখানে subclass বসালে caller টের পেয়ে যায়। ইংরেজি বাক্য "Stack is a List"
ঠিক শোনায়, কিন্তু List প্রতিশ্রুতি দেয় "যেকোনো জায়গায় ঢোকানো যাবে" — আর ঠিক সেই
প্রতিশ্রুতিটাই Stack রাখতে পারে না। **Vocabulary না, behaviour দিয়ে test করো।**

---

## কালকের প্রস্তুতি (Day 4)

**Composition over Inheritance — একই feature দুইভাবে বানিয়ে পার্থক্যটা হাতে অনুভব করা।**

আজ জেনেছ inheritance কখন ভুল। কাল দেখবে composition দিয়ে কী কী **সম্ভব হয়ে যায়**
যেটা inheritance এ হতোই না — সবচেয়ে বড়টা: **runtime এ behaviour বদলানো।**

---

*Day 3 of 90 · টার্গেট: "এখন আমার বেসিক শক্তিশালী।"*
