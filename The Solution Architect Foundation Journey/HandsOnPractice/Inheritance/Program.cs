using Inheritance;
using Inheritance.Good_Example;

internal class Program
{
    private static void Main(string[] args)
    {
        var brokenStack = new BrokenStack<int>();
        brokenStack.Push(1);
        brokenStack.Push(2);

        brokenStack.Pop();

        brokenStack.Add(10); // broken Stack rules
        brokenStack[0] = 20; // broken Stack rules
        brokenStack.RemoveAt(0); // broken Stack rules



        var safeStack = new SafeStack<int>();
        safeStack.Push(1);
        safeStack.Pop();
        Console.WriteLine(safeStack.Peek());

        // safeStack.Add(10, 20); // compile error


        var readOnlyAudit = new ReadOnlyAuditReport("ReadOnly Audit Report");
        readOnlyAudit.Render();
        readOnlyAudit.Delete(); // inheritance break the rules of ReadOnlyAuditReport


        var game = new Chess();
        game.Start();
    }
}


