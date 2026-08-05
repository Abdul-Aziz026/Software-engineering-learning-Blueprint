// ============================================================================
// Day 6 of 90 — Coupling & Cohesion
// ============================================================================
// নিয়ম: TODO গুলো নিজে হাতে টাইপ করো। copy-paste করলে আজকের পাঠ হারিয়ে যাবে।
// আসল কাজটা করো HandsOnPractice/CouplingCohesion/ project এ:
//     Bad Example/   -> PART 1 + PART 4
//     Good Example/  -> PART 2 + PART 3
// এই ফাইলটা শুধু roadmap.
//
// আজকের একমাত্র লক্ষ্য: একটা test লিখতে গিয়ে আটকে যাওয়া — নিজের হাতে।
// ওই আটকে যাওয়াটাই coupling এর পরিমাপ।
// ============================================================================

using System;
using System.Collections.Generic;

namespace Day06
{
    // ========================================================================
    // PART 0 — খেলার ঘুঁটি
    // ========================================================================
    // TODO 0: একটা ছোট Filing class/record লেখো —
    //           Id (int), TaxYear (int), Amount (decimal),
    //           Country (string), UserEmail (string)
    //         এটাই একমাত্র জিনিস যেটা copy-paste করলেও ক্ষতি নেই।


    // ========================================================================
    // PART 1 — ❌ BAD: এক class, ছয় কাজ, এগারোটা চেনা মুখ
    // ========================================================================
    // TODO 1a: FilingService লেখো, ভেতরে সরাসরি field হিসেবে —
    //            private readonly SqlConnection _db   = new(...);
    //            private readonly SmtpClient    _smtp = new(...);
    //          (package না থাকলে nuget নাও, অথবা fake class বানাও —
    //           কিন্তু 'new' টা ভেতরেই রাখো, ওটাই আজকের অপরাধ।)
    //
    // TODO 1b: Submit(Filing filing) লেখো, ছয়টা ধাপ একটার পর একটা:
    //            1. validate  (year / amount / country)
    //            2. calculate (BD হলে 25%, নাহলে 15%)
    //            3. XML string বানাও
    //            4. _db দিয়ে INSERT
    //            5. _smtp দিয়ে email
    //            6. File.AppendAllText দিয়ে log
    //
    // TODO 1c: থামো। কাগজে গোনো — এবং notes.md এ লেখো:
    //            (ক) কয়টা আলাদা কাজ?                        ______
    //            (খ) কয়টা বাইরের নাম চেনে? (SqlConnection,
    //                connection string, table নাম, SQL syntax,
    //                SmtpClient, host, from-address, File,
    //                path, DateTime.Now ...)                 ______
    //            (গ) কয়জন আলাদা মানুষ এই file বদলাতে বলতে পারে? ______
    //
    // TODO 1d: এক লাইনে class টার কাজ লেখার চেষ্টা করো।
    //          "আর" শব্দটা কয়বার লাগল?  ______
    //          👉 প্রতিটা "আর" একটা ফাঁস হওয়া cohesion.


    // ========================================================================
    // PART 1e — 🔴 আজকের সবচেয়ে গুরুত্বপূর্ণ ধাপ: test টা লেখার চেষ্টা
    // ========================================================================
    // TODO 1e: শুধু এইটুকু যাচাই করার test লেখো —
    //            "BD এর filing এ tax = Amount * 0.25"
    //
    //            var service = new FilingService();
    //            service.Submit(new Filing { Country = "BD", Amount = 1000m, ... });
    //            // ...এখন কী assert করব?
    //
    //          ⚠️ থামো এখানে। সত্যিই চালাও।
    //          - কী কী দাঁড় করাতে হলো? (SQL? SMTP? C:\logs\ folder?)
    //          - সত্যিকারের email কি চলে গেল?
    //          - assert করার মতো কিছু ফেরত এল কি?
    //
    //          👉 যেই লাইনে আটকে গেলে, সেই লাইনটা হুবহু notes.md এ লিখে রাখো।
    //             "test লেখা কঠিন" testing এর দোষ না — ওটা design এর রোগ নির্ণয়।
    //             এই মুহূর্তটা না পেরিয়ে PART 2 এ যেও না।


    // ========================================================================
    // PART 2 — ✅ GOOD, ধাপ ১: শুধু কাঁচি (cohesion). এখনো কোনো interface না।
    // ========================================================================
    // TODO 2a: FilingValidator বের করো  — Validate(Filing f)
    // TODO 2b: TaxCalculator   বের করো  — decimal Calculate(Filing f)
    //          👉 ইচ্ছাকৃতভাবে কোনো interface দিচ্ছি না। কারণ ভাবো:
    //             এদের কি দ্বিতীয় কোনো implementation লাগবে?
    //
    // TODO 2c: এখন সেই একই test আবার লেখো:
    //            Assert.Equal(250m, new TaxCalculator()
    //                .Calculate(new Filing { Country = "BD", Amount = 1000m }));
    //
    //          notes.md এ লেখো:
    //            - কত লাইন লাগল?            ______
    //            - কয়টা জিনিস দাঁড় করাতে হলো? ______
    //          👉 কোডের কোনো "logic" বদলায়নি। শুধু কে কোথায় থাকে সেটা বদলেছে।
    //             তবু test টা সম্ভব থেকে তুচ্ছ হয়ে গেল — এটাই আজকের পুরো point.


    // ========================================================================
    // PART 3 — ✅ GOOD, ধাপ ২: contract (coupling)
    // ========================================================================
    // TODO 3a: দুইটা interface লেখো —
    //            public interface IFilingStore { void Save(Filing f, decimal tax); }
    //            public interface INotifier    { void FilingSubmitted(Filing f); }
    //
    // TODO 3b: FilingService কে constructor দিয়ে চারটা নির্ভরতা নিতে দাও
    //          (validator, calculator, store, notifier) — সব readonly field.
    //          Submit() এখন চার লাইন: validate → calculate → save → notify.
    //
    // TODO 3c: দুইটা করে implementation লেখো —
    //            SqlFilingStore   / InMemoryFilingStore
    //            EmailNotifier    / NullNotifier
    //
    // TODO 3d: InMemory + Null দিয়ে পুরো Submit() এর test লেখো।
    //          notes.md এ লেখো:
    //            - FilingService এর ভেতরে কয়টা লাইন বদলাতে হলো? ______
    //          👉 উত্তর 0 হওয়ার কথা। কেন সেটা গুরুত্বপূর্ণ, এক লাইনে লেখো।
    //
    // TODO 3e: প্রতিটা নির্ভরতার পাশে লেখো — "এটা কি আমার চেয়ে ধীরে বদলায়?"
    //            FilingValidator ______   TaxCalculator ______
    //            IFilingStore    ______   INotifier     ______
    //          👉 এই প্রশ্নটার নামই Day 17 এ হবে DIP.


    // ========================================================================
    // PART 4 — ⚖️ উল্টো দিক: over-splitting ও একটা রোগ
    // ========================================================================
    // TODO 4a: FilingValidator কে তিনটা class এ ভাঙো —
    //            TaxYearValidator / TaxAmountValidator / TaxCountryValidator
    //
    // TODO 4b: এখন একটা নতুন নিয়ম যোগ করো ("Amount ১০ কোটির বেশি হলে reject")।
    //            - কয়টা file ছুঁতে হলো?              ______
    //            - কয়টা class নতুন বানাতে হলো?       ______
    //            - কয়টা জায়গায় register করতে হলো?   ______
    //
    // TODO 4c: এক লাইনে রায় দাও: এটা কি উন্নতি? কেন / কেন না?
    //          👉 ইঙ্গিত: তিনটা নিয়ম কি আলাদা কারণে বদলায়, নাকি একই কারণে
    //             (compliance) একসাথে? একই কারণ হলে ওরা একসাথে থাকারই কথা।
    //             এক-method class এর ছড়াছড়ি low cohesion এরই আরেক চেহারা।


    // ========================================================================
    // PART 5 — 🔍 Orbitax hunt (Day 9 এ এটা লাগবে — ফেলে দিও না)
    // ========================================================================
    // TODO 5a: তোমার repo এর সবচেয়ে বড় MediatR handler টা খুঁজে বের করো।
    //          (হাতিয়ার: `grep -rn "IRequestHandler" --include=*.cs | ...`
    //           অথবা শুধু বড় Handler file গুলো `wc -l` দিয়ে মাপো।)
    //
    // TODO 5b: notes.md এ table বানাও —
    //            | ও কী কী কাজ করে | কে এটা বদলাতে বলবে |
    //          দুইয়ের বেশি আলাদা নাম এলে: তুমি PART 1 কে নিজের codebase এ পেয়েছ।
    //
    // TODO 5c: Domain project এ কোনো infra namespace আছে কিনা খোঁজো
    //          (SqlClient, MongoDB.Driver, HttpClient, SmtpClient ...)।
    //          পেলে — layer এর তীরচিহ্ন উল্টো দিকে যাচ্ছে। file নামটা লিখে রাখো।
}
