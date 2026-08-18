using System;
using System.Collections.Generic;
using System.Text;

namespace SingleResponsibility.Good_Example;

public class PayCalculator
{
    private int weeklyPayHourCap = 40;
    public void CalculatePay(Employee e)
    {
        decimal totalHours= Math.Min(weeklyPayHourCap, e.DailyHours.Sum());
        decimal totalPay = totalHours * e.HourlyRate;
        Console.WriteLine($"Employee {e.Name} is owed ${totalPay:F2} this pay period.");
    }
}
