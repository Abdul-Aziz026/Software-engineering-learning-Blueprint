# Day 2 of 90 — Abstraction vs Encapsulation

**Block:** Month 1 (Foundation: OOD + SOLID) · Week 1 (OOP in depth)

আজকের পুরো পাঠ এক লাইনে:

> **Encapsulation = ভেতরটা কে ছুঁতে পারবে।
> Abstraction = বাইরে থেকে জিনিসটা কেমন দেখায়।**

ব্যস। বাকিটা শুধু উদাহরণ।

---

## 1. The problem first

Invoice এর টাকা নিতে হবে। Stripe দিয়ে লিখলে:

```csharp
public class InvoiceService
{
    private readonly StripeClient _stripe;

    public async Task PayAsync(Invoice invoice)
    {
        var charge = await _stripe.Charges.CreateAsync(...);   // 💀 Stripe এর কথা এখানে
        invoice.MarkPaid(charge.Id);
    }
}
```

তিন মাস পর boss বলল: **"Europe এ Adyen লাগবে।"**

এখন `InvoiceService` খুলে ভেতরে হাত দিতে হবে। Test করতেও Stripe SDK লাগবে।

**এখানে আসল খেয়াল করার জিনিসটা:** `Invoice` class টা কিন্তু ঠিকই ছিল — field private, guard আছে, Day 1 এর সব নিয়ম মানা। **তবুও design টা খারাপ।**

মানে সমস্যাটা encapsulation এর না। ওটার নাম **abstraction**।

---

## 2. The idea — analogy

**গাড়ির ড্যাশবোর্ড।**

- **Encapsulation** = bonnet টা লক করা। চলতে চলতে ইঞ্জিনে হাত দিতে পারবে না।
- **Abstraction** = ড্যাশবোর্ডে কী থাকবে সেই **সিদ্ধান্ত**। Steering, brake, speedometer — এটুকুই। Fuel injector এর timing না।

| | প্রশ্ন | ভাঙলে কী হয় |
|---|---|---|
| **Encapsulation** | কে state বদলাতে পারবে? | একটা object নষ্ট হয় |
| **Abstraction** | caller কী দেখবে? | সবাইকে ইঞ্জিন বুঝতে হয় |

উপরের code টায় bonnet লক করা ছিল ✅, কিন্তু ড্যাশবোর্ডে "Stripe" লেখা ছিল ❌।

---

## 3. Minimal example

Interface লেখার সময় একটাই প্রশ্ন:

> **Caller আসলে কী চায়?** → "এই টাকাটা তোলো।" ব্যস।

### ❌ খারাপ

```csharp
public interface IPaymentProcessor
{
    Task<StripeChargeResponse> ChargeAsync(long amountInCents, string stripeCustomerId);
}
```

নামটা `IPaymentProcessor`, কিন্তু ভেতরে তিন জায়গায় Stripe:

- `StripeChargeResponse` — Stripe এর type
- `amountInCents` — Stripe এর unit
- `stripeCustomerId` — Stripe এর id

Adyen এটা implement করবে কীভাবে? পারবে না।

### ✅ ভালো

```csharp
public record PaymentRequest(decimal Amount, string Currency, string CustomerRef);
public record PaymentResult(bool Success, string Reference, string? Error);

public interface IPaymentProcessor
{
    Task<PaymentResult> ChargeAsync(PaymentRequest request);
}
```

কোথাও "Stripe" শব্দটা নেই। Stripe এখন **একটা** ফাইলে বন্দী — `StripePaymentProcessor` এর ভেতরে, যেখানে `Amount * 100` করে cents বানানো হয়। ওই কুৎসিত detail টা আর বাইরে আসে না।

Caller এখন শান্ত:

```csharp
public class InvoiceService
{
    private readonly IPaymentProcessor _payments;
    public InvoiceService(IPaymentProcessor payments) => _payments = payments;

    public async Task PayAsync(Invoice invoice)
    {
        var result = await _payments.ChargeAsync(
            new PaymentRequest(invoice.Total, invoice.Currency, invoice.CustomerRef));

        if (result.Success) invoice.MarkPaid(result.Reference);
        else                invoice.MarkFailed(result.Error);
    }
}
```

Adyen লাগলে নতুন একটা class লিখবে। `InvoiceService` এ **একটা লাইনও** বদলাবে না।

**মনে রাখার পরীক্ষা:**

> **Interface টা পড়ে কি বলা যায় কোন library ব্যবহার হচ্ছে?
> বলা গেলে — ওটা abstraction না, ওটা wrapper.**

---

## 4. Apply it — তোমার Orbitax stack

আজ একটাই কাজ, ১০ মিনিট।

একটা MediatR handler খোলো। ওর constructor এর dependency গুলোর নামে প্রশ্ন করো:

> **এই নামটা কি একটা business ধারণা, নাকি একটা technology?**

- `ITaxFilingSubmitter` → ধারণা ✅
- `IMongoRepository` → technology 🚩

আর repository তে খোঁজো: কোনো method কি `IQueryable`, `FilterDefinition<T>`, বা `BsonDocument` নেয় বা return করে? করলে Mongo leak করছে — caller এখন database এর কথা জানে।

---

## 5. "Is there a simpler way?"

হ্যাঁ। **.NET এ সবচেয়ে বড় over-engineering — সবকিছুর জন্য interface।**

**লাগবে না যখন:** একটাই implementation, দ্বিতীয়টার সম্ভাবনা নেই (`IUserMapper` → `UserMapper`)। অথবা শুধু mock করার জন্য বানাচ্ছ।

**লাগবেই যখন:** একাধিক implementation **আজই** আছে, বা বাইরের কিছুর সাথে কথা বলছ — payment, network, filing authority।

**আজকের সবচেয়ে দামি লাইন:**

> **ভুল abstraction, কোনো abstraction না থাকার চেয়ে খারাপ।**
> Duplicate code পরে সহজে ঠিক করা যায়। কিন্তু ২০০ জায়গায় বসে যাওয়া একটা ভুল interface সরানো নরক।
> তাই **দুইটা implementation চোখে না দেখা পর্যন্ত abstract করো না।**

---

## 6. আজকের hands-on task

তিনটাই যথেষ্ট:

1. Leaky interface টার তিনটা leak নাম ধরে লেখো। ✅ *(করে ফেলেছ)*
2. পরিষ্কার version টা নিজে টাইপ করো — `PaymentRequest`, `PaymentResult`, `IPaymentProcessor`.
3. **দুইটা** fake implementation লেখো (card + wallet), আর `InvoiceService` টা **দুইটার সাথেই** চালাও — একটা লাইনও না বদলে। এই মুহূর্তটাই আজকের পুরো শিক্ষা।

> তোমার `HandsOnPractice/Abstraction/` project এ `Money`, `IdempotencyKey`, `PaymentState.Pending` আছে।
> **আজ ওগুলো ধরার দরকার নেই** — ওরা Day 12/15/51 এর জিনিস। `decimal` আর `bool Success` দিয়েই আজকের কাজ হয়ে যাবে।

---

## 7. One-line self-check

> **নিজের ভাষায় বলো: Encapsulation আর Abstraction এর পার্থক্য কী?**

উত্তর: encapsulation ঠিক করে **কে ভেতরে ঢুকতে পারবে**; abstraction ঠিক করে **বাইরে থেকে কী দেখা যাবে**। একটা class পুরোপুরি encapsulated হয়েও খারাপ abstraction দিতে পারে — যেমন `IPaymentProcessor` যার signature এ Stripe লেখা।

---

## কালকের প্রস্তুতি (Day 3)

**Inheritance: কখন ব্যবহার করবে, কখন করবে না — "is-a" test.**

`class SavingsAccount : BankAccount` লিখতে ৩ সেকেন্ড লাগে, ভুল হলে ৩ বছর ভোগায়। প্রশ্নটা কখনোই "code share করা যাবে?" না।

---

*Day 2 of 90 · টার্গেট: "এখন আমার বেসিক শক্তিশালী।"*
