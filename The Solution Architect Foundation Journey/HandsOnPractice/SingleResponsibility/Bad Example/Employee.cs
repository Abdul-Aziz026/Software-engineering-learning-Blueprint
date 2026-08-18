using System;
using System.Collections.Generic;
using System.Text;

namespace SingleResponsibility.Bad_Example;

public class Employee
{
    public static int PayableWeeklyCap = 40;
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public int[] DailyHours { get; set; } = new int[7];

    public decimal CalculatePay()
    {
        int totalHours = RegularHours();
        return (totalHours * HourlyRate);
    }

    public void ReportedHours()
    {
        // Report hours to manager
        Console.WriteLine($"Employee {Name} workded {RegularHours()} hours in this week.");
    }

    public void Save()
    {
        // Save employee to database
        Console.WriteLine($"Employee {Name} saved to database.");
    }

    private int RegularHours()
    {
        int total = 0;
        foreach (var hours in DailyHours)
        {
            total += hours;
        }

        return Math.Min(total, PayableWeeklyCap);
    }
}
