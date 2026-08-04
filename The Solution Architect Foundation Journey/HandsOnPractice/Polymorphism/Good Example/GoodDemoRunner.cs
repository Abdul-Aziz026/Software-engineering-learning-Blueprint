using System;
using System.Collections.Generic;
using System.Text;

namespace Polymorphism.Good_Example;

public class GoodDemoRunner
{
    public static void Run()
    {
        Account ac = new SavingAccount(50_000m);
        Console.WriteLine(ac.MonthlyInterest());
    }
}
