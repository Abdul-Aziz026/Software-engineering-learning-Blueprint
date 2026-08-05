# Day 6 of 90 — Coupling & Cohesion: একটা class কতটা "একটা জিনিস", আর অন্যদের কতটা চেনে

**Block:** Month 1 (Foundation: OOD + SOLID) · Week 1 (OOP in depth)
**Date:** 2026-08-05

আজকের পুরো পাঠ এক লাইনে:

> **Cohesion = ভেতরের প্রশ্ন — এই class এর জিনিসগুলো কি সত্যিই একে অপরের?
> Coupling = বাইরের প্রশ্ন — এই class ভাঙলে আর কে কে ভাঙে?
> এই দুইটা মাপ ছাড়া "clean code" শব্দটা শুধু রুচির ব্যাপার। এই দুইটা দিয়ে মাপলে ওটা একটা তর্ক-যোগ্য সিদ্ধান্ত।**

Day 1–5 এ তুমি শিখেছ *একটা* class কীভাবে বানাতে হয় — invariant, abstraction,
inheritance, composition, polymorphism। আজ প্রথমবার প্রশ্নটা **class এর ভেতর থেকে
class-দের মাঝখানে** সরছে।

আজকের পর থেকে পুরো Week 2–3 (SOLID) আসলে এই দুই শব্দের ব্যাখ্যা।
**SRP মানে cohesion বাড়াও। DIP মানে coupling কমাও।** বাকিগুলো এই দুইটার ভিন্ন কোণ।

---

## 1. Goal

আজকের শেষে তুমি যেকোনো class দেখে **দুইটা সংখ্যা** বলতে পারবে —
এর ভেতরের অংশগুলো কতটা একসাথে থাকার যোগ্য (cohesion), আর একে বদলালে কতগুলো
file হাত দিতে হবে (coupling) — এবং কেন **সব coupling খারাপ না**।

---

## 2. ❌ The Bad Version

Orbitax-ঘেঁষা কিন্তু ছোট রাখলাম — একটা tax filing জমা দেওয়ার service।
Junior হিসেবে আমরা প্রায় সবাই ঠিক এটাই প্রথমে লিখি — কারণ এটা **কাজ করে**,
আর সব logic এক জায়গায় থাকায় প্রথম দিন পড়তেও সহজ লাগে:

```csharp
public class FilingService
{
    private readonly SqlConnection _db = new SqlConnection("Server=prod;Database=Tax;...");
    private readonly SmtpClient   _smtp = new SmtpClient("smtp.orbitax.com");

    public void Submit(Filing filing)
    {
        // 1. validate
        if (filing.TaxYear < 2000)          throw new Exception("bad year");
        if (filing.Amount  < 0)             throw new Exception("negative");
        if (filing.Country.Length != 2)     throw new Exception("bad country");

        // 2. calculate
        decimal tax = filing.Amount * 0.15m;
        if (filing.Country == "BD") tax = filing.Amount * 0.25m;

        // 3. build the XML
        var xml = $"<Filing><Year>{filing.TaxYear}</Year><Tax>{tax}</Tax></Filing>";

        // 4. save
        _db.Open();
        new SqlCommand($"INSERT INTO Filings VALUES ('{xml}')", _db).ExecuteNonQuery();
        _db.Close();

        // 5. notify
        _smtp.Send(new MailMessage("noreply@orbitax.com", filing.UserEmail,
                                   "Filed", $"Your {filing.TaxYear} filing is submitted."));

        // 6. log
        File.AppendAllText(@"C:\logs\filing.txt", $"{DateTime.Now}: filed {filing.Id}\n");
    }
}
```

চোখে দেখতে খারাপ লাগছে না, তাই না? সবকিছু এক জায়গায়, উপর থেকে নিচে পড়া যায়।
**এইটাই ফাঁদ।** "একসাথে পড়া যায়" আর "একসাথে থাকার কথা" এক জিনিস না।

---

## 3. Why it is bad — দুইটা আলাদা রোগ, একটাই class

### রোগ ১ — Low cohesion (ভেতরের রোগ)

এক লাইনে class টার কাজ বলো। পারবে না — বলতে হবে **"ও validate করে, tax হিসাব
করে, XML বানায়, DB তে লেখে, email পাঠায়, আর log লেখে।"**

> যেই মুহূর্তে class এর বর্ণনায় **"আর"** শব্দটা আসে, cohesion সেখানেই ফাঁস হয়ে গেছে।

**Analogy:** ভাবো তোমার রান্নাঘরের একটা ড্রয়ার — যেখানে চামচ, পাসপোর্ট, চার্জার,
আর ওষুধ একসাথে। ড্রয়ারটা **কাজ করে**, সবকিছু ধরে রাখে। কিন্তু কেউ জিজ্ঞেস করলে
"এই ড্রয়ারটা কীসের?" — উত্তর নেই। আর ওষুধ খুঁজতে গিয়ে তোমাকে পাসপোর্ট নাড়াতে হয়।

আসল test টা কিন্তু "দেখতে সুন্দর কিনা" না — **কে কখন বদলায়**:

| অংশ | কে বদলাতে বলে | কত ঘন ঘন |
|---|---|---|
| validation rules | সরকার / compliance | বছরে কয়েকবার |
| tax rate | বাজেট | বছরে একবার |
| XML format | schema version | কালেভদ্রে |
| DB write | infra team | খুব কম |
| email text | marketing | যখন খুশি |
| log format | ops | যখন খুশি |

ছয়টা আলাদা মানুষ, ছয়টা আলাদা তালে — **একটাই file এ**।
মানে marketing এর email text বদলানোর PR টাও ওই file এ, যেখানে tax calculation
বসে আছে। Merge conflict এর কারখানা, আর review এর সময় কেউ tax logic টা দ্বিতীয়বার
পড়ে না কারণ "এই PR টা তো শুধু email এর"।

> **Cohesion এর সংজ্ঞা যেটা কাজে লাগে:** একসাথে যেগুলো *বদলায়*, তারা একসাথে থাকুক।
> একসাথে যেগুলো *পড়া হয়* — সেটা কোনো যুক্তি না।

### রোগ ২ — High coupling (বাইরের রোগ)

এই class টা নিজের কাজ করতে গিয়ে **কতগুলো বাইরের জিনিস চেনে**, গুনি:

`SqlConnection` (concrete), connection string, table name `Filings`, SQL syntax,
`SmtpClient` (concrete), SMTP host, from-address, `File` API, `C:\logs\` path,
`DateTime.Now`।

**এগারোটা।** এখন সবচেয়ে ধারালো প্রশ্নটা — যেটা তোমার Day 5 এর test-লেখা অভ্যাসকে সরাসরি ধরবে:

```csharp
[Fact]
public void BD_filing_is_taxed_at_25_percent()
{
    var service = new FilingService();
    service.Submit(new Filing { Country = "BD", Amount = 1000m, ... });
    // ...এখন কী assert করব?
}
```

এই test টা চালাতে গেলে তোমার লাগবে — **একটা চালু SQL Server, একটা চালু SMTP
server, `C:\logs\` তে write permission** — শুধু `0.25m` সংখ্যাটা ঠিক আছে কিনা
দেখার জন্য। আর CI তে চালালে ওটা **সত্যিকারের email পাঠাবে**।

> **Coupling এর সবচেয়ে সৎ পরিমাপ যন্ত্রটা হলো unit test।**
> একটা class কে test করতে যত জিনিস দাঁড় করাতে হয়, coupling ঠিক তত।
> "Test লেখা কঠিন" কখনোই testing এর দোষ না — ওটা design এর রোগ নির্ণয়।

---

## 4. Problems it causes — কী ভাঙে, কবে

1. **Tax rate বদলাতে গিয়ে email ভাঙে।** এক file, ছয় কারণ — অসংশ্লিষ্ট পরিবর্তন
   একই জায়গায় ধাক্কা খায়।
2. **Test করা যায় না, তাই test লেখা হয় না**, তাই tax logic চিরকাল unverified থাকে।
3. **পুনঃব্যবহার অসম্ভব।** এখন একটা *bulk* filing দরকার — email ছাড়া, DB এর
   বদলে file এ। `Submit()` এর একটা অংশও নেওয়া যায় না; copy-paste ছাড়া উপায় নেই।
   এরপর tax rate দুই জায়গায় বদলাতে হয় — আর একদিন কেউ একটা ভুলে যায়।
4. **SMTP down মানে filing ব্যর্থ।** Email পাঠানো filing এর অংশ না, কিন্তু
   কোডে ওটা এখন অংশ। ব্যবসায়িকভাবে ভুল ব্যর্থতা।
5. **Blast radius অজানা।** `SqlConnection` ছেড়ে MongoDB তে গেলে কোথায় কোথায়
   হাত দিতে হবে — কেউ জানে না, কারণ কেউ চেনে না কে কাকে চেনে।

---

## 5. ✅ The Good Version — সবচেয়ে সহজ সঠিক রূপ

দুইটা আলাদা ওষুধ, কারণ রোগ দুইটা:

**ধাপ ১ — Cohesion ঠিক করো: যেগুলো একসাথে বদলায়, আলাদা করে ফেলো।**
(এখনো কোনো interface নেই। এই ধাপে শুধু কাঁচি।)

```csharp
public class FilingValidator                 // বদলায়: compliance বললে
{
    public void Validate(Filing f)
    {
        if (f.TaxYear < 2000)      throw new ArgumentException("bad year");
        if (f.Amount  < 0)         throw new ArgumentException("negative");
        if (f.Country.Length != 2) throw new ArgumentException("bad country");
    }
}

public class TaxCalculator                   // বদলায়: বাজেট বললে
{
    public decimal Calculate(Filing f)
        => f.Country == "BD" ? f.Amount * 0.25m : f.Amount * 0.15m;
}
```

**ধাপ ২ — Coupling ঠিক করো: যা বদলাতে পারে তার সাথে সরাসরি না, contract দিয়ে জোড়ো।**

```csharp
public interface IFilingStore  { void Save(Filing f, decimal tax); }
public interface INotifier     { void FilingSubmitted(Filing f); }

public class FilingService
{
    private readonly FilingValidator _validator;
    private readonly TaxCalculator   _calculator;
    private readonly IFilingStore    _store;
    private readonly INotifier       _notifier;

    public FilingService(FilingValidator validator, TaxCalculator calculator,
                         IFilingStore store, INotifier notifier)
    {
        _validator  = validator;
        _calculator = calculator;
        _store      = store;
        _notifier   = notifier;
    }

    public void Submit(Filing filing)
    {
        _validator.Validate(filing);
        decimal tax = _calculator.Calculate(filing);
        _store.Save(filing, tax);
        _notifier.FilingSubmitted(filing);
    }
}
```

`FilingService` এর কাজ এখন এক লাইনে বলা যায়, **"আর" ছাড়া**:
*"filing জমা দেওয়ার ধাপগুলো ঠিক ক্রমে চালানো।"* ওটা একটা conductor —
নিজে কোনো বাদ্যযন্ত্র বাজায় না।

আর test টা এখন:

```csharp
[Fact]
public void BD_filing_is_taxed_at_25_percent()
    => Assert.Equal(250m, new TaxCalculator()
           .Calculate(new Filing { Country = "BD", Amount = 1000m }));
```

**কোনো DB নেই, কোনো SMTP নেই, কোনো folder নেই।** একটা `new`, একটা assert।

---

## 6. What changed and why

| যে সমস্যাটা ছিল | কোন পরিবর্তন ওটা সারালো |
|---|---|
| এক class, ছয় কারণে বদলায় | কাজগুলো আলাদা class এ কাটা — cohesion ↑ |
| Test চালাতে DB + SMTP লাগত | `TaxCalculator` এর কোনো নির্ভরতা নেই — coupling ↓ |
| MongoDB তে যাওয়া = অজানা blast radius | `IFilingStore` এর একটামাত্র নতুন implementation |
| Bulk filing এ copy-paste | `TaxCalculator` যেকোনো জায়গা থেকে পুনঃব্যবহার্য |
| SMTP down = filing fail | `INotifier` এর পেছনে; পরে queue/no-op বসানো যায় |

লক্ষ্য করো — `FilingValidator` আর `TaxCalculator` **interface পায়নি**।
ইচ্ছাকৃত। ওগুলোর একটাই implementation থাকবে বলে আশা করছি, আর ওরা নিজেরা কিছু
চেনে না বলে test করতেও কষ্ট নেই। **Interface তখনই দাও যখন সত্যিই বিকল্প আছে
(DB, SMTP), শুধু "abstraction ভালো" বলে না।**

---

## 7. Bad vs Good — পাশাপাশি

| | ❌ Bad | ✅ Good |
|---|---|---|
| এক লাইনে কাজ | বলতে "আর" লাগে ×৫ | "ধাপগুলো ঠিক ক্রমে চালানো" |
| পরিবর্তনের কারণ | ৬টা | ১টা করে, প্রতি class এ |
| বাইরের জিনিস চেনে | ১১টা concrete | ৪টা — ২টা abstraction, ২টা নির্ভরতাহীন class |
| Tax rule test করতে লাগে | SQL + SMTP + file system | `new TaxCalculator()` |
| MongoDB তে সরাতে | অজানা | ১টা নতুন class |
| Email text বদলাতে | tax logic ছোঁয়া file এ PR | আলাদা file |
| Bulk filing এ পুনঃব্যবহার | copy-paste | সরাসরি |

---

## 8. Architect's reasoning — "আরো সহজ উপায় আছে কি?"

এখানেই আজকের আসল দিনটা। উপরের refactor টা **সবসময় সঠিক না**।

**Coupling শূন্য করা যায় না — সরানো যায় মাত্র।** `FilingService` এখনো চারটা
জিনিস চেনে; আমরা শুধু চেনাগুলোকে *ভঙ্গুর* থেকে *স্থিতিশীল* করেছি। Zero coupling
মানে zero কাজ — সম্পর্কহীন object কিছুই করতে পারে না। প্রশ্নটা কখনোই "coupling
আছে কি?" — সবসময় **"কীসের সাথে coupling?"**

মাপার নিয়ম: **আমি যার উপর নির্ভর করছি, সে কি আমার চেয়ে কম বদলায়?**
`decimal` এর উপর নির্ভরতা free — ও বদলাবে না। `IFilingStore` এর উপর নির্ভরতা
সস্তা — ওই signature টা বছরে একবারও নড়বে না। কিন্তু `SqlConnection` +
connection string + table নাম — ওটা infra এর ঘড়িতে চলে, তোমার ঘড়িতে না।

> **নিচের দিকে নির্ভর করো — যা তোমার চেয়ে ধীরে বদলায়, তার দিকে।** (Day 17 এ এটারই নাম হবে DIP.)

**আর কখন এই refactor over-engineering?**

যদি `Submit()` হতো শুধু validate + save — দুই ধাপ, একটাই caller, একটাই DB চিরকাল —
তাহলে দুইটা interface আর চারটা constructor parameter **খরচ, লাভ না**।
তখন সহজতর সঠিক উত্তর: একটাই class, ভেতরে ছোট private method।
এই ভাগাভাগির দাম শোধ হয় তিনটার একটা ঘটলে — (ক) সত্যিই দ্বিতীয় implementation
লাগবে, (খ) অংশগুলো সত্যিই আলাদা তালে বদলায়, (গ) test করতে infra লাগছে।
**একটাও না ঘটলে তুমি শুধু একটা ফাইলকে চারটা বানিয়েছ।**

এবং সাবধান — ভুল দিকেও যাওয়া যায়। এটা **cohesion ধ্বংস**, উন্নতি না:

```csharp
public class TaxYearValidator   { }   // ১ লাইনের class
public class TaxAmountValidator { }   // ১ লাইনের class
public class TaxCountryValidator{ }   // ১ লাইনের class
```

তিনটা নিয়ম একই সাথে, একই কারণে (compliance) বদলায় — **তাই একসাথেই থাকা উচিত**।
এক-method class এর ছড়াছড়ি low cohesion এরই আরেক চেহারা: সম্পর্কিত জিনিস ছড়িয়ে ফেলা।

> **আসল দক্ষতা "সবকিছু ভাগ করা" না — কোন রেখায় কাটতে হবে সেটা চেনা।
> রেখাটা টানো "কে বদলাতে বলে" বরাবর, "কোডটা কী করে" বরাবর না।**

Junior জিজ্ঞেস করে "এটা কি ভাগ করা উচিত?" — architect জিজ্ঞেস করে
**"এই দুইটা কি কখনো আলাদা কারণে বদলাবে?"** না হলে ওরা একসাথে থাকুক।

---

## 9. Real-world usage — তোমার Orbitax stack

তুমি **প্রতিদিন** এই দুইটা মাপকাঠির উপর দাঁড়িয়ে কাজ করছ, খেয়াল করোনি হয়তো:

- **Clean Architecture এর layer গুলো** = জোর করে coupling এর দিক ঠিক করে দেওয়া।
  Domain কাউকে চেনে না, Infrastructure সবাইকে চেনে। তীরচিহ্ন সবসময় ভেতরের দিকে —
  ঠিক ওই "নিচের দিকে নির্ভর করো" নিয়মটাই, project reference দিয়ে বাধ্য করা।
- **MediatR handler** = cohesion এর ইউনিট। এক handler, এক use case, এক কারণে বদলায়।
  যেদিন handler টা ৩০০ লাইন হয়ে যায় — ওটা আজকের রোগ ১, ফিরে এসেছে।
  *(Day 9 এ ঠিক এই কাজটাই করব, একটা আসল handler নিয়ে।)*
- **FluentValidation** = validation কে handler থেকে বের করে আনা — আজকের ধাপ ১ এর
  ready-made রূপ। `Validate()` এর জন্য কোনো DB লাগে না, তাই ওটা একা test হয়।
- **Pipeline behaviour** = cross-cutting কাজ (logging, retry, validation) কে
  business handler থেকে সরানো, যেন handler এর cohesion না ভাঙে। *(Day 50 — Chain of Responsibility.)*
- **Polly** = "SMTP down মানে filing fail" সমস্যাটার উত্তর, কিন্তু ওটা কাজ করে
  কারণ resilience টা call-site এ ছড়ানো নেই। *(Day 37 — Decorator.)*

**আজকের শিকার (এটাই hands-on এর অর্ধেক):**
তোমার repo তে সবচেয়ে বড় handler টা খুঁজে বের করো। কাগজে ওর কাজগুলো তালিকা করো,
আর প্রতিটার পাশে লেখো **কে ওটা বদলাতে বলবে**। দুইয়ের বেশি আলাদা নাম এলে —
তুমি আজকের bad example টা নিজের codebase এ খুঁজে পেয়েছ।

---

## 10. Key takeaway

> **একসাথে যেগুলো বদলায় তাদের একসাথে রাখো (cohesion);
> যা তোমার চেয়ে দ্রুত বদলায় তার দিকে সরাসরি তাকিয়ো না (coupling)।
> আর মনে রেখো — coupling শূন্য করা যায় না, শুধু ভঙ্গুর থেকে স্থিতিশীলের দিকে সরানো যায়।**

---

## 11. Hands-on exercise — আজ রাতে নিজে হাতে

কাজ করো `HandsOnPractice/CouplingCohesion/` project এ (`Bad Example/` + `Good Example/`),
আর প্রতিটা উত্তর `journey/code/day-06/notes.md` তে লেখো।

1. **উপরের `FilingService` টা হুবহু টাইপ করো** — খারাপ রূপটাই। copy-paste করো না;
   হাতে টাইপ করলে ওটা কত কিছু চেনে সেটা আঙুলে টের পাওয়া যায়।
2. **গোনো:** (ক) `Submit()` কয়টা আলাদা কাজ করে? (খ) কয়টা বাইরের নাম চেনে?
   (গ) এই class টা বদলাতে বলার ক্ষমতা কয়জন মানুষের আছে?
3. **Test টা লেখার চেষ্টা করো** — শুধু BD এর 25% যাচাই করতে, bad version এর
   বিরুদ্ধে। **যেখানে আটকে যাবে সেই মুহূর্তটা লিখে রাখো।** ওটাই আজকের পাঠ।
4. **ধাপ ১ চালাও** — শুধু `FilingValidator` আর `TaxCalculator` বের করো, আর কিছু না।
   এখন test টা আবার লেখো। কত লাইন লাগল?
5. **ধাপ ২ চালাও** — `IFilingStore` আর `INotifier` যোগ করো, দুইটা করে
   implementation লেখো (`SqlFilingStore` / `InMemoryFilingStore`,
   `EmailNotifier` / `NullNotifier`)। `FilingService` এর ভেতরে **একটা লাইনও
   বদলাতে হয়েছে কি?** উত্তরটা লিখে রাখো।
6. **উল্টো দিকটাও দেখো:** `FilingValidator` কে তিনটা এক-নিয়মের class এ ভাঙো।
   এখন নতুন একটা নিয়ম যোগ করতে কয়টা file ছুঁতে হয়? **এটা কি উন্নতি?**
   এক লাইনে যুক্তি দাও।
7. **Orbitax hunt:** সবচেয়ে বড় handler টার কাজ + "কে বদলাতে বলে" এর তালিকা।
   *(এটা Day 9 এ লাগবে — ফেলে দিও না।)*

**Stretch (ঐচ্ছিক):** `dotnet build` এর পর তোমার Domain project এ
`using Microsoft.Data.SqlClient` বা কোনো infra namespace আছে কিনা খোঁজো।
পেলে — সেটা layer এর তীরচিহ্ন উল্টো দিকে যাওয়ার প্রমাণ।

---

## 12. আগামীকাল

**Day 7 — Retrieval day.** নতুন কিছু না; Day 1–6 এর উপর প্রশ্ন উপরে,
উত্তর নিচে, নিজে পরীক্ষা দেবে। আজকের notes.md পূরণ করে রাখলে কালকের দিনটা
অনেক বেশি কাজে দেবে।
