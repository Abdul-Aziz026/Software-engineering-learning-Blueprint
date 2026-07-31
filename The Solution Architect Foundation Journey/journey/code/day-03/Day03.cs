// =====================================================================================
// Day 03 — Inheritance: when to use, and when NOT. The "is-a" test.
//
// Run it:  dotnet run           (from journey/code/day-03, after `dotnet new console`)
// or paste into LINQPad / any scratch console project.
//
// Rule for today:  inheritance is not code borrowing. It is adoption —
//                  you take the family name AND every debt, forever.
// =====================================================================================

using System;
using System.Collections.Generic;

namespace Day03
{
    // =================================================================================
    // TASK 1 — BREAK IT.
    //
    // This compiles. It even passes a naive test. It is still wrong.
    // Your job: run Demo.Task1_SeeItBreak() and watch the LIFO invariant die.
    // =================================================================================
    public class BrokenStack<T> : List<T>
    {
        public void Push(T item) => Add(item);

        public T Pop()
        {
            var item = this[Count - 1];
            RemoveAt(Count - 1);
            return item;
        }

        public T Peek() => this[Count - 1];

        // TODO (write it down, don't just think it):
        // Name three methods this class inherited that make no sense on a Stack,
        // and say which one you'd be most scared of a teammate calling.
        //   1. Add()
        //   2. RemoveAt()
        //   3. this[index]
    }


    // =================================================================================
    // TASK 2 — FIX IT with composition.
    //
    // Stack has-a List. It is not a List.
    // Type the body yourself — muscle memory is the point, not reading.
    // =================================================================================
    public class SafeStack<T>
    {
        private readonly List<T> _items = new();

        public int Count => _items.Count;

        public void Push(T item)
        {
            // TODO: add to the end of _items
            _items.Add(item);
        }

        public T Pop()
        {
            // TODO: guard empty (throw InvalidOperationException),
            //       then take the LAST item and remove it.
            if (Count == 0)
                throw new InvalidOperationException("Stack is empty");

            var item = this[Count - 1];
            _items.RemoveAt(Count - 1);
            return item;
        }

        public T Peek()
        {
            // TODO: guard empty, return the last item WITHOUT removing it.
            if (Count == 0)
                throw new InvalidOperationException("Stack is empty");

            return this[Count - 1];
        }

        // TODO: now try to write   public void Insert(int index, T item)
        // ...and then ask yourself why you would ever want to.
        // Delete it. That deletion is today's whole lesson.

        // Answer:- No Insert or Delete method implementation here because a Stack is not a List. 
        // It is a Stack. LIFO. End of story. 
    }


    // =================================================================================
    // TASK 3 — FEEL the fragile base class problem.
    //
    // Step 1: run Demo.Task3_FragileBase() as-is. Fine.
    // Step 2: add this method to LegacyReport:
    //
    //             public void Delete() => Console.WriteLine($"{Title} DELETED");
    //
    // Step 3: go look at ReadOnlyAuditReport. You did not touch that file.
    //         It can now be deleted. An audit record. That must never be deleted.
    //
    // Nobody reviewed that. Nobody approved it. The `:` did it.
    // =================================================================================
    public class LegacyReport
    {
        public string Title { get; }

        public LegacyReport(string title) => Title = title;

        public virtual void Render() => Console.WriteLine($"[report] {Title}");

        public void Export() => Console.WriteLine($"[export] {Title}.pdf");

        public void Delete() => Console.WriteLine($"{Title} DELETED");
    }

    public class ReadOnlyAuditReport : LegacyReport
    {
        public ReadOnlyAuditReport(string title) : base(title) { }

        public override void Render() => Console.WriteLine($"[audit·immutable] {Title}");

        // Notice: this class says nothing about deleting.
        // It will inherit Delete() anyway. Silently.

        // wrong inheritance
    }

    // =================================================================================
    // BONUS — what GOOD inheritance looks like.
    //
    // Base is abstract. Base holds almost no state. Base exists to ENFORCE A RULE
    // ("never submit something that failed validation"), not to hand out free code.
    // One level deep. Small public surface.
    //
    // Nothing to do here today — just read it and notice how different it feels.
    // (Its name is Template Method. You'll meet it properly on Day 49.)
    // =================================================================================
    
    public abstract class Game
    {
        // Shared rule (cannot be overridden)
        public void Start()
        {
            LoadResources();   // Always happens first
            Play();            // Game-specific behavior
        }

        private void LoadResources()
        {
            Console.WriteLine("Loading resources...");
        }

        protected abstract void Play();
    }

    public class Chess : Game
    {
        protected override void Play()
        {
            Console.WriteLine("Playing Chess");
        }
    }

    public class Football : Game
    {
        protected override void Play()
        {
            Console.WriteLine("Playing Football");
        }
    }


    // =================================================================================
    // DEMO
    // =================================================================================
    public static class Demo
    {
        public static void Main()
        {
            Task1_SeeItBreak();
            Console.WriteLine();

            // Task 2 is yours to implement — until then it throws, so it must not
            // block the rest of the demo from running.
            try
            {
                Task2_SeeItHold();
            }
            catch (NotImplementedException)
            {
                Console.WriteLine("── TASK 2: not implemented yet — go fill in SafeStack<T>. ──");
            }

            Console.WriteLine();
            Task3_FragileBase();
            Console.WriteLine();
            Bonus_GoodInheritance();
        }

        public static void Task1_SeeItBreak()
        {
            Console.WriteLine("── TASK 1: BrokenStack : List<T> ──");

            var s = new BrokenStack<string>();
            s.Push("a");
            s.Push("b");
            s.Push("c");

            Console.WriteLine($"Pop() → {s.Pop()}          (correct: c)");

            // Every one of these compiles. Every one of these is a knife in the LIFO rule.
            s.Insert(0, "sneaked-in");
            s.Reverse();
            s[0] = "overwritten";

            Console.WriteLine($"Pop() → {s.Pop()}          (LIFO says this should be 'b')");
            Console.WriteLine("The compiler never objected. That is the problem.");
        }

        public static void Task2_SeeItHold()
        {
            Console.WriteLine("── TASK 2: SafeStack (composition) ──");

            var s = new SafeStack<string>();
            s.Push("a");
            s.Push("b");

            Console.WriteLine($"Pop() → {s.Pop()}          (b)");
            Console.WriteLine($"Peek() → {s.Peek()}        (a)");
            Console.WriteLine("There is no Insert to call. LIFO is now guaranteed at compile time.");
        }

        public static void Task3_FragileBase()
        {
            Console.WriteLine("── TASK 3: fragile base class ──");

            LegacyReport audit = new ReadOnlyAuditReport("Q3 audit trail");
            audit.Render();
            audit.Export();

            // After Step 2, uncomment this. It will compile. Sit with that.
            // audit.Delete();
        }

        public static void Bonus_GoodInheritance()
        {
            Console.WriteLine("── BONUS: inheritance done right ──");

            Game game = new Chess();
            game.Start();
        }
    }
}
