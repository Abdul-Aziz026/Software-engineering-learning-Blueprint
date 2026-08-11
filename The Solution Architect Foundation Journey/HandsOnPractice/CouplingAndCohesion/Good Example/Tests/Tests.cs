using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CouplingAndCohesion.Good_Example.Tests;

public class Tests
{
    public static void CalculateTax()
    {
        var filing = new Filing()
        {
            Country = "BD",
            Amount = 100
        };

        var tax = new TaxCalculator().CalculateTax(filing);
        Assert.Equal(25, tax);
    }
}
