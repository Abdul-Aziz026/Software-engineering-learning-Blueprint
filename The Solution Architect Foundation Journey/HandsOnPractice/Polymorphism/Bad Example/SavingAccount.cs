using System;
using System.Collections.Generic;
using System.Text;

namespace Polymorphism.Bad_Example;

public class SavingAccount : Account
{
    public SavingAccount(decimal balence) : base(balence)
    {
    }

    public new decimal MonthlyInterest()
    {
        return Balance * 0.05m;
    }
}
