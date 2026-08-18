# JOURNEY_LOG.md

## Day 1
- **Date**: 2026-07-24 (Wednesday)
- **Topic**: Encapsulation as invariant protection
- **Estimated Time**: [≤1hr]
- **Actual Time**: 1h
- **Takeaway**: Encapsulation protects object state and enforces invariants.

## Day 2
- **Date**: 2026-07-25 (Thursday)
- **Topic**: Abstraction design (good vs bad interfaces)
- **Estimated Time**: [≤1hr]
- **Actual Time**: 1h
- **Takeaway**: Good abstractions hide implementation details and expose intent.

## Day 3
- **Date**: 2026-07-31 (Friday)
- **Topic**: Inheritance — when to use, when NOT ("is-a" test)
- **Estimated Time**: [≤1hr]
- **Actual Time**:
- **Takeaway**: Inheritance is adoption, not borrowing — the subclass takes the base's entire public surface and every future addition. Test with behavioural substitutability, not English vocabulary.

## Day 4
- **Date**: 2026-08-01 (Saturday)
- **Topic**: Composition over Inheritance — same feature built both ways
- **Estimated Time**: [≤1hr]
- **Actual Time**:
- **Takeaway**: Inheritance welds behaviour into the type at compile time; composition makes behaviour a field, so it can be swapped at runtime. Two independent axes of variation grow as `M × N` with inheritance but `M + N` with composition — and inheritance still wins when the base's job is to *enforce a rule*, not to hand out behaviour.

## Day 5
- **Date**: 2026-08-02 (Sunday)
- **Topic**: Polymorphism — subtype vs ad-hoc, and the vtable behind `override`
- **Estimated Time**: [≤1hr]
- **Actual Time**:
- **Takeaway**: `override` seizes the base's vtable slot; `new` builds a slot beside it and leaves the base's untouched — which is why one object gives two different answers depending on the *declared* type of the reference. Overload is chosen by the compiler from what you declared; override is chosen by the CLR from what the object actually is. And the real culprit is rarely the `new` keyword — it's the fake default in the base (`return 0m`) that should have been `abstract`, so the compiler stops you instead of the customer.

## Day 6
- **Date**: 2026-08-05 (Wednesday)
- **Topic**: Coupling & Cohesion — the two rulers behind all of SOLID
- **Estimated Time**: [≤1hr]
- **Actual Time**:
- **Takeaway**: Cohesion is the inside question (do these things belong to each other?), coupling is the outside question (who breaks when I break?). Cut along *"who asks for the change"*, not along *"what the code does"* — things that read together aren't necessarily things that change together. The honest measuring instrument for coupling is the unit test: whatever you must stand up to test a class **is** its coupling, so "this is hard to test" is never a testing complaint, it's a design diagnosis. And coupling can never be zero — only moved from volatile to stable: depend downward, on things that change more slowly than you do. Over-splitting (three one-rule validator classes that all change for the same compliance reason) is low cohesion wearing a different costume.

## Day 7 — 🔁 Retrieval Day (Week 1)
- **Date**: 2026-08-11 (Tuesday)
- **Topic**: Retrieval / self-test — Days 1–6 (Encapsulation, Abstraction, Inheritance, Composition, Polymorphism, Coupling & Cohesion)
- **Estimated Time**: [45–60 min]
- **Actual Time**:
- **Format**: No new material. 43 recall questions + 6 code-judgment snippets + 5 architect-judgment prompts at the top; answers in a separate section at the bottom for honest self-testing.
- **Takeaway**: Week 1 is six versions of one question — *কে কার উপর ক্ষমতা রাখে?* Days 1–2 ask it inside one class (who may touch the state, what does the outside see), Days 3–5 ask it between two types (how much power is the base handing out, whose version actually runs), Day 6 asks it across the whole system (who breaks when I break). Retrieval itself is the lesson today: recognition is not recall, so the answers were deliberately quarantined at the bottom — reading them early converts a memory test into a reading exercise. The trap question (C6, a plain DTO that needs no fixing) exists because after six days of "here is the bad version," the reflex to refactor everything is itself the next junior mistake.

## Day 8
- **Date**: 2026-08-14 (Friday)
- **Topic**: SRP — "one reason to change" = one **actor** to answer to
- **Estimated Time**: [≤1hr]
- **Actual Time**:
- **Takeaway**: SRP-এর "reason" মানে কোনো technical কারণ না — মানে **একজন মানুষ**; Uncle Bob-এর শোধরানো সংজ্ঞা হলো *responsible **to** one actor*, *responsible **for** one task* না। তাই কাঁচি চালাতে হয় actor গুনে, method গুনে না — যে কারণে `BankAccount`-এর তিনটা method এক actor-এর হওয়ায় ভাঙা **ভুল**, আর তিন actor-এর `Employee` ভাঙা **ঠিক**। আজকের আসল আবিষ্কার: দুই actor একটা shared private helper (`RegularHours()`) ভাগ করলে একজনের অনুরোধে আরেকজনের সংখ্যা **নীরবে** বদলে যায় — compiler চুপ, test সবুজ, শুধু report ভুল (accidental duplication)। তাই DRY-এর মানে "একরকম দেখতে কোড দুইবার থাকবে না" না, "একই *জ্ঞান* দুই জায়গায় থাকবে না" — পরীক্ষা: *একজনের অনুরোধে বদলালে অন্যজনও কি সেই বদলটাই চাইত?* না হলে ওটা duplication না, coincidence। Invariant entity-র ভেতরে থাকে, policy বাইরে যায়; আর class-boundary org-chart-এর মতো দেখতে হওয়াটা কাকতালীয় না — এটাই Conway's Law, আর microservice-এর সীমা টানার প্রশ্নটাও হুবহু একই প্রশ্ন।
