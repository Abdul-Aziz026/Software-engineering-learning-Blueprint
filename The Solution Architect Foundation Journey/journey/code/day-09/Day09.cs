// ============================================================================
// Day 9 of 90 — SRP practice: একটা আসল MediatR handler কাটা
// ============================================================================
// নিয়ম: TODO গুলো নিজে হাতে টাইপ করো। copy-paste করলে আজকের পাঠ হারিয়ে যাবে।
// আসল কাজটা করো এখানে:
//     HandsOnPractice/SingleResponsibility/Handler Practice/
//         Bad Example/   -> PART A (1-3)
//         Good Example/  -> PART A (4)
//         Fake Refactor/ -> PART B (5)
// এই ফাইলটা শুধু roadmap। উত্তর লেখো notes.md এ।
//
// আজ নতুন principle নেই। আজ একটা PROCEDURE — চার ধাপের SRP audit:
//   STEP 1  প্রতিটা block এর পাশে কাজের নাম লেখো   (জগৎ বদলালেই সীমানা)
//   STEP 2  প্রতিটা কাজের পাশে actor + ঘড়ি লেখো
//   STEP 3  প্রতিটা কাজকে গন্তব্য দাও:
//              ⬇️ Domain     — "API আর DB মুছে দিলেও নিয়মটা সত্য?"
//              ⬇️ Infra      — "test করতে মেশিন লাগে?"
//              ⬆️ Pipeline   — "এই কোড কি প্রতিটা handler এ থাকত?"
//              ➡️ Handler    — "সরালে feature টাই বদলে যায়?"
//   STEP 4  যা থেকে গেল সেটা পড়ো — feature এর গল্প শোনাচ্ছে কি?
//
// আজকের একমাত্র লক্ষ্য: নিজের হাতে প্রমাণ করা যে
//   ৪টা dependency থাকা handler-ও SRP মানতে পারে,
//   আর ২টা tax rate জানা handler পারে না।
//   => call গোনো না, DECISION গোনো।
// ============================================================================

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Day09
{
    // ========================================================================
    // PART 0 — TODO 0: fake plumbing (২ মিনিট, তারপর আর ফিরে তাকাবে না)
    // ========================================================================
    // MediatR / Mongo / SignalR install করার দরকার নেই। লক্ষ্য কোড চালানো না,
    // লক্ষ্য ভিড়টা নিজের হাতে অনুভব করা। নিজে লিখে নাও:
    //
    //   interface IRequestHandler<TReq, TRes> {
    //       Task<TRes> Handle(TReq request, CancellationToken ct);
    //   }
    //   class FakeMongo<T>   { public Task InsertOneAsync(T doc, CancellationToken ct) ... }
    //   class FakeHub        { public Task SendAsync(string ev, object arg, CancellationToken ct) ... }
    //   class FakeSmtp       { public void Send(string to, string subject, string body) ... }
    //   class FakeLogger     { public void LogInformation(string msg, params object[] a) ... }
    //
    // Console.WriteLine দিয়ে ভরে দাও — কোনটা কখন চলল সেটা চোখে দেখতে পাবে।


    // ========================================================================
    // PART A — ❌ BAD: এক handler, আট actor, এক করিডোর
    // ========================================================================
    // TODO 1: session file এর bad `CreateFilingCommandHandler` হুবহু হাতে লেখো।
    //         নয়টা কাজ, এই ক্রমে — কোনোটা বাদ দিও না, ভিড়টাই আজকের বিষয়:
    //            1. logging
    //            2. TIN + period validation  (৩টা if/throw)
    //            3. tax calculation          (taxable base + ৩টা rate)
    //            4. DTO -> entity mapping    (DateTime.Now সহ — bug টা রাখো!)
    //            5. GIR XML payload
    //            6. Mongo insert
    //            7. audit trail insert
    //            8. SignalR push
    //            9. email
    //
    //         command + result টাও লাগবে:
    //            record CreateFilingCommand(string Tin, int PeriodYear,
    //                 string Jurisdiction, decimal Revenue, decimal Deductions,
    //                 string UserEmail);
    //            record CreateFilingResult(string Id, decimal Tax);
    //
    // লেখা শেষ হলে থামো। কোড না পড়ে শুধু নিজেকে জিজ্ঞেস করো:
    //     "এই handler টা বদলাতে বলার ক্ষমতা কতজন মানুষের আছে?"
    // প্রথম যে সংখ্যাটা মাথায় এলো, notes.md এ লিখে ফেলো — পরে মিলিয়ে দেখবে।


    // ------------------------------------------------------------------------
    // TODO 2 — STEP 1 + STEP 2: ACTOR + CLOCK TABLE  (কোড না, আজকের সবচেয়ে দামি ধাপ)
    // ------------------------------------------------------------------------
    // notes.md এর table টা পূরণ করো — নয়টা সারি, তিনটা কলাম:
    //     কাজ | কে বদলাতে বলে (actor) | কত ঘন ঘন বদলায় (clock)
    //
    // ⚠️ session file এর উত্তর-table দেখার আগে নিজের সংখ্যাটা লিখে ফেলো।
    //    ভুল হলে ক্ষতি নেই — কিন্তু আগে দেখে ফেললে আজকের practice টা
    //    reading exercise হয়ে যাবে (Day 7 এর সেই একই ফাঁদ)।


    // ------------------------------------------------------------------------
    // TODO 3 — STEP 3: গন্তব্য বসাও
    // ------------------------------------------------------------------------
    // নয়টা কাজের প্রতিটার পাশে একটা লেখো: ⬇️Domain / ⬇️Infra / ⬆️Pipeline / ➡️Handler
    //
    // 🛑 audit trail এ এসে থামো। ওটা কোথায় যাবে?
    //    - Pipeline behaviour এর পক্ষে যুক্তি কী?
    //    - Domain event এর পক্ষে যুক্তি কী?
    //    দুই দিকের যুক্তি লেখো, তারপর একটা বাছো আর কারণ লেখো।
    //    (একটাই সঠিক উত্তর নেই। trade-off বলতে পারাটাই আজকের পরীক্ষা।)


    // ========================================================================
    // PART A (contd.) — ✅ GOOD: প্রতিটা কাজ তার মালিকের ঠিকানায়
    // ========================================================================
    // TODO 4: নিচের টুকরোগুলো লেখো — Good Example/ ফোল্ডারে, আলাদা file এ।
    //
    //   ⬇️ Domain:
    //      class Filing            -> private ctor + static Draft(...) factory,
    //                                 invariant ভেতরে (TIN খালি না, amount >= 0),
    //                                 TaxableBase => Math.Max(0, Revenue - Deductions),
    //                                 Tax/Payload -> private set + SetTax/SetPayload
    //      interface ITaxCalculator / class TaxCalculator
    //                              -> switch on Jurisdiction: IE .125, HU .09, _ .15
    //                                 ⚠️ এই class এ Mongo/SMTP/Logger এর নাম থাকবে না।
    //                                    একবার লেখা শেষ করে using গুলো পড়ো — বিশুদ্ধ?
    //      interface IGirXmlBuilder / class GirXmlBuilder
    //
    //   ⬇️ Infra:
    //      interface IFilingRepository / class MongoFilingRepository
    //      interface IFilingNotifier   / class FilingNotifier   (SignalR + email দুইটাই)
    //
    //   ⬆️ উপরে উঠে গেল (handler এ ফিরবে না):
    //      class CreateFilingCommandValidator   (FluentValidation এর আকৃতিতে,
    //                                            fake হলেও চলবে)
    //      class LoggingBehaviour<TReq,TRes>
    //      class AuditBehaviour<TReq,TRes>
    //
    //   ➡️ Handler: ৭ লাইন। ৪টা ctor dependency। কোনো if নেই, কোনো number নেই।
    //
    // TODO 5 — 🔴 আজকের প্রমাণ: mock ছাড়া test
    //      TaxCalculator এর জন্য একটা test লেখো (xUnit না থাকলে Main এ
    //      Console assert চলবে):
    //          IE, revenue 1000, deductions 200  =>  expected 100
    //          HU, revenue 500,  deductions 900  =>  expected 0   (base clamp)
    //
    //      এখন notes.md এ এক লাইনে লেখো:
    //          "এই test টা আগের design এ লেখা সম্ভব ছিল না, কারণ ______"
    //      (উত্তরে 'Mongo', 'SMTP', 'SignalR' — এই শব্দগুলো আসা উচিত।)


    // ========================================================================
    // PART B — ⚠️ ভুয়া refactor: "সরানো" আর "ভাগ করা" এক জিনিস না
    // ========================================================================
    // TODO 6: Fake Refactor/ ফোল্ডারে —
    //      পুরো ৫০ লাইন FilingService.CreateFiling() এ সরাও,
    //      handler কে one-liner বানাও:
    //          => await _filingService.CreateFiling(cmd, ct);
    //
    //      এখন সৎভাবে notes.md এ লেখো:
    //          Handler এ actor সংখ্যা:              ____
    //          FilingService এ actor সংখ্যা:        ____
    //          System এ মোট actor-বিভ্রান্তি কমল?   ____
    //          "লাইন সরানো refactor না, কারণ ______"
    //
    //      যাচাইয়ের প্রশ্নটা মনে রাখো: লাইন সংখ্যা না, **actor সংখ্যা কমল কি?**


    // ========================================================================
    // PART C — 🎯 আসল কাজ: তোমার নিজের Orbitax handler
    // ========================================================================
    // এখানে কোনো কোড লিখতে হবে না — notes.md এ table পূরণ করো।
    //
    // TODO 7: তোমার সবচেয়ে বড় handler টা খোলো (Day 6 এর ৭ নম্বর hunt,
    //         Day 8 এ যেটার `using` list গুনেছিলে)। চার ধাপের audit চালাও:
    //             file নাম (বা ছদ্মনাম) · Handle() কত লাইন ·
    //             কয়টা কাজ · কয়জন actor · প্রতিটার গন্তব্য ·
    //             কাটার পরে কত লাইন হতো (আন্দাজ)
    //
    // TODO 8: git দিয়ে তোমার actor-table যাচাই করো —
    //             git log --oneline --since="6 months ago" -- <path>
    //             git shortlog -sn --since="6 months ago" -- <path>
    //         কয়টা commit? কয়জন আলাদা author?
    //         এই সংখ্যাগুলো তোমার আন্দাজ সমর্থন করল, না ভুল প্রমাণ করল?
    //         (দুইটাই মূল্যবান উত্তর — git মিথ্যা বলে না।)
    //
    // TODO 9 — 🔁 উল্টো drill (over-engineering ধরার জন্য, বাদ দিও না):
    //         তোমার সবচেয়ে ছোট handler টা খোঁজো। ওটার উপরেও audit চালাও,
    //         তারপর যুক্তি দাও কেন ওটাকে **ছুঁয়ো না**।
    //         তিনটা trigger এর কয়টা ওখানে আছে?
    //             (1) ঘন ঘন বদলায়?  (2) আলাদা টিম থেকে PR আসে?  (3) test এ infra লাগে?
    //         একটাও না ⇒ হাত দিও না। Refactor এর trigger পরিবর্তনের হার,
    //                    কোডের সৌন্দর্য না।


    // ========================================================================
    // Stretch (ঐচ্ছিক)
    // ========================================================================
    // TODO 10: repo র যেকোনো একটা pipeline behaviour খুলে পড়ো। লেখো —
    //          এই কোডটা behaviour না হয়ে handler এ থাকলে কয়টা file এ
    //          একই জিনিস লেখা থাকত?  (Day 50 এর কাঁচামাল)
    //
    // TODO 11: bad handler এর  if (cmd.Jurisdiction == "IE")  সিঁড়িটা মার্ক করে রাখো।
    //          SRP বলেছে ফাইলটা কোথায় থাকবে। OCP (Day 10) বলবে ওটার ভেতরে
    //          হাত না দিয়ে কীভাবে বাড়াবে। কাল সকালে ওটাই প্রথম শিকার।
    // ========================================================================
}
