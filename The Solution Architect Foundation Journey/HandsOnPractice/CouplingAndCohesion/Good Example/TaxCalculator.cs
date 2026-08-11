using System;
using System.Collections.Generic;
using System.Text;

namespace CouplingAndCohesion.Good_Example;

public class TaxCalculator
{
    public decimal CalculateTax(Filing filing)
    {
        // 2. calculate
        decimal tax = filing.Amount * 0.15m;
        if (filing.Country == "BD") tax = filing.Amount * 0.25m;

        Console.WriteLine($"Tax: {tax}");
        return tax;
    }
}
