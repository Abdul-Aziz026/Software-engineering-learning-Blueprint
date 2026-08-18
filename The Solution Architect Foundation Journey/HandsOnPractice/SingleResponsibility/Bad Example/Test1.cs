using System;
using System.Collections.Generic;
using System.Text;

namespace SingleResponsibility.Bad_Example;

public class Test1
{
    public void Run()
    {
        var employee = new Employee
        {
            Id = 1,
            Name = "John Doe",
            HourlyRate = 20.0m,
            DailyHours = new int[] { 8, 8, 8, 8, 8, 6, 0 }
        };
        employee.ReportedHours();
    }
}
