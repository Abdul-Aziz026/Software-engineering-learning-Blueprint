# Day 2 of 90 — Abstraction vs Encapsulation (`IPaymentProcessor`)

**Block:** Month 1 (Foundation: OOD + SOLID) · Week 1 (OOP in depth)
**Date:** 2026-07-27 · Day 2 of 90

কালকে শিখেছি — encapsulation মানে **পাহারা** (কে state বদলাতে পারবে)। আজকে অন্য অক্ষটা।

আজকের এক লাইনের thesis:

> **Encapsulation ঠিক করে — ভেতরটা কে ছুঁতে পারবে। Abstraction ঠিক করে — caller কোন *ধারণা* নিয়ে কাজ করবে। প্রথমটা ভাঙলে data corrupt হয়। দ্বিতীয়টা ভাঙলে পুরো architecture টা corrupt হয়।**

---

## 1. The problem first — abstraction ছাড়া code টা কেমন দেখতে

তোমার একটা invoice service লাগবে, payment নিতে হবে। Stripe দিয়ে শুরু:

```csharp
// ❌ Version 0 — কোনো abstraction নেই
public class InvoiceService
{
    private readonly StripeClient _stripe;

    public async Task PayAsync(Invoice invoice)
    {
        var intent = await _stripe.PaymentIntents.CreateAsync(new PaymentIntentCreateOptions
        {
            Amount   = (long)(invoice.Total * 100),   // Stripe cents চায়
            Currency = "usd",
            Customer = invoice.StripeCustomerId       // 💀 Invoice এখন Stripe এর কথা জানে
        });
        invoice.MarkPaid(intent.Id);
    }
}
```

তিন মাস পর business বলল: "US এ Stripe, Europe এ Adyen, আর একটা enterprise client bank transfer চায়।" তুমি লিখলে:

```csharp
// ❌ Version 1 — if/else দিয়ে জোড়াতালি
if (region == "US")        { /* Stripe code */ }
else if (region == "EU")   { /* Adyen code, ওরা minor units আলাদা করে */ }
else if (bankTransfer)     { /* কোনো API নেই — file generate হয়, ২ দিন পর settle হয় */ }
```

**এখন যা যা ভাঙল:**

- `InvoiceService` তিনটা SDK এর কথা জানে — test করতে তিনটা mock লাগে।
- প্রত্যেকটা নতুন provider = এই ফাইলটা আবার edit (কাল Day 10 এ এটার নাম শিখব: **OCP violation**)।
- সবচেয়ে খারাপ: bank transfer টা **সাথে সাথে succeed করে না**। তোমার সব code ধরে বসে আছে "payment মানে instant"। সেই ভুল ধারণাটাই এখন পুরো codebase এ ছড়িয়ে গেছে।

খেয়াল করো — Version 0 তে **encapsulation ঠিকই ছিল**। `Invoice` এর field private, guard আছে। তবুও design টা পচা। **কারণ সমস্যাটা encapsulation এর না, abstraction এর।** এই দুইটা আলাদা জিনিস, আজকের পুরো পয়েন্ট এটাই।

---

## 2. The idea in plain language — analogy

**গাড়ির ড্যাশবোর্ড।**

- **Encapsulation** = bonnet টা লক করা। তুমি চলতে চলতে ইঞ্জিনে হাত দিতে পারবে না। *(access control)*
- **Abstraction** = ড্যাশবোর্ডে কী কী থাকবে সেই **সিদ্ধান্ত**। Steering, accelerator, brake, speedometer — এতটুকুই। Fuel injector এর timing না, coolant এর pressure না। *(কোন ধারণাটা তুমি দেখাচ্ছ)*

দুইটা আলাদা ব্যর্থতা:

| ব্যর্থতা | মানে | ফল |
|---|---|---|
| Encapsulation ভাঙা | কেউ ইঞ্জিনে হাত দিয়ে দিল | একটা object corrupt |
| **Leaky abstraction** | ড্যাশবোর্ডে "Set Fuel Injector Pulse Width" নব বসিয়ে দিলে | **প্রত্যেক ড্রাইভারকে ইঞ্জিন বুঝতে হবে** |

আর সবচেয়ে বাজেটা: **ভুল abstraction।** ড্যাশবোর্ডে শুধু accelerator আর brake দিলে — মনে হবে সুন্দর, সরল। কিন্তু গাড়ি টার্ন নিতে পারবে না। **Abstraction টা যদি ভুল ধারণা দেয়, সেটা লুকানো detail এর চেয়েও বেশি ক্ষতিকর, কারণ ভুলটা তখন তোমার সব caller এর মধ্যে ছড়ায়।**

উপরের bank transfer এর ঘটনাটা এটাই ছিল। "Payment instant" — এই ভুল ধারণাটা abstraction এ ঢুকে গিয়েছিল।

**মনে রাখার বাক্য:**

> Encapsulation একটা **enforcement** কৌশল। Abstraction একটা **modelling সিদ্ধান্ত** — অর্থাৎ judgment। তাই abstraction ভুল করা সহজ, আর ঠিক করা কঠিন।

---

## 3. Minimal runnable example — `IPaymentProcessor` টা design করা

Interface টা লেখার **আগে** তিনটা প্রশ্ন। এই তিনটা আজকের আসল skill:

1. **Caller আসলে কী চায়?** — "এই টাকাটা তোলো।" ব্যস। সে কোন provider, কোন retry, কোন webhook — কিছুই জানতে চায় না।
2. **কোন detail টা provider-এর মধ্যে ভিন্ন হবেই?** — cents vs minor units, customer token এর নাম, sync vs async settlement. এগুলো **অবশ্যই ভেতরে থাকবে।**
3. **কোন সত্যটা সব provider এর জন্য সত্য?** — এটাই abstraction এর আসল আকার। ⚠️ এবং settlement instant হওয়া সেই সত্যগুলোর একটা **না**।

### প্রথম চেষ্টা — যেটা প্রায় সবাই লেখে

```csharp
// ⚠️ দেখতে ঠিক, কিন্তু leak করছে
public interface IPaymentProcessor
{
    Task<StripeChargeResponse> ChargeAsync(long amountInCents, string stripeCustomerId);
}
```

তিনটা leak, নাম ধরে বলো:
- `StripeChargeResponse` — return type এ একটা vendor. Adyen implement করবে কীভাবে?
- `amountInCents` — Stripe এর unit convention interface এ উঠে এসেছে। JPY এর cents নেই।
- `stripeCustomerId` — vendor identity leak.

### ভালো version

```csharp
// ---- Domain এর নিজের ভাষা, কোনো vendor শব্দ নেই ----

public readonly record struct Money(decimal Amount, string Currency);   // Day 1 এর Money, একটু বড় হয়ে

public sealed record PaymentRequest(
    Money Amount,
    string CustomerReference,      // *আমাদের* id, vendor এর না
    string IdempotencyKey);        // পরে দেখাচ্ছি এটা কেন interface এ থাকা লাগে

public enum PaymentState { Authorized, Settled, Pending, Failed }

public sealed record PaymentResult(
    PaymentState State,
    string ProviderReference,      // opaque string. আমরা কখনো parse করব না।
    string? FailureReason);

public interface IPaymentProcessor
{
    Task<PaymentResult> ChargeAsync(PaymentRequest request, CancellationToken ct = default);
    Task<PaymentResult> RefundAsync(string providerReference, Money amount, CancellationToken ct = default);
}
```

Implementation টাই একমাত্র জায়গা যেখানে vendor এর শব্দ থাকতে পারবে:

```csharp
public sealed class StripePaymentProcessor : IPaymentProcessor
{
    private readonly StripeClient _stripe;
    public StripePaymentProcessor(StripeClient stripe) => _stripe = stripe;

    public async Task<PaymentResult> ChargeAsync(PaymentRequest r, CancellationToken ct = default)
    {
        var intent = await _stripe.PaymentIntents.CreateAsync(new()
        {
            Amount   = ToMinorUnits(r.Amount),          // এই কুৎসিত জ্ঞানটা এখানেই বন্দী
            Currency = r.Amount.Currency.ToLowerInvariant(),
            Customer = MapCustomer(r.CustomerReference)
        }, new RequestOptions { IdempotencyKey = r.IdempotencyKey }, ct);

        return new PaymentResult(Map(intent.Status), intent.Id, intent.LastPaymentError?.Message);
    }

    public Task<PaymentResult> RefundAsync(string providerReference, Money amount, CancellationToken ct = default)
        => throw new NotImplementedException("তোমার কাজ।");

    private static long ToMinorUnits(Money m) =>
        ZeroDecimalCurrencies.Contains(m.Currency)      // JPY, KRW... cents নেই
            ? (long)m.Amount
            : (long)(m.Amount * 100);

    private static PaymentState Map(string stripeStatus) => stripeStatus switch
    {
        "succeeded"              => PaymentState.Settled,
        "requires_capture"       => PaymentState.Authorized,
        "processing"             => PaymentState.Pending,
        _                        => PaymentState.Failed
    };

    private static readonly HashSet<string> ZeroDecimalCurrencies =
        new(StringComparer.OrdinalIgnoreCase) { "JPY", "KRW", "VND" };

    private static string MapCustomer(string ourRef) => ourRef;   // আসলে একটা lookup হবে
}
```

আর caller টা এখন সরল, আর provider-অজ্ঞ:

```csharp
public sealed class InvoiceService
{
    private readonly IPaymentProcessor _payments;
    public InvoiceService(IPaymentProcessor payments) => _payments = payments;

    public async Task PayAsync(Invoice invoice, CancellationToken ct)
    {
        var result = await _payments.ChargeAsync(new PaymentRequest(
            new Money(invoice.Total, invoice.Currency),
            invoice.CustomerRef,
            IdempotencyKey: $"invoice-{invoice.Id}-attempt-{invoice.AttemptCount}"), ct);

        switch (result.State)                        // Pending কে ignore করা যাবে না — এটাই মূল্যবান অংশ
        {
            case PaymentState.Settled:   invoice.MarkPaid(result.ProviderReference);      break;
            case PaymentState.Authorized:invoice.MarkAuthorized(result.ProviderReference);break;
            case PaymentState.Pending:   invoice.MarkAwaitingSettlement(result.ProviderReference); break;
            case PaymentState.Failed:    invoice.MarkFailed(result.FailureReason);        break;
        }
    }
}
```

### দুইটা সিদ্ধান্ত ব্যাখ্যা করা দরকার — এগুলোই আজকের গভীর অংশ

**(a) কেন `IdempotencyKey` interface এ আছে? এটা তো vendor এর জিনিস মনে হয়?**

মনে হয়, কিন্তু না। Network এ টাকা পাঠানোর অর্থই হলো — তুমি জানো না request টা পৌঁছেছিল কি না। তাই "retry করলে double charge হবে না" — এই দরকারটা **provider এর detail না, ডোমেইনের সত্য**। যেই abstraction এটা লুকায়, সেটা caller কে চুপচাপ double-charge bug দিয়ে দেয়।

> **নিয়ম: যে detail টা caller এর সিদ্ধান্ত বদলে দেয়, সেটা লুকানো যাবে না। ওটা detail না, ওটাই contract.**

**(b) কেন `PaymentState.Pending` আছে?**

কারণ bank transfer instant না। আমি যদি `Task<bool>` return করতাম, abstraction টা দেখতে **আরও সুন্দর** হতো — আর সেটাই ফাঁদ। ওটা মিথ্যা বলত। **Abstraction এর কাজ সত্যকে সরল করা, সত্যকে বদলে দেওয়া না।**

এই লাইনটা মনে রাখো:

> A good abstraction hides **how**. A bad abstraction hides **whether it worked**.

---

## 4. Apply it in your world — Orbitax .NET stack

**(1) Clean Architecture তে interface টা কোথায় থাকে?** এটাই সবচেয়ে বেশি ভুল হয়।

`IPaymentProcessor` থাকবে **Application/Domain layer এ**, Infrastructure এ না। কারণ interface টার মালিক হলো **caller**, implementer না। Implementation (`StripePaymentProcessor`) থাকবে Infrastructure এ, আর ভেতরের দিকে তাকিয়ে থাকবে। (Day 17-18 এ এটার নাম পাবে — **DIP**, আর এই মালিকানার ব্যাপারটাই বেশিরভাগ লোক miss করে।)

দ্রুত পরীক্ষা: **Infrastructure project টা মুছে দিলে Application project টা compile করবে?** করলে abstraction ঠিক জায়গায়। না করলে leak আছে।

**(2) MediatR handler গুলোতে হান্ট করো (আজ, ১৫ মিনিট):** তোমার একটা handler খোলো, আর ওর dependency গুলো দেখো। প্রত্যেকটা interface এ প্রশ্ন করো — *"এই নামটা কি একটা business ধারণা, নাকি একটা technology?"*

- `ITaxFilingSubmitter` → business ধারণা ✅
- `IMongoCollectionWrapper` → technology, ছদ্মবেশে 🚩

**(3) তোমার MongoDB repository — সবচেয়ে সম্ভাব্য leak.** যদি কোনো method `IQueryable`, `FilterDefinition<T>`, বা `BsonDocument` return করে বা নেয় — তোমার abstraction leak করছে। Caller এখন Mongo এর কথা জানে, তাই in-memory দিয়ে test করা যাবে না, আর কোনোদিন storage বদলানো অসম্ভব। (Day 15-16 এ ISP তে আবার আসছে।)

**(4) তোমার GIR XML tooling — আসল সোনার খনি।** OECD schema টা ভয়ঙ্কর detailed। প্রশ্ন: caller `XElement` বানাচ্ছে, নাকি একটা domain model বানাচ্ছে যেটা XML এ translate হয়? প্রথমটা হলে schema টা তোমার পুরো codebase এ leak করছে, আর schema version 2.0 আসলে সব জায়গায় হাত দিতে হবে। দ্বিতীয়টা হলে একটা জায়গায় বদলালেই হবে।

**(5) তোমার TTS abstraction (Day 36 এ Adapter হিসেবে ফিরবে):** নিজেকে জিজ্ঞাসা করো — interface টার নাম কি `ISpeechSynthesizer` (ধারণা) নাকি `IAzureTtsWrapper` (vendor)? আর voice/format/rate গুলো কি vendor এর enum ব্যবহার করছে? করলে সেটা vendor lock, শুধু একটা interface এর ছদ্মবেশে।

**সোনালী পরীক্ষা, সব ক্ষেত্রেই এটা চালাও:**

> **Interface এর নাম আর ওর সব parameter/return type পড়ে কি বলা যায় কোন library টা ব্যবহার হচ্ছে? বলা গেলে সেটা abstraction না, সেটা একটা wrapper.**

---

## 5. "Is there a simpler way?" — সবসময় এই প্রশ্নটা

আজকে প্রশ্নটা খুব দরকারি, কারণ "সবকিছুর জন্য interface" — এটা .NET জগতের সবচেয়ে common over-engineering.

**কখন abstraction টা লাগবেই না:**

- **একটাই implementation, আর দ্বিতীয়টার কোনো সম্ভাবনা নেই।** `IUserMapper` এর একটামাত্র `UserMapper` — ওটা নিছক ceremony। Class টা সরাসরি inject করো। দরকার হলে পরে interface বের করে নেবে; Visual Studio এ ওটা ১০ সেকেন্ডের কাজ। **YAGNI আসল।**
- **শুধু mock করার জন্য interface।** এটা একটা code smell, যদিও অনেকে করে। যদি জিনিসটা pure logic হয়, mock না করে আসল টা দিয়েই test করো — test টা আরও ভালো হবে।
- **`IDateTimeProvider` টাইপ micro-abstraction** যদি একটা জায়গায় লাগে। যদি ২০ জায়গায় লাগে, তখন লাগবে।

**কখন abstraction টা non-negotiable:**

- একাধিক implementation **আজই** আছে (Stripe + Adyen)।
- একটা **external boundary** — network, payment, filing authority, file system, clock। এগুলো slow, flaky, আর expensive to test। তোমার ডোমেইন এদের ভরা।
- Provider বদলানোর খরচ যেখানে বিশাল (regulatory/vendor migration)।

**সবচেয়ে দামি কথাটা আজকের:** একটা **ভুল** abstraction, কোনো abstraction না থাকার চেয়ে খারাপ। Duplicated code refactor করা সহজ। কিন্তু ২০০ জায়গায় বসে যাওয়া একটা ভুল interface — ওটা নরক। তাই:

> **Two implementations দেখার আগে abstract করো না।** একটা example দিয়ে তুমি বুঝতেই পারবে না কোনটা আসল variation আর কোনটা কাকতালীয়। এই কারণেই সিনিয়ররা "rule of three" বলে।

---

## 6. আজকের hands-on task (লিখতে হবে, পড়লে হবে না)

`journey/code/day-02/Day02.cs` তে scaffold আছে। **নিজে টাইপ করো।**

1. Leaky `IPaymentProcessor` টা (file এর মাথায় আছে) পড়ো, আর comment এ **তিনটা leak নাম ধরে লেখো।**
2. পরিষ্কার abstraction টা নিজে লেখো: `Money`, `PaymentRequest`, `PaymentResult`, `IPaymentProcessor`.
3. **দুইটা** implementation লেখো — `FakePaymentProcessor` (সবসময় Settled) আর `BankTransferPaymentProcessor` (সবসময় **Pending**)।
4. **আসল কাজ:** `InvoiceService` লেখো যেটা দুইটার সাথেই কাজ করে। Compiler কে দিয়ে জোর করাও যেন `Pending` handle করতেই হয়। এখানেই টের পাবে — `Task<bool>` return করলে আজ এই bug টা তুমি ধরতেই পারতে না।
5. **তারপর ইচ্ছা করে abstraction টা ভাঙো:** interface এ একটা method যোগ করো — `Task<StripeCustomer> GetStripeCustomerAsync(string id)`. এখন `BankTransferPaymentProcessor` এ ওটা implement করার চেষ্টা করো। **ব্যথাটা অনুভব করো** — এই ব্যথাই leaky abstraction এর নাম, আর Day 12 (LSP) আর Day 15 (ISP) এ এই একই ব্যথা ফিরে আসবে। তারপর method টা মুছে দাও।
6. **তোমার নিজের codebase:** একটা আসল interface বের করো যেটা vendor detail leak করছে। `notes.md` তে লেখো — leak টা কী, আর leak না করে কী নাম/type হতে পারত।

চালাতে: console project এ ফেলে `dotnet run` (কোনো Stripe SDK লাগবে না — সব fake)।

---

## 7. One-line self-check

> **নিজের ভাষায় বলো: একটা class পুরোপুরি encapsulated (সব field private, সব guard আছে) — তবুও ওর abstraction ভয়ানক খারাপ হতে পারে। একটা উদাহরণ দাও।**

উত্তরে এই ধারণাটা থাকা চাই: encapsulation = access control (কে ছুঁতে পারবে), abstraction = কোন ধারণা/model টা তুমি দেখাচ্ছ। Method এর নাম বা signature যদি vendor detail ফাঁস করে, বা caller কে মিথ্যা ধারণা দেয় (যেমন "সব payment instant"), তাহলে state টা perfect পাহারায় থাকলেও design টা পচা।

---

## কালকের প্রস্তুতি (Day 3)

**Inheritance: কখন ব্যবহার করবে, আর কখন করবে না — "is-a" test.**

আজকে দেখলাম abstraction একটা judgment. কালকে দেখব সবচেয়ে বেশি misuse হওয়া abstraction টুল — inheritance. `class SavingsAccount : BankAccount` লিখতে ৩ সেকেন্ড লাগে, আর ওটা তোমাকে ৩ বছর ভোগাতে পারে। প্রশ্নটা কখনোই "code share করা যাবে?" না — প্রশ্নটা "এটা কি সত্যিই একটা BankAccount, base এর সব প্রতিশ্রুতি সহ?"

---

*Day 2 of 90 · টার্গেট: "এখন আমার বেসিক শক্তিশালী।"*
