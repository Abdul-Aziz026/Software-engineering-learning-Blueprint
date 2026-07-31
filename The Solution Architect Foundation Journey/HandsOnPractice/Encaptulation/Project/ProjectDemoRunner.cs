using System;
using System.Collections.Generic;
using System.Text;

namespace Encaptulation.Project;

public class ProjectDemoRunner
{
    public static void Run()
    {
        try
        {
            var capped = new CappedBankAccount(200_000m, new SystemClock());
            capped.Withdraw(30_000m);
            capped.Withdraw(30_000m);   // should breach the 50k daily cap
            Console.WriteLine("  ❌ daily cap was NOT enforced — invariant #2 is broken");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✅ daily cap enforced: {ex.Message}");
        }
    }
}
