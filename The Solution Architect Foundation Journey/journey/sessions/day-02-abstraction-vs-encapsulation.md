# Day 2 of 90 — Abstraction vs Encapsulation

**Block:** Month 1 (Foundation: OOD + SOLID) · Week 1 (OOP in depth)
**Originally written:** 2026-07-25 · **Simplified rewrite:** 2026-07-31

> **Note:** এটা Day 2 এর সহজ version. আগের version টা topic এর তুলনায় বেশি ভারী ছিল।
> আজকের লক্ষ্য একটাই ধারণা পরিষ্কার করা — বেশি না।

আজকের পুরো পাঠ এক লাইনে:

> **Encapsulation = ভেতরটা কে ছুঁতে পারবে।
> Abstraction = বাইরে থেকে জিনিসটা কেমন দেখায়।**

ব্যস। বাকিটা শুধু এই দুইটার উদাহরণ।

---

## 1. The problem first

তোমার invoice এর টাকা নিতে হবে। Stripe দিয়ে লিখলে:

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

তিন মাস পর boss বলল: "Europe এ Adyen লাগবে।"

এখন `InvoiceService` খুলে ভেতরে হাত দিতে হবে। Test করতে Stripe SDK লাগবে। নতুন provider এলে আবার এই ফাইলে হাত।

**এখানে খেয়াল করার জিনিসটা:** `Invoice` class টা কিন্তু ঠিকই ছিল — field private, guard আছে, Day 1 এর সব নিয়ম মানা। **তবুও design টা খারাপ।**

মানে সমস্যাটা encapsulation এর না। সমস্যাটা অন্য কিছু। ওটার নাম **abstraction**।

---

## 2. The idea — analogy

**গাড়ির ড্যাশবোর্ড।**

- **Encapsulation** = bonnet টা লক করা। চলতে চলতে তুমি ইঞ্জিনে হাত দিতে পারবে না।
- **Abstraction** = ড্যাশবোর্ডে কী কী থাকবে সেই **সিদ্ধান্ত**। Steering, brake, speedometer — এটুকুই। Fuel injector এর timing না।

দুইটাই দরকার, কিন্তু দুইটা আলাদা কাজ:

| | প্রশ্ন | ভাঙলে কী হয় |
|---|---|---|
| **Encapsulation** | কে state বদলাতে পারবে? | একটা object নষ্ট হয় |
| **Abstraction** | caller কী দেখবে? | সবাইকে ইঞ্জিন বুঝতে হয় |

উপরের code টায় bonnet লক করা ছিল ✅, কিন্তু ড্যাশবোর্ডে "Stripe" লেখা ছিল ❌।

---

## 3. Minimal example

Interface টা লেখার সময় একটাই প্রশ্ন:

> **Caller আসলে কী চায়?** → "এই টাকাটা তোলো।" ব্যস। সে Stripe না Adyen, জানতে চায় না।

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

Adyen এই interface implement করবে কীভাবে? পারবে না।

### ✅ ভালো

```csharp
public record PaymentRequest(decimal Amount, string Currency, string CustomerRef);

public record PaymentResult(bool Success, string Reference, string? Error);

public interface IPaymentProcessor
{
    Task<PaymentResult> ChargeAsync(PaymentRequest request);
}
```

কোথাও "Stripe" শব্দটা নেই। এখন Stripe শুধু **একটা** ফাইলে বন্দী:

```csharp
public class StripePaymentProcessor : IPaymentProcessor
{
    private readonly StripeClient _stripe;

    public async Task<PaymentResult> ChargeAsync(PaymentRequest r)
    {
        var charge = await _stripe.Charges.CreateAsync(new()
        {
            Amount   = (long)(r.Amount * 100),        // Stripe এর কুৎসিত detail এখানেই থাকল
            Currency = r.Currency.ToLower(),
            Customer = r.CustomerRef
        });

        return new PaymentResult(charge.Status == "succeeded", charge.Id, null);
    }
}
```

আর caller টা এখন শান্ত:

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

আজ **একটাই** কাজ, ১০ মিনিট।

তোমার একটা MediatR handler খোলো। ওর constructor এর dependency গুলো দেখো। প্রত্যেকটা interface এর নামে এই প্রশ্ন করো:

> **এই নামটা কি একটা business ধারণা, নাকি একটা technology?**

- `ITaxFilingSubmitter` → ধারণা ✅
- `IMongoRepository` → technology 🚩

আর repository তে একটা জিনিস খোঁজো: কোনো method কি `IQueryable`, `FilterDefinition<T>`, বা `BsonDocument` return করে বা নেয়? করলে Mongo leak করছে — caller এখন database এর কথা জানে।

*(Clean Architecture তে interface টা কোন layer এ থাকা উচিত — সেই আলোচনাটা Day 17-18, DIP এর দিন। আজ শুধু নাম আর type দেখো।)*

---

## 5. "Is there a simpler way?"

হ্যাঁ, প্রায়ই। **.NET এ সবচেয়ে বড় over-engineering হলো — সবকিছুর জন্য interface।**

**Interface লাগবে না যখন:**
- একটাই implementation, দ্বিতীয়টার কোনো সম্ভাবনা নেই। `IUserMapper` → `UserMapper` — এটা শুধু ceremony। Class টা সরাসরি inject করো।
- শুধু mock করার জন্য বানাচ্ছ।

**Interface লাগবেই যখন:**
- একাধিক implementation **আজই** আছে (Stripe + Adyen)।
- বাইরের কিছুর সাথে কথা বলছ — payment, network, file, filing authority। এগুলো ছাড়া test করা কঠিন।

**আজকের সবচেয়ে দামি লাইন:**

> **ভুল abstraction, কোনো abstraction না থাকার চেয়ে খারাপ।**
> Duplicate code পরে সহজে ঠিক করা যায়। কিন্তু ২০০ জায়গায় বসে যাওয়া একটা ভুল interface — ওটা সরানো নরক।
> তাই **দুইটা implementation চোখে না দেখা পর্যন্ত abstract করো না।**

---

## 6. আজকের hands-on task

`journey/code/day-02/Day02.cs` তে scaffold আছে। **আজকের জন্য এই তিনটাই যথেষ্ট:**

1. Leaky interface টার তিনটা leak comment এ লেখো। ✅ *(তুমি এটা করে ফেলেছ)*
2. পরিষ্কার version টা নিজে টাইপ করো — `PaymentRequest`, `PaymentResult`, `IPaymentProcessor`.
3. দুইটা fake implementation লেখো, আর `InvoiceService` টা **দুইটার সাথেই** চালাও — একটা লাইনও না বদলে। এই মুহূর্তটাই আজকের পুরো শিক্ষা।

**তারপর, সময় থাকলে (optional):**

4. Interface এ একটা Stripe-only method যোগ করো: `Task<StripeCustomer> GetStripeCustomerAsync(string id);`
   এখন bank transfer class এ ওটা implement করার চেষ্টা করো। পারবে না — throw করা ছাড়া উপায় নেই।
   **এই ব্যথাটাই** Day 12 (LSP) আর Day 15 (ISP) এর বীজ। অনুভব করে method টা মুছে দাও।

5. তোমার নিজের code এ একটা leaky interface খুঁজে `notes.md` তে লেখো।

*(File এ `Money`, `IdempotencyKey`, `PaymentState.Pending` — এগুলো আছে। আজ ওগুলো নিয়ে মাথা ঘামিও না, নিচের bonus এ এক প্যারায় বলে দিলাম।)*

---

## 7. One-line self-check

> **নিজের ভাষায় বলো: Encapsulation আর Abstraction এর পার্থক্য কী?**

সহজ উত্তর: encapsulation ঠিক করে **কে ভেতরে ঢুকতে পারবে**; abstraction ঠিক করে **বাইরে থেকে কী দেখা যাবে**। একটা class পুরোপুরি encapsulated হয়েও খারাপ abstraction দিতে পারে — যেমন `IPaymentProcessor` যার signature এ Stripe লেখা।

---

## Bonus (আজ skip করলেও চলবে — Day 12/15 এ ফিরবে)

Code file এ দুইটা জিনিস আছে যা আজকের পাঠের বাইরে, তবু বাস্তব code এ লাগে:

- **`IdempotencyKey`** — network এ request পাঠালে তুমি জানো না ওটা পৌঁছেছিল কি না। Retry করলে দুইবার charge হয়ে যেতে পারে। তাই একটা unique key পাঠাও — provider দেখে বলে "এটা তো আগেই করেছি" আর আগের result ফেরত দেয়।
- **`PaymentState.Pending`** — bank transfer সাথে সাথে হয় না, ২ দিন লাগে। `bool Success` দিয়ে ওটা প্রকাশ করা যায় না, তাই invoice ভুল করে "Paid" হয়ে যায়। এটা একটা সুন্দর নিয়মের উদাহরণ: **abstraction detail লুকাবে, কিন্তু সত্য বদলাবে না।**

দুইটাই এখন শুধু চিনে রাখো। আজকের পরীক্ষায় আসবে না।

---

## কালকের প্রস্তুতি (Day 3)

**Inheritance: কখন ব্যবহার করবে, কখন করবে না — "is-a" test.**

`class SavingsAccount : BankAccount` লিখতে ৩ সেকেন্ড লাগে, আর ভুল হলে ৩ বছর ভোগায়। প্রশ্নটা কখনোই "code share করা যাবে?" না।

---

*Day 2 of 90 · টার্গেট: "এখন আমার বেসিক শক্তিশালী।"*
