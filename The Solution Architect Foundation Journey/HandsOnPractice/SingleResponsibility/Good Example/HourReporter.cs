using System;
using System.Collections.Generic;
using System.Text;

namespace SingleResponsibility.Good_Example;

public class HourReporter
{
    public void ReportHours(Employee e)
    {
        int totalHours = e.DailyHours.Sum();
        Console.WriteLine($"Employee {e.Name} worked {totalHours} hours this week.");
    }
}
