# Day 4 of 90 — Composition over Inheritance: একই feature দুইভাবে

**Block:** Month 1 (Foundation: OOD + SOLID) · Week 1 (OOP in depth)
**Date:** 2026-08-01

আজকের পুরো পাঠ এক লাইনে:

> **Inheritance এ behaviour টা compile time এ fix হয়ে যায়।
> Composition এ behaviour টা একটা object — মানে ওটা বদলানো যায়।**

Day 3 এ শিখেছ inheritance **কখন ভুল**। আজ দেখবে composition দিয়ে কী কী
**সম্ভব হয়ে যায়** যেটা inheritance এ হতোই না।

---

## 1. The problem first

Report export করতে হবে। CSV আর XML — দুই format।
সবচেয়ে স্বাভাবিক প্রথম solution, আর **এটা আসলে খারাপও না**:

```csharp
public abstract class ReportExporter
{
    public abstract string Export(Report report);
}

public class CsvExporter : ReportExporter
{
    public override string Export(Report report) => /* csv বানাও */ "...";
}

public class XmlExporter : ReportExporter
{
    public override string Export(Report report) => /* xml বানাও */ "...";
}
```

দুইটা class, পরিষ্কার, `is-a` টাও সত্যি। **Day 3 এর তিনটা প্রশ্নেও পাস করে।**
এখানে থেমে গেলে কোনো সমস্যা নেই।

সমস্যা শুরু হয় **পরের requirement** এ।

### ধাক্কা ১ — "export টা কোথায় যাবে সেটাও তো বদলায়"

কোনো report disk এ save হবে, কোনোটা FTP তে যাবে, কোনোটা email এ।
Inheritance দিয়ে চালিয়ে গেলে:

```csharp
public class CsvToDiskExporter  : ReportExporter { }
public class CsvToFtpExporter   : ReportExporter { }
public class CsvToEmailExporter : ReportExporter { }
public class XmlToDiskExporter  : ReportExporter { }
public class XmlToFtpExporter   : ReportExporter { }
public class XmlToEmailExporter : ReportExporter { }   // 💀 ২ × ৩ = ৬
```

কাল JSON যোগ হলো → **৯টা**। পরশু S3 যোগ হলো → **১২টা**।

এবং লক্ষ্য করো — **CSV লেখার code টা তিন জায়গায় copy হয়েছে।
FTP তে পাঠানোর code টাও তিন জায়গায়।**

> **Inheritance এ একটা class এর একটাই parent থাকে — মানে variation এর
> একটাই axis handle করা যায়। দুইটা axis হলেই গুণ হয়ে যায়, যোগ হয় না।**

### ধাক্কা ২ — "user ই তো runtime এ ঠিক করে"

এখন UI তে দুইটা dropdown। User বেছে দেয় format আর destination।
Inheritance এ তোমার লিখতে হবে:

```csharp
ReportExporter exporter = (format, destination) switch
{
    ("csv", "disk")  => new CsvToDiskExporter(),
    ("csv", "ftp")   => new CsvToFtpExporter(),
    ("csv", "email") => new CsvToEmailExporter(),
    ("xml", "disk")  => new XmlToDiskExporter(),
    // ... বাকি ৮টা
};
```

আর সত্যিকারের মারটা এখানে — একটা export চলার মাঝপথে যদি FTP fail করে
আর disk এ fallback করতে হয়?

```csharp
exporter.Destination = new DiskDestination();   // ❌ এমন কিছু নেই
```

**পারবে না।** কারণ destination টা object এর কোনো *অংশ* না —
ওটা object এর **type এর ভেতরে গাঁথা**। আর type runtime এ বদলায় না।

> **Inheritance দিয়ে যা জোড়া লাগাও, সেটা compile time এ ঝালাই হয়ে যায়।**

---

## 2. The idea — analogy

Day 3 এ বলেছিলাম: inheritance = দত্তক নেওয়া, composition = ধার নেওয়া।
আজ analogy টা এক ধাপ এগোই।

**Inheritance হলো জন্মসূত্রে পাওয়া পরিচয়। Composition হলো হাতের যন্ত্র।**

একজন মিস্ত্রি আর তার যন্ত্রপাতি ভাবো।

- **Inheritance এর দুনিয়ায়:** "ড্রিল-মিস্ত্রি", "করাত-মিস্ত্রি", "ড্রিল-ও-করাত-মিস্ত্রি" —
  আলাদা আলাদা মানুষ। কাজ বদলালে **মানুষটাকেই বদলাতে হয়**।
- **Composition এর দুনিয়ায়:** একজন মিস্ত্রি, ব্যাগে যন্ত্র। কাঠ কাটতে হলে করাত বের করে,
  ফুটো করতে হলে ড্রিল। **মানুষ একই, যন্ত্র বদলায়।**

তোমার class টা মিস্ত্রি। যে behaviour গুলো বদলাতে পারে — সেগুলো ওর **যন্ত্র**,
ওর **জাত** না।

**তাই সিদ্ধান্তের প্রশ্নটা:**

> **এই জিনিসটা আমার object টা *কী* — নাকি আমার object টা যা *ব্যবহার করে*?**
> "কী" হলে inheritance ভাবতে পারো। "ব্যবহার করে" হলে — field.

---

## 3. Minimal example — ঠিক করা

যা বদলায়, সেটাকে **object বানাও**, তারপর **ভেতরে রাখো**:

```csharp
// যা বদলায় #১ — format
public interface IReportFormatter
{
    string Format(Report report);
}

// যা বদলায় #২ — destination
public interface IExportDestination
{
    void Send(string content, string fileName);
}
```

```csharp
public class ReportExporter                       // আর abstract না, subclass ও নেই
{
    private IReportFormatter _formatter;
    private IExportDestination _destination;

    public ReportExporter(IReportFormatter formatter, IExportDestination destination)
    {
        _formatter = formatter;
        _destination = destination;
    }

    public void Export(Report report)
    {
        var content = _formatter.Format(report);          // যন্ত্র #১
        _destination.Send(content, report.Name);          // যন্ত্র #২
    }

    // ⭐ আজকের আসল লাইন — inheritance এ এটা লেখাই সম্ভব ছিল না
    public void UseDestination(IExportDestination destination) => _destination = destination;
}
```

এখন গুনে দেখো: ২টা formatter + ৩টা destination = **৫টা ছোট class**,
আর তাতে **৬টা combination** পাওয়া যায়।

**সৎভাবে গুনি, কারণ আমি তোমাকে ঠকাতে চাই না:**

| | Bad | Good |
|---|---|---|
| এখনকার মোট file | ৬ class + ১ abstract base = **৭** | ৫ class + ১ exporter + ২ interface = **৮** |

দেখলে? **আজ, এই মুহূর্তে, composition এ file বেশি।** যে কেউ বলতে পারে "লাভ কী হলো?"
লাভটা আজকের সংখ্যায় না, **কালকের ঢালে**:

| নতুন requirement | Bad এ যোগ হয় | Good এ যোগ হয় |
|---|---|---|
| JSON format | +৩ class | **+১ class** (আর ৩টা combination free) |
| S3 destination | +৩ class (JSON ধরলে +৪) | **+১ class** |
| CSV escaping rule বদল | ৩ জায়গায় edit | **১ জায়গায় edit** |

> **Inheritance এ বৃদ্ধি `M × N` — গুণ। Composition এ `M + N` — যোগ।
> শুরুতে যোগের খরচ বেশি মনে হয়। দুই মাস পরে গুণটা তোমাকে গিলে ফেলে।**

**এটাই architect আর junior এর তফাত:** junior আজকের line count গোনে,
architect ঢালটা (growth rate) দেখে।

আর ধাক্কা ২ এর সেই fallback, যেটা করা যাচ্ছিল না:

```csharp
var exporter = new ReportExporter(new XmlFormatter(), new FtpDestination());

try
{
    exporter.Export(report);
}
catch (IOException)          // FTP গেল না
{
    exporter.UseDestination(new DiskDestination());   // ✅ একই object, নতুন যন্ত্র
    exporter.Export(report);
}
```

**একই object, চলতি অবস্থায় behaviour বদলে গেল।** এটাই composition এর আসল ক্ষমতা —
class কম হওয়াটা শুধু বোনাস।

---

## 4. Bad vs Good — পাশাপাশি

| | Inheritance দিয়ে | Composition দিয়ে |
|---|---|---|
| ২ format × ৩ destination, আজকের file সংখ্যা | ৭ | ৮ *(হ্যাঁ, বেশি)* |
| + JSON format | +৩টা class | **+১টা class** |
| বৃদ্ধির হার | `M × N` (গুণ) | `M + N` (যোগ) |
| CSV লেখার code | ৩ জায়গায় copy | ১ জায়গায় |
| Runtime এ destination বদল | **অসম্ভব** | একটা setter |
| Test এ formatter টা fake করা | পুরো class inherit করতে হবে | fake object পাঠিয়ে দাও |
| Object তৈরি | `new CsvToFtpExporter()` — ছোট | `new ReportExporter(a, b)` — বড় |
| একবার সেট হলে | চিরকালের জন্য | যখন খুশি বদলাও |

**সৎ কথাটা:** composition এ `new` লেখাটা লম্বা হয়, আর একটা extra interface পড়তে হয়।
এটাই দাম। কিন্তু দামটা **একবার**, লাভটা **প্রতিটা নতুন requirement এ**।

---

## 5. "Is there a simpler way?" — আজকের সবচেয়ে জরুরি অংশ

সাবধান। "Composition over inheritance" শুনে অনেকে **সব কিছু** interface দিয়ে
মুড়ে ফেলে। ওটা আরেক রোগ।

উপরের প্রথম version টা মনে করো — শুধু `CsvExporter` আর `XmlExporter`।
**যদি destination কখনো না বদলায়, তাহলে ওই দুইটা subclass ই সঠিক উত্তর।**
তখন interface যোগ করা over-engineering।

**Composition তখন নাও যখন এই তিনটার একটা সত্যি:**

| লক্ষণ | মানে |
|---|---|
| একের বেশি জিনিস স্বাধীনভাবে বদলায় | class explosion আসছে → compose |
| Runtime এ behaviour বদলাতে হতে পারে | inheritance এ অসম্ভব → compose |
| Base এর অনেক member subclass ব্যবহার করে না | ওটা toolbox, parent না → compose |

**তিনটার একটাও না হলে — inheritance রেখে দাও।** কম code, কম indirection।

আর Day 3 এর `TaxFiling` base টা মনে আছে? ওটা এখনো ঠিকই আছে —
কারণ ওর কাজ behaviour বিলি করা না, **একটা নিয়ম জোর করে চাপানো**
(validate ছাড়া submit নেই)। ওটা composition দিয়ে হয় না।

> **নিয়ম চাপাতে হলে inheritance। Behaviour বদলাতে হলে composition।**

*(আর হ্যাঁ — আজ যেটা বানালে, ওর একটা নাম আছে। যা বদলায় সেটাকে object বানিয়ে
ভেতরে রাখা = **Strategy pattern** (Day 45), আর দুইটা axis কে আলাদা করা = **Bridge**
(Day 41)। আজ নাম মুখস্থ করার দরকার নেই — pattern গুলো এই একই চিন্তা থেকেই জন্মেছে,
সেটা টের পাওয়াই আসল।)*

---

## 6. Apply it — তোমার Orbitax stack

আজ **একটাই** কাজ, ১০-১৫ মিনিট। খুঁজে বের করো — **তোমার codebase এ composition
ইতিমধ্যেই সব জায়গায় আছে, তুমি শুধু নাম দাওনি।**

যেকোনো একটা MediatR handler এর constructor খোলো:

```csharp
public class SubmitFilingHandler(
    IFilingRepository repository,          // ← যন্ত্র
    IGirXmlBuilder builder,                // ← যন্ত্র
    INotificationService notifications)    // ← যন্ত্র
```

তিনটা প্রশ্ন করো নিজেকে:

1. এই handler টা কি `BaseHandler` থেকে inherit করে? — **করে না।**
2. তাহলে ওর কাজগুলো কোথা থেকে আসছে? — **constructor দিয়ে ঢোকানো object থেকে।**
3. Test এ `IFilingRepository` কে fake করতে কি handler টা inherit করতে হয়? — **হয় না।**

**পুরো Clean Architecture + DI container জিনিসটাই composition, industrial scale এ।**
`Program.cs` এর `services.AddScoped<...>()` লাইনগুলো আসলে "কোন মিস্ত্রি কোন যন্ত্র পাবে"
তার তালিকা। *(Day 17 এ এটাই DIP নামে ফিরে আসবে।)*

**বোনাস শিকার (৫ মিনিট):** GIR XML generation এ format-বদল আর destination-বদল
কি সত্যিই আলাদা হয়ে আছে, নাকি একটাই class দুইটা কাজ করছে? পেলে notes.md এ লিখে রাখো —
Day 8 (SRP) এ কাজে দেবে।

---

## 7. আজকের hands-on task

`journey/code/day-04/Day04.cs` তে scaffold আছে। **হাতে টাইপ করবে, copy-paste না।**
তোমার `HandsOnPractice/Composition/` project টা তো খালিই পড়ে আছে — ওখানেই লেখো,
`Bad Example/` আর `Good Example/` folder এ।

তিনটা কাজ, এর বেশি না:

1. **Explosion টা নিজের হাতে বানাও।** `Bad Example/` এ ৬টা class লেখো:
   `CsvToDiskExporter`, `CsvToFtpExporter`, `CsvToEmailExporter`, `XmlToDisk...`
   বাকিগুলোও। **ক্লান্ত লাগা পর্যন্ত লেখো — ওই ক্লান্তিটাই আজকের পাঠ।**
   তারপর গোনো: CSV লেখার line গুলো কতবার copy হলো?
2. **Compose করো।** `Good Example/` এ `IReportFormatter` + `IExportDestination` +
   একটা `ReportExporter`। এবার JSON format যোগ করো — **একটা class**, আর ৩টা
   combination চলে এল। Bad Example এ একই কাজ করতে কত লাগত হিসাব করো।
3. **Runtime swap টা চোখে দেখো।** `FtpDestination.Send()` কে জোর করে throw করাও।
   `catch` এ `UseDestination(new DiskDestination())` করে আবার export করো।
   Console এ দেখো একই object টা এখন disk এ লিখছে।
   **Bad Example এ এই কাজটা করার চেষ্টা করো — পারবে না। ওই আটকে যাওয়াটাই আজকের সবচেয়ে দামি মুহূর্ত।**

**সময় থাকলে (optional):**

4. `ReportExporter` কে `readonly` field দিয়ে লেখো (setter ছাড়া) — এতে কী হারালে,
   কী পেলে? *(হিন্ট: Day 1 এর invariant বনাম আজকের নমনীয়তা। দুইটা সত্যিকারের trade-off।)*
5. `notes.md` তে লিখো: আজকের কোন জায়গায় composition **over-engineering** হতো?

---

## 8. One-line self-check

> **নিজের ভাষায় বলো: composition ঠিক কী দেয় যেটা inheritance দিতেই পারে না?**

সহজ উত্তর: **runtime এ behaviour বদলানোর ক্ষমতা**, আর **variation এর একের বেশি axis
যোগ করে (গুণ না করে) সামলানো**। Inheritance এ behaviour টা type এর ভেতরে ঝালাই হয়ে
থাকে — বদলাতে হলে object টাই নতুন বানাতে হয়। Composition এ behaviour একটা field,
আর field বদলানো যায়।

---

## কালকের প্রস্তুতি (Day 5)

**Polymorphism: subtype vs ad-hoc — আর override এর পেছনের vtable এর গল্প।**

আজ তুমি `_formatter.Format(report)` লিখেছ, অথচ জানো না ভেতরে CSV না XML।
কাল দেখবে **CLR টা কীভাবে ঠিক করে কোন method চলবে** — আর কেন `virtual`
লিখতে ভুললে পুরো জিনিসটা নীরবে ভেঙে যায়।

---

*Day 4 of 90 · টার্গেট: "এখন আমার বেসিক শক্তিশালী।"*
