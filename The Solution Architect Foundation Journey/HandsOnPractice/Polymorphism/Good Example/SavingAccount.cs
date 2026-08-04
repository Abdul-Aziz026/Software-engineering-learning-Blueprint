using System;
using System.Collections.Generic;
using System.Text;

namespace Polymorphism.Good_Example;

public class SavingAccount : Account
{
    public SavingAccount(decimal balence) : base(balence)
    {
    }

    public override decimal MonthlyInterest()
    {
        return Balance * 0.05m;
    }
}
