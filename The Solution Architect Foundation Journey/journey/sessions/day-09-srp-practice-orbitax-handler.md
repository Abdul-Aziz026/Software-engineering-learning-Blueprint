# Day 9 of 90 — SRP practice: একটা আসল MediatR handler কাটা

**Block:** Month 1 (Foundation: OOD + SOLID) · Week 2 (SOLID — first three)
**Date:** 2026-08-18

আজ নতুন কোনো principle নেই। আজ Day 8-এর কাঁচিটা নিয়ে আমরা **আসল কোডে** ঢুকছি।

আর এখানেই একটা সমস্যা আছে, যেটা আগে স্বীকার করে নিই:

> Day 8-এ actor-রা **নাম লেখানো ছিল** — `CalculatePay` পাশে লেখা ছিল "Finance",
> `Save` পাশে "DBA"। আসল কোডে কেউ নাম লিখে রাখে না।
> ৯০ লাইনের একটা handler-এ actor-রা **অদৃশ্য**। তোমাকে ওদের *খুঁজে বের করতে* হবে।

তাই আজকের deliverable কোনো তত্ত্ব না — আজকের deliverable একটা **procedure**।
চার ধাপের একটা audit, যেটা তুমি যেকোনো handler-এর উপর চালাতে পারবে, আজ থেকে
career-এর শেষ দিন পর্যন্ত।

> **⚠️ একটা assumption note:** আমার কাছে তোমার Orbitax repo-র access নেই, তাই
> নিচের handler টা আমি **প্রতিনিধিত্বমূলক** (representative) করে লিখেছি — Clean
> Architecture + MediatR + MongoDB + SignalR + GIR XML, মানে হুবহু তোমার stack-এর
> আকৃতিতে। এটা তোমার কোড না, কিন্তু এটা তোমার কোডের **আকার**। আসল handler-এ audit
> চালানোটাই আজকের ১১ নম্বর কাজ — সেটা তোমাকেই করতে হবে, আর ওটাই আসল পাঠ।

---

## 1. Goal

আজকের শেষে তুমি যেকোনো handler খুলে **চার ধাপে** বলতে পারবে — কোন লাইন কার,
কোনটা কোথায় যাবে, আর কেটে ফেলার পরে **handler-এ কী থেকে যাওয়া উচিত**।
আর সবচেয়ে জরুরি জিনিসটা বুঝবে: *কেন ৪টা dependency থাকা handler-ও নিখুঁতভাবে
SRP মানতে পারে।*

---

## 2. ❌ The Bad Version — একটা god handler

এটা দেখতে খারাপ **লাগবেও না**। এটা কাজ করে, test আছে (হয়তো), production-এ চলছে।
এটাই আজকের ফাঁদ — Day 8-এর `Employee` ছিল ৩০ লাইন, এটা ৬০, আর এই ৬০ লাইনই
বাস্তবতা:

```csharp
public class CreateFilingCommandHandler
    : IRequestHandler<CreateFilingCommand, CreateFilingResult>
{
    private readonly IMongoCollection<Filing> _filings;
    private readonly IMongoDatabase _db;
    private readonly IHubContext<FilingHub> _hub;
    private readonly SmtpClient _smtp;
    private readonly ILogger<CreateFilingCommandHandler> _logger;

    public async Task<CreateFilingResult> Handle(
        CreateFilingCommand cmd, CancellationToken ct)
    {
        _logger.LogInformation("Creating filing for TIN {Tin}", cmd.Tin);

        // ── validation ────────────────────────────────────────────────
        if (string.IsNullOrWhiteSpace(cmd.Tin))
            throw new ArgumentException("TIN is required");
        if (cmd.Tin.Length != 9)
            throw new ArgumentException("TIN must be 9 digits");
        if (cmd.PeriodYear < 2020)
            throw new ArgumentException("Period too old to file");

        // ── tax calculation ───────────────────────────────────────────
        decimal taxable = cmd.Revenue - cmd.Deductions;
        if (taxable < 0) taxable = 0;
        decimal tax = taxable * 0.15m;
        if (cmd.Jurisdiction == "IE") tax = taxable * 0.125m;   // Irish rate
        if (cmd.Jurisdiction == "HU") tax = taxable * 0.09m;    // Hungarian rate

        // ── mapping ───────────────────────────────────────────────────
        var filing = new Filing
        {
            Tin          = cmd.Tin,
            PeriodYear   = cmd.PeriodYear,
            Jurisdiction = cmd.Jurisdiction,
            Revenue      = cmd.Revenue,
            Deductions   = cmd.Deductions,
            Tax          = tax,
            CreatedAt    = DateTime.Now,          // 🐛 UTC না
            Status       = "Draft"
        };

        // ── GIR XML payload ───────────────────────────────────────────
        var xml = new XDocument(
            new XElement("GIR",
                new XElement("TIN", filing.Tin),
                new XElement("Period", filing.PeriodYear),
                new XElement("TaxDue", tax.ToString("F2"))));
        filing.Payload = xml.ToString();

        // ── persistence ───────────────────────────────────────────────
        await _filings.InsertOneAsync(filing, cancellationToken: ct);

        // ── audit trail ───────────────────────────────────────────────
        await _db.GetCollection<BsonDocument>("audit").InsertOneAsync(
            new BsonDocument {
                { "action", "FilingCreated" },
                { "tin",    filing.Tin      },
                { "at",     DateTime.Now    }
            }, cancellationToken: ct);

        // ── realtime UI push ──────────────────────────────────────────
        await _hub.Clients.All.SendAsync("FilingCreated", filing.Id, ct);

        // ── email ─────────────────────────────────────────────────────
        _smtp.Send(new MailMessage(
            "noreply@orbitax.com", cmd.UserEmail,
            "Filing created", $"Your tax due is {tax:C}"));

        _logger.LogInformation("Filing {Id} created", filing.Id);
        return new CreateFilingResult(filing.Id, tax);
    }
}
```

থামো। কোড পড়া বন্ধ করো, আর নিজেকে একটা প্রশ্ন করো:

> **"এই handler টা বদলাতে বলার ক্ষমতা কতজন মানুষের আছে?"**

তোমার প্রথম উত্তর সম্ভবত "৩-৪ জন"। আসল উত্তর নিচে, আর সংখ্যাটা তোমাকে চমকে দেবে।

---

## 3. Why it is bad — অদৃশ্য actor-দের বের করার procedure

এটাই আজকের পুরো জিনিস। মুখস্থ করার মতো না — **চালানোর** মতো।

### 🔧 THE SRP AUDIT — চার ধাপ

#### STEP 1 — প্রতিটা block-এর পাশে *কাজের নাম* লেখো

কোড লাইন গুনো না, **কাজ** গুনো। উপরে আমি comment দিয়ে সীমা এঁকে দিয়েছি — তোমার
আসল handler-এ comment থাকবে না, তোমাকে নিজে আঁকতে হবে। একটা কাজ শেষ হয়েছে বুঝবে
কীভাবে? **যখন পরের লাইনটা অন্য জগতের শব্দ ব্যবহার করে।** `taxable`, `deductions`
→ tax-এর জগৎ। `InsertOneAsync`, `BsonDocument` → database-এর জগৎ। জগৎ বদলালেই
সীমানা।

#### STEP 2 — প্রতিটা কাজের পাশে *actor* আর *ঘড়ি* লেখো

Day 8-এর table, দুইটা নতুন কলাম নিয়ে:

| # | কাজ | কে বদলাতে বলে (actor) | কত ঘন ঘন বদলায় (clock) |
|---|---|---|---|
| 1 | logging | **Ops / SRE** | দুই বছরে একবার |
| 2 | TIN + period validation | **Compliance** | নতুন jurisdiction এলেই |
| 3 | tax rate + taxable base | **Tax / Product** | 💥 প্রতি বাজেটে, প্রতি দেশে |
| 4 | DTO → entity mapping | **এই feature-এর owner** | field যোগ হলে |
| 5 | GIR XML payload | **OECD schema** (বাইরের কেউ!) | schema version এলে |
| 6 | Mongo insert | **Infra / DBA** | storage বদলালে |
| 7 | audit trail | **Legal / Audit** | audit policy বদলালে |
| 8 | SignalR push | **Frontend team** | UI event বদলালে |
| 9 | email | **Comms / Support** | template বদলালে |

**নয়টা কাজ। আটটা আলাদা actor। একটা `Handle()` method।**

এখন Day 8-এর প্রশ্নটা এই table-এ চালাও: *একজনের অনুরোধে আরেকজনের আউটপুট নীরবে
বদলে যেতে পারে?* হ্যাঁ — এবং এখানে সেটা **আরও খারাপ**, কারণ লাইন ৩-এর `tax`
variable টা লাইন ৫ (XML), লাইন ৬ (DB), লাইন ৯ (email) — তিন জায়গায় ঢুকেছে।
Tax টিম rounding বদলাল ⇒ OECD-র কাছে যাওয়া XML বদলে গেল। **কেউ OECD-কে জিজ্ঞেস
করেনি।**

**Analogy:** এটা একটা বাড়ি না, এটা একটা **করিডোর** যেটা আটটা বিভাগের অফিসের
ভেতর দিয়ে গেছে, আর প্রতিটা বিভাগ করিডোরের দেয়ালে নিজের নোটিশ টাঙিয়েছে।
কেউ দেয়াল রং করতে চাইলে আট জায়গায় অনুমতি লাগে। করিডোরের নিজের কোনো মালিক নেই —
এবং **মালিকহীন জিনিসই সবচেয়ে দ্রুত নোংরা হয়।**

#### STEP 3 — প্রতিটা কাজকে *গন্তব্য* দাও (আজকের নতুন অস্ত্র)

Day 8-এ কাঁচি চালিয়ে আমরা তিনটা class পেয়েছিলাম — সব পাশাপাশি। আসল architecture-এ
টুকরোগুলো **পাশে যায় না, বিভিন্ন দিকে যায়**। মাত্র চারটা গন্তব্য আছে:

| গন্তব্য | কোন কাজ যায় | 🔑 যে প্রশ্নে চেনা যায় | তোমার stack-এ |
|---|---|---|---|
| **⬇️ Domain** | business rule, calculation, invariant | *"Web API আর MongoDB মুছে দিলেও কি এই নিয়মটা সত্য থাকবে?"* হ্যাঁ ⇒ Domain | `TaxCalculator`, `Filing` entity |
| **⬇️ Infrastructure** | DB, SMTP, SignalR, file, HTTP | *"এটা test করতে একটা মেশিন লাগে?"* হ্যাঁ ⇒ Infra | Repository, `INotifier` |
| **⬆️ Pipeline (cross-cutting)** | logging, retry, transaction, validation, timing, audit | *"এই একই কোড কি **প্রতিটা** handler-এ থাকত?"* হ্যাঁ ⇒ Pipeline | MediatR behaviour, FluentValidation, Polly |
| **➡️ Handler-এ থেকে যায়** | ধাপের **ক্রম** — use case টা নিজে | *"এটা সরালে feature-টাই বদলে যায়?"* হ্যাঁ ⇒ থাকুক | `Handle()` |

ওই তৃতীয় সারির প্রশ্নটা আজকের সবচেয়ে ধারালো বাক্য, একবার আলাদা করে পড়ো:

> **যদি এই কোডটা প্রতিটা handler-এ থাকত, তাহলে এটা কোনো handler-এই থাকা উচিত না।**

Logging প্রতিটা handler-এ থাকবে। Retry প্রতিটা handler-এ থাকবে। Audit প্রতিটা
command-এ থাকবে। এগুলো handler-এর কাজ না — এগুলো **handler-কে ঘিরে থাকা** কাজ।
এই কারণেই এদের নাম cross-**cutting**: এরা সব handler-কে আড়াআড়ি কাটে। *(এদের
জন্য যে জিনিসটা বানানো হয় — MediatR pipeline behaviour — সেটার pattern-নাম
Chain of Responsibility, Day 50।)*

#### STEP 4 — যা থেকে গেল, সেটা পড়ো

কাটার পরে যা অবশিষ্ট থাকল সেটা এক নিঃশ্বাসে পড়ে দেখো। যদি পড়তে শোনায়
*"filing বানাও, tax বসাও, payload বানাও, সংরক্ষণ করো, জানিয়ে দাও"* — অর্থাৎ
**feature-এর গল্প** — তাহলে কাঁচি ঠিক জায়গায় বসেছে।
যদি এখনো `0.15m` বা `BsonDocument` চোখে পড়ে — কাঁচি এখনো বাকি।

---

## 4. Problems it causes

1. **আট actor, এক merge queue** — Tax টিম, frontend টিম আর compliance টিম একই
   file-এ PR দিচ্ছে। এক sprint-এই conflict।
2. **Unit test অসম্ভব** — `0.125m` টা ঠিক কিনা যাচাই করতে তোমার লাগবে একটা Mongo,
   একটা SMTP, একটা SignalR hub। *(Day 6-এর ruler: "test করা কঠিন" কোনো testing
   অভিযোগ না, এটা design diagnosis।)*
3. **আংশিক ব্যর্থতা** — Mongo-তে insert হয়ে গেছে, তারপর SMTP timeout ⇒ exception ⇒
   caller ভাবল ব্যর্থ, কিন্তু filing database-এ বসে আছে। **email পাঠানোর ব্যর্থতা
   একটা filing-কে ব্যর্থ করে দিচ্ছে।** এই দুইটা জিনিসের importance এক না, কিন্তু
   একই `try` ব্লকে আছে।
4. **`0.15m` খুঁজে পাওয়া যাবে না** — নতুন jurisdiction যোগ করতে গেলে কেউ জানে না
   rate কয়টা handler-এ ছড়িয়ে আছে। *(কাল Day 10 — OCP — এই `if`-এর সিঁড়িটাই
   আমাদের বিষয়।)*
5. **DateTime.Now** — এই bug টা এখানে *লুকিয়ে থাকতে পেরেছে* কারণ handler টা এত
   ভিড়। ৬ লাইনের handler হলে reviewer এটা প্রথম চোখে ধরত। **God class শুধু
   পরিবর্তন কঠিন করে না, bug লুকানোর জায়গাও দেয়।**

---

## 5. ✅ The Good Version — সবচেয়ে সহজ সঠিক রূপ

STEP 3-এর table চালানোর পরে handler-এ যা থাকে:

```csharp
public class CreateFilingCommandHandler
    : IRequestHandler<CreateFilingCommand, CreateFilingResult>
{
    private readonly ITaxCalculator   _tax;       // ⬇️ domain
    private readonly IGirXmlBuilder   _xml;       // ⬇️ domain (OECD schema)
    private readonly IFilingRepository _repo;     // ⬇️ infra
    private readonly IFilingNotifier  _notifier;  // ⬇️ infra

    public CreateFilingCommandHandler(
        ITaxCalculator tax, IGirXmlBuilder xml,
        IFilingRepository repo, IFilingNotifier notifier)
    {
        _tax = tax; _xml = xml; _repo = repo; _notifier = notifier;
    }

    public async Task<CreateFilingResult> Handle(
        CreateFilingCommand cmd, CancellationToken ct)
    {
        var filing = Filing.Draft(
            cmd.Tin, cmd.PeriodYear, cmd.Jurisdiction,
            cmd.Revenue, cmd.Deductions);

        filing.SetTax(_tax.Calculate(filing));
        filing.SetPayload(_xml.Build(filing));

        await _repo.AddAsync(filing, ct);
        await _notifier.FilingCreated(filing, ct);

        return new CreateFilingResult(filing.Id, filing.Tax);
    }
}
```

আর টুকরোগুলো:

```csharp
// ⬇️ DOMAIN — invariant entity-র ভেতরে (Day 1 + Day 8)
public class Filing
{
    public string Id           { get; private set; }
    public string Tin          { get; }
    public int    PeriodYear   { get; }
    public string Jurisdiction { get; }
    public decimal Revenue     { get; }
    public decimal Deductions  { get; }
    public decimal Tax         { get; private set; }
    public string  Payload     { get; private set; }

    private Filing(string tin, int year, string jur, decimal rev, decimal ded)
    {
        // অস্তিত্বের শর্ত — কোনো বিভাগের মতামত না
        if (string.IsNullOrWhiteSpace(tin)) throw new ArgumentException("TIN required");
        if (rev < 0 || ded < 0) throw new ArgumentException("amounts cannot be negative");
        Tin = tin; PeriodYear = year; Jurisdiction = jur;
        Revenue = rev; Deductions = ded;
    }

    public static Filing Draft(string tin, int year, string jur,
                               decimal rev, decimal ded)
        => new Filing(tin, year, jur, rev, ded);

    public decimal TaxableBase => Math.Max(0m, Revenue - Deductions);

    public void SetTax(decimal tax)     => Tax = tax;
    public void SetPayload(string xml)  => Payload = xml;
}

// ⬇️ DOMAIN — Tax টিমের নিজের ঘর। এখানে Mongo-র নাম নেই, SMTP-র নাম নেই।
public class TaxCalculator : ITaxCalculator
{
    public decimal Calculate(Filing f) => f.Jurisdiction switch
    {
        "IE" => f.TaxableBase * 0.125m,
        "HU" => f.TaxableBase * 0.09m,
        _    => f.TaxableBase * 0.15m
    };
}

// ⬇️ INFRA — একটাই কাজ: filing রাখা
public class MongoFilingRepository : IFilingRepository { /* InsertOneAsync */ }

// ⬇️ INFRA — একটাই কাজ: "হয়ে গেছে" খবরটা মানুষের কাছে পৌঁছানো
public class FilingNotifier : IFilingNotifier { /* SignalR + email */ }
```

আর **যে তিনটা জিনিস handler থেকে একেবারে উঠে গেল** (এরা পাশে যায়নি, উপরে গেছে):

```csharp
// ⬆️ compliance-এর নিজের file — FluentValidation
public class CreateFilingCommandValidator : AbstractValidator<CreateFilingCommand>
{
    public CreateFilingCommandValidator()
    {
        RuleFor(x => x.Tin).NotEmpty().Length(9);
        RuleFor(x => x.PeriodYear).GreaterThanOrEqualTo(2020);
    }
}

// ⬆️ ops actor — একবার লেখা, সব handler-এ কাজ করে
public class LoggingBehaviour<TReq, TRes> : IPipelineBehavior<TReq, TRes> { /* ... */ }

// ⬆️ legal/audit actor — একবার লেখা, সব command-এ কাজ করে
public class AuditBehaviour<TReq, TRes> : IPipelineBehavior<TReq, TRes> { /* ... */ }
```

লক্ষ্য করো: `LoggingBehaviour` লিখতে হলো **একবার**, আর এটা তোমার ৪০টা handler-এ
কাজ করে। handler-এ logging রাখলে ওই কোড **৪০ বার** থাকত। এটা DRY-র কথা না —
এটা এই কথা যে ops actor-এর সুইচ **একটাই** হওয়া উচিত।

---

## 6. What changed and why

| আগের যন্ত্রণা | এখন |
|---|---|
| `0.125m` test করতে Mongo + SMTP লাগত | `new TaxCalculator().Calculate(filing)` — শূন্য infra |
| SMTP fail করলে filing fail | notify আলাদা; পরে চাইলে fire-and-forget বা outbox করা যায় |
| Tax টিমের rounding বদলালে OECD-র XML বদলে যেত | `TaxCalculator` আর `GirXmlBuilder` আলাদা সুইচ |
| ৮ actor এক file-এ PR দিত | প্রতিটা actor-এর নিজের file |
| logging ৪০ handler-এ ছড়ানো | একটা behaviour |
| `DateTime.Now` bug ভিড়ে লুকিয়ে ছিল | ৭ লাইনের handler — লুকানোর জায়গা নেই |

---

## 7. Bad vs Good — পাশাপাশি

| | ❌ Bad | ✅ Good |
|---|---|---|
| `Handle()` দৈর্ঘ্য | ~৫০ লাইন | ৭ লাইন |
| আলাদা actor handler-এ | ৮ | **১** |
| Handler কী *সিদ্ধান্ত* নেয় | tax rate, validation, schema, storage, delivery | কিছুই — শুধু **ক্রম** |
| Handler test করতে লাগে | Mongo, SMTP, SignalR, Logger | ৪টা mock |
| Tax rate বদলাতে কোন file? | handler (৮ actor-এর দেশ) | `TaxCalculator` (Tax টিমের ঘর) |
| Constructor dependency | ৫ | ৪ |

ওই শেষ সারিটা ইচ্ছে করে রেখেছি। **Dependency সংখ্যা কমেনি** — ৫ থেকে ৪। তবু
এটা বিশাল উন্নতি। কেন? সেটাই পরের section, আর সেটাই আজকের আসল পাঠ।

---

## 8. Architect's reasoning

### 💡 আজকের সবচেয়ে দামি বাক্য: call গুনো না, *decision* গুনো

ভালো handler টাও ৪টা জিনিস ডাকে। তাহলে ওটার ৪টা responsibility?

**না।** এবং এই পার্থক্যটা না বুঝলে তুমি SRP-র নামে সারাজীবন ভুল জায়গায় কাঁচি চালাবে:

> **৪টা জিনিসের উপর নির্ভর করা ≠ ৪ জনের কাছে জবাবদিহি করা।**
> SRP গোনে class টা কতগুলো **সিদ্ধান্ত নেয়**, কতগুলো **call করে** সেটা না।

প্রমাণ — প্রতিটা actor-কে একটা করে পরিবর্তন করতে দাও, আর দেখো handler-এ হাত পড়ে কি:

| কে কী চাইল | কোন file বদলাবে | Handler ছুঁতে হলো? |
|---|---|---|
| Tax টিম: Irish rate 12.5% ⇒ 15% | `TaxCalculator` | ❌ না |
| OECD: নতুন schema element | `GirXmlBuilder` | ❌ না |
| Infra: Mongo ⇒ PostgreSQL | `FilingRepository` | ❌ না |
| Compliance: TIN এখন ১০ digit | `Validator` | ❌ না |
| Ops: log format বদল | `LoggingBehaviour` | ❌ না |
| Product: "সংরক্ষণের **আগে** approval লাগবে" | **Handler** | ✅ **হ্যাঁ** |

শেষ সারিটাই উত্তর। Handler বদলায় **শুধু একটা কারণে** — যখন *ধাপগুলো নিজে* বদলায়।
আর সেটা চায় একজনই: **এই use case-এর owner**। এক actor। SRP ✅।

### 💡 Orchestration নিজেই একটা responsibility — শূন্যতা না

জুনিয়রদের সবচেয়ে সাধারণ আপত্তি: *"handler-এ তো কিছুই রইল না, এটা শুধু pass-through!"*

ভুল। যা রইল সেটা হলো **ক্রম** — কোন ধাপের পরে কোন ধাপ, কোনটা আগে হতেই হবে,
কোনটা ব্যর্থ হলে থামবে। এটা তুচ্ছ জ্ঞান না; এটাই **use case-টা কী** তার সংজ্ঞা।
XML বানানোর আগে tax বসাতেই হবে — এই জ্ঞানটা `TaxCalculator`-এর নেই,
`GirXmlBuilder`-এরও নেই। এই জ্ঞান handler-এর, একমাত্র handler-এর।

> **Coordinator একটা ভূমিকা, ভূমিকার অভাব না।**
> একজন conductor কোনো বাদ্যযন্ত্র বাজান না। তাতে তাঁর কাজ শূন্য হয়ে যায় না।

### ⚠️ ফাঁদ ১ — "সরানো" আর "ভাগ করা" এক জিনিস না

সবচেয়ে সাধারণ ভুয়া refactor:

```csharp
// ❌ এটা refactor না, এটা নাম বদল
public async Task<CreateFilingResult> Handle(cmd, ct)
    => await _filingService.CreateFiling(cmd, ct);   // ৫০ লাইন এখন এখানে
```

god class মরেনি, **ঠিকানা বদলেছে**। এখনো আট actor এক file-এ, শুধু file-এর নাম
`FilingService.cs`। যাচাইয়ের প্রশ্ন: **"actor সংখ্যা কমল কি?"** — লাইন সংখ্যা না,
actor সংখ্যা। না কমলে তুমি শুধু আসবাব সরিয়েছ।

### ⚠️ ফাঁদ ২ — সব handler-এ এটা করা over-engineering

সৎ থাকি। এই audit-টা এক ঘণ্টার কাজ, আর তোমার repo-তে ৪০টা handler আছে। **সব
handler-এ চালানো লাভজনক না।** Trigger তিনটা, আর এদের কোনোটাই "লাইন বেশি" না:

1. **এই handler কি ঘন ঘন বদলায়?** (git log দেখো — ৬ মাসে ২০টা commit ⇒ হ্যাঁ)
2. **এর কি একাধিক PR author আছে আলাদা টিম থেকে?** (আট actor-এর ছাপ git-এ পড়ে)
3. **এটা test করতে কি infra লাগে?**

তিনটার একটাও না হলে — একটা ২০ লাইনের handler যেটা শুধু একটা document insert করে,
বছরে একবার বদলায় — **ওটা ছুঁয়ো না।** ৪টা interface বানিয়ে ওটাকে "ঠিক" করা
মানে তুমি ভবিষ্যতের একটা কল্পিত সমস্যার জন্য আজকের পড়ার খরচ বাড়ালে।

> **Refactor-এর trigger পরিবর্তনের হার, কোডের সৌন্দর্য না।**
> যে কোড বদলায় না, তার design নিয়ে কারো মাথা ব্যথা হয় না।

### 🤔 একটা সৎ judgment call — email আর SignalR কি এক actor?

আমি দুইটাকে এক `IFilingNotifier`-এর পিছনে রেখেছি, আর এটা তর্কসাপেক্ষ। যুক্তি:
আজ দুইটাই একই বাক্যের উত্তর — *"মানুষকে জানাও যে filing হয়ে গেছে"*, আর দুইটাই
একসাথে বদলায়। কিন্তু যেদিন frontend টিম নতুন event field চাইবে আর support টিম
email template বদলাবে — **আলাদা ঘড়ি, আলাদা অনুরোধ** — সেদিন ভাঙবে।
*(Day 6-এর restraint: interface শুধু ওখানে যেখানে সত্যিকারের বিকল্প আছে। আগেভাগে
ভাঙলে সেটা আবার তিনটা এক-লাইনের validator class।)*

**এই ধরনের প্রশ্নের একটাই সঠিক উত্তর নেই — এবং trade-off টা *বলতে* পারাটাই
architect হওয়া।** "জানি না" আর "দুই দিকের দাম জানি, আজ এই দিকটা বেছেছি এই কারণে" —
এই দুইটার মধ্যে পুরো career-টা।

---

## 9. Real-world usage — তোমার Orbitax stack

এই map টা মুখস্থ করার মতো, কারণ আজকের চার গন্তব্য তোমার stack-এ **আগে থেকেই
বানানো আছে** — তুমি শুধু ব্যবহার করছ না:

| গন্তব্য | তোমার stack-এ যন্ত্রটা |
|---|---|
| ⬆️ validation | **FluentValidation** + `ValidationBehaviour` |
| ⬆️ logging / timing / correlation-id | **MediatR pipeline behaviour** |
| ⬆️ retry / circuit breaker | **Polly** *(কেন এটা handler-এ ঢুকে না — Decorator, Day 37)* |
| ⬆️ audit trail | **`AuditBehaviour`** (বা domain event) |
| ⬇️ persistence | **MongoDB repository** |
| ⬇️ realtime | **SignalR hub** — `IHubContext` handler-এ ঢুকলে UI actor ঢুকে পড়ল |
| ⬇️ GIR XML | **schema builder** — OECD একটা বাইরের actor, ওর জন্য নিজের ঘর |
| ➡️ ক্রম | **handler** |

> **Clean Architecture-এর layer-গুলো আসলে actor-দের ঠিকানা।** Domain, Application,
> Infrastructure — এগুলো "কোড সাজানোর ফোল্ডার" না, এগুলো *"কে কার কাছে জবাবদিহি
> করে"* সেটার মানচিত্র। এই কারণেই Domain থেকে Infrastructure-এ reference থাকা
> নিষিদ্ধ: tax নিয়মকে database-এর অনুমতির জন্য অপেক্ষা করতে হবে না। *(Day 6-এর
> "depend downward" — project reference দিয়ে compiler-এ প্রয়োগ করা।)*

**আজকের শিকার (এটাই আজকের আসল কাজ):**

তোমার সবচেয়ে বড় handler টা খোলো — Day 6-এর ৭ নম্বর hunt-এ যেটা পেয়েছিলে,
আর Day 8-এ যেটার `using` list গুনেছিলে। ওটার উপর চার ধাপের audit চালাও।
আমি একটা নির্দিষ্ট সংখ্যা শুনতে চাই: **কতজন actor?**

---

## 10. Key takeaway

> **God class ঠিক করার নিয়মটা "ছোট করো" না — নিয়মটা হলো *প্রতিটা কাজকে তার
> মালিকের ঠিকানায় পাঠাও*: rule নামে Domain-এ, machine নামে Infrastructure-এ,
> "সব handler-এ থাকবে" জিনিস উপরে Pipeline-এ। যা থেকে যায় সেটা হলো **ক্রম** —
> আর ক্রম রক্ষা করাটা শূন্যতা না, ওটাই handler-এর একমাত্র responsibility।
> তাই class-এর dependency গুনে বিচার কোরো না; গোনো কতগুলো **সিদ্ধান্ত** ও নিজে
> নেয়। ৪টা জিনিস ডাকা handler SRP মানতে পারে; ২টা রেট জানা handler পারে না।**

---

## 11. Hands-on exercise — আজ রাতে নিজে হাতে

কাজ করো `HandsOnPractice/SingleResponsibility/` project-এ (নতুন ফোল্ডার
`Handler Practice/` বানাও), আর উত্তর লেখো `journey/code/day-09/notes.md`-এ।

### PART A — আমার handler-টা দিয়ে হাত গরম (৩০ মিনিট)

1. **উপরের bad handler টা হাতে টাইপ করো।** MediatR/Mongo install করার দরকার নেই —
   `IRequestHandler`, `IMongoCollection`, `IHubContext` এর জন্য নিজের ২-লাইনের
   fake interface লিখে নাও। **লক্ষ্য কোড চালানো না, লক্ষ্য ভিড়টা নিজের হাতে
   অনুভব করা।**
2. **STEP 1 + 2 চালাও** — notes.md-এ actor + clock table পূরণ করো। আমার উত্তর
   দেখার **আগে** নিজের সংখ্যাটা লিখে ফেলো।
3. **STEP 3 চালাও** — ৯টা কাজের প্রতিটার পাশে চারটা গন্তব্যের একটা লেখো
   (⬇️Domain / ⬇️Infra / ⬆️Pipeline / ➡️Handler)। **audit trail নিয়ে থামো** —
   ওটা কোথায় যাবে, আর দুই দিকের যুক্তি কী?
4. **Good version লেখো** এবং `TaxCalculator`-এর জন্য একটা unit test লেখো যেখানে
   **একটাও mock নেই**। এই test টা আগের design-এ লেখা *সম্ভব ছিল না* — এক লাইনে
   লেখো কেন।

### PART B — actor-সংখ্যা কমল কি? (১৫ মিনিট)

5. **ভুয়া refactor টা নিজে করো:** পুরো ৫০ লাইন `FilingService.CreateFiling()`-এ
   সরাও, handler-কে one-liner বানাও। এখন লিখে রাখো:
   - Handler-এর actor সংখ্যা: ____
   - System-এর মোট actor-বিভ্রান্তি কমল কি? ____
   - এক লাইনে: *"লাইন সরানো refactor না, কারণ ______"*

### PART C — 🎯 আসল কাজ: তোমার নিজের Orbitax handler (৩০ মিনিট)

6. তোমার সবচেয়ে বড় handler-এ **চার ধাপের audit** চালাও। notes.md-এ লেখো:
   - File নাম (বা ছদ্মনাম, যদি লিখতে না চাও)
   - `Handle()` কত লাইন
   - কয়টা কাজ, **কয়জন actor**
   - প্রতিটা কাজের গন্তব্য
   - কাটার পরে handler কত লাইন হতো (আন্দাজ)
7. **git log দেখো** ওই file-এর: `git log --oneline -- <path>` । শেষ ৬ মাসে কয়টা
   commit? কয়জন আলাদা author? **এই সংখ্যাগুলো তোমার actor-tableকে সমর্থন করছে
   কি, না তোমার আন্দাজ ভুল প্রমাণ করছে?** (দুইটাই মূল্যবান উত্তর।)
8. **উল্টো drill (over-engineering ধরা):** তোমার **সবচেয়ে ছোট** handler টা খোঁজো।
   ওটার উপরেও audit চালাও, আর তারপর যুক্তি দাও কেন ওটাকে **ছুঁয়ো না**। তিনটা
   trigger-এর কয়টা ওখানে আছে? *(এই প্রশ্নের উত্তরই তোমাকে "সব কিছু refactor করা
   জুনিয়র" থেকে আলাদা করবে।)*

### Stretch (ঐচ্ছিক)

9. তোমার repo-তে যেকোনো একটা **pipeline behaviour** খুলে পড়ো, আর লেখো: এই কোডটা
   যদি behaviour না হয়ে handler-এ থাকত, তাহলে কয়টা file-এ একই জিনিস লেখা থাকত?
   *(এটা কালকের না, Day 50-এর কাঁচামাল — কিন্তু আজ দেখলে বেশি দাগ কাটবে।)*
10. Bad handler-এর `if (cmd.Jurisdiction == "IE")` সিঁড়িটা মার্ক করে রাখো।
    **কাল সকালে ওটাই আমাদের প্রথম শিকার।**

---

## 12. আগামীকাল

**Day 10 — OCP: open to extension, closed to modification.** আজ আমরা `TaxCalculator`-কে
তার নিজের ঘরে পাঠিয়েছি — কিন্তু ভেতরে ওই `switch` টা রয়ে গেছে। নতুন jurisdiction
এলে আমাদের **কাজ করা, tested, production কোড আবার খুলতে হবে**। SRP বলেছে
*ফাইলটা কোথায় থাকবে*; OCP বলবে *ওটার ভেতরে হাত না দিয়ে কীভাবে বাড়াবে*।
আজকের `switch` টা হাতের কাছে রেখো।
