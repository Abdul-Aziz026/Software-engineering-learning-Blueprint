// ============================================================================
// Day 5 of 90 — Polymorphism: subtype vs ad-hoc, আর vtable
// ============================================================================
// নিয়ম: TODO গুলো নিজে হাতে টাইপ করো। copy-paste করলে আজকের পাঠ হারিয়ে যাবে।
// আসল কাজটা করো HandsOnPractice/Polymorphism/ project এ:
//     Bad Example/   -> PART 1 + PART 4
//     Good Example/  -> PART 2 + PART 3
// এই ফাইলটা শুধু roadmap.
//
// আজকের একমাত্র লক্ষ্য: একটা object, দুইটা উত্তর — নিজের চোখে দেখা।
// ============================================================================

using System;
using System.Collections.Generic;

namespace Day05
{
    // ========================================================================
    // PART 1 — ❌ BAD: 'new' দিয়ে warning চুপ করানো
    // ========================================================================
    // TODO 1a: Account base class লেখো —
    //            public decimal Balance { get; protected set; }
    //            public decimal MonthlyInterest() => 0m;      // virtual নয়!
    //          এই '0m' টাই আজকের আসল অপরাধী: একটা মিথ্যা default.
    //
    // TODO 1b: SavingsAccount : Account লেখো, ভেতরে —
    //            public new decimal MonthlyInterest() => Balance * 0.05m / 12;
    //          'new' না লিখলে compiler warning দেবে। warning টা একবার পড়ো,
    //          তারপর 'new' লিখে চুপ করাও — junior রা ঠিক এটাই করে।
    //
    // TODO 1c: এই দুই লাইন চালাও এবং দুইটা সংখ্যাই notes.md এ লিখে রাখো:
    //
    //            SavingsAccount savings = new SavingsAccount(100_000m);
    //            Account        asBase  = savings;      // একই object!
    //
    //            Console.WriteLine(savings.MonthlyInterest());   // ?
    //            Console.WriteLine(asBase.MonthlyInterest());    // ?
    //
    //          ⚠️ থামো এখানে। সংখ্যা দুইটা আলাদা আসার পর নিজেকে জিজ্ঞেস করো:
    //             "object টা তো একটাই — তাহলে পার্থক্যটা কোথায় হলো?"
    //             উত্তরটা মাথায় আসার আগে PART 2 এ যেও না।
    //
    // TODO 1d: FixedDeposit আর CurrentAccount ও লেখো, তারপর month-end job:
    //
    //            List<Account> accounts = new() { savings, fd, current };
    //            decimal total = 0m;
    //            foreach (Account a in accounts) total += a.MonthlyInterest();
    //            Console.WriteLine($"মাসিক মোট interest: {total}");
    //
    //          total কত এল? এটাই সেই bug যেটা crash করে না, শুধু ভুল টাকা দেয়।


    // ========================================================================
    // PART 2 — ✅ GOOD, ধাপ ১: virtual + override
    // ========================================================================
    // TODO 2a: Account.MonthlyInterest() কে 'virtual' করো।
    // TODO 2b: SavingsAccount এ 'new' মুছে 'override' লেখো।
    // TODO 2c: PART 1 এর ঐ একই দুই লাইন আবার চালাও।
    //          এখন দুইটা সংখ্যা এক। foreach loop এর একটা অক্ষরও বদলাওনি —
    //          এই "loop অপরিবর্তিত" ব্যাপারটাই polymorphism এর পুরো point.


    // ========================================================================
    // PART 3 — ✅ GOOD, ধাপ ২: abstract দিয়ে compiler কে পাহারায় বসানো
    //          (আজকের সবচেয়ে দামি অংশ)
    // ========================================================================
    // TODO 3a: Account কে 'abstract class' করো, আর method টা —
    //            public abstract decimal MonthlyInterest();
    //          base এর '0m' body টা পুরোপুরি মুছে ফেলো। কোনো default থাকবে না।
    //
    // TODO 3b: CurrentAccount এ ইচ্ছা করেই লেখো: override ... => 0m;
    //          এই 0 আর PART 1 এর 0 এক জিনিস নয় —
    //          একটা "ভেবে নেওয়া সিদ্ধান্ত", আরেকটা "ভুলে যাওয়া"।
    //
    // TODO 3c: 🔴 এই ধাপটা বাদ দিও না।
    //          StudentAccount : Account লেখো, কিন্তু MonthlyInterest() লিখো *না*।
    //          Build করো। Compiler কী বলল?
    //          → error টা হুবহু copy করে notes.md এ রাখো।
    //          এখন PART 4 এর switch version এ একই কাজ করে দেখবে — সেখানে
    //          compiler একটা শব্দও বলবে না। এই তুলনাটাই আজকের মূল শিক্ষা।


    // ========================================================================
    // PART 4 — ❌ BAD (বিকল্প রোগ): type এর উপর switch
    // ========================================================================
    // TODO 4a: InterestService লেখো —
    //            if (a is SavingsAccount) return ...;
    //            if (a is FixedDeposit)   return ...;
    //            if (a is CurrentAccount) return 0m;
    //            return 0m;                       // ← নীরব ফাঁদ
    //
    // TODO 4b: আরও দুইটা switch লেখো একই ধাঁচে: MonthlyFee(a), StatementLabel(a).
    //          তিনটাই লেখো — পুনরাবৃত্তিটা টের পাওয়াই উদ্দেশ্য।
    //
    // TODO 4c: এবার StudentAccount যোগ করো।
    //            (i)  কয়টা জায়গায় হাত দিতে হলো?
    //            (ii) compiler কয়বার তোমাকে থামাল?   ← উত্তর: ০
    //          দুইটা সংখ্যা notes.md এ লিখো।
    //
    // TODO 4d: এক লাইনে লিখো — এই তিনটার মধ্যে কোন switch টা আসলে
    //          বাইরে থাকাই ঠিক ছিল, আর কেন?
    //          (হিন্ট: StatementLabel টা কার ব্যবসা — account এর, না UI এর?)


    // ========================================================================
    // PART 5 — ad-hoc বনাম subtype: ফাঁদটা প্রমাণ করো
    // ========================================================================
    // TODO 5a: দুইটা overload লেখো (static হলেই চলবে) —
    //            static void Log(Account a)        => Console.WriteLine("Account");
    //            static void Log(SavingsAccount s) => Console.WriteLine("Savings");
    //
    // TODO 5b: চালাও —
    //            Account acc = new SavingsAccount(100_000m);
    //            Log(acc);                        // কী ছাপল?
    //            Console.WriteLine(acc.MonthlyInterest());   // কারটা চলল?
    //
    // TODO 5c: notes.md এ এক লাইনে লিখো: একই object, একই দুই লাইন —
    //          একটা সিদ্ধান্ত কে নিল আর কীসের ভিত্তিতে, অন্যটা কে নিল আর কীসের ভিত্তিতে?
    //
    // TODO 5d (optional, চোখ খুলে দেবে): PART 5b এর প্রথম লাইনটা বদলে
    //            Log((SavingsAccount)acc);
    //          করো। এখন কী ছাপল? object তো বদলায়নি — শুধু তুমি compiler কে
    //          কী *বলেছ* সেটা বদলেছে। এটাই "ad-hoc = compile time" এর প্রমাণ।


    // ========================================================================
    // PART 6 — optional: sealed
    // ========================================================================
    // TODO 6a: SavingsAccount.MonthlyInterest() কে 'sealed override' করো।
    //          তারপর PremiumSavings : SavingsAccount বানিয়ে ওটা override করার
    //          চেষ্টা করো। compiler কী বলল?
    // TODO 6b: এক লাইনে — কেউ কেন ইচ্ছা করে দরজাটা বন্ধ করতে চাইবে?
    //          (Day 3 এর fragile base class মনে করো।)


    // ------------------------------------------------------------------------
    // যাচাই করার তালিকা — দিন শেষে সবগুলোয় টিক পড়া চাই
    // ------------------------------------------------------------------------
    // [ ] 'new' version এ দুইটা আলাদা সংখ্যা নিজের চোখে দেখেছি
    // [ ] 'override' এ দুইটা এক হয়ে গেছে, আর loop টা ছুঁইনি
    // [ ] abstract না লিখে subclass বানাতে গিয়ে compiler error খেয়েছি
    // [ ] switch version এ একই ভুল করেও compiler এর কাছ থেকে নীরবতা পেয়েছি
    // [ ] overload ফাঁদটা চালিয়ে দেখেছি, আর কেন হলো লিখে রেখেছি
    // [ ] codebase এ `public new ` grep করেছি
}
