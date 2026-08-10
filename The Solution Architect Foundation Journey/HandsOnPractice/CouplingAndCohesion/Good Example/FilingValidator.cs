using System;
using System.Collections.Generic;
using System.Text;

namespace CouplingAndCohesion.Good_Example;

public class FilingValidator
{
    public bool Validate(Filing filing)
    {
        // 1. validate
        if (filing.TaxYear < 2000) throw new Exception("bad year");
        if (filing.Amount < 0) throw new Exception("negative");
        if (filing.Country.Length != 2) throw new Exception("bad country");
        Console.WriteLine("File Validated Successfully!!!");
        return true;
    }
}
