using System;
using System.Collections.Generic;
using System.Text;

namespace Polymorphism.Bad_Example;

public class BadDemoRunner
{
    public static void Run()
    {
        Account ac = new SavingAccount(50_000m);
        Console.WriteLine(ac.MonthlyInterest());
    }
}
