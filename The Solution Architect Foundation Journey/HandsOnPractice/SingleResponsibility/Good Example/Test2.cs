using System;
using System.Collections.Generic;
using System.Text;

namespace SingleResponsibility.Good_Example;

public class Test2
{
    private Employee e = new Employee
    {
        Name = "John Doe",
        HourlyRate = 20.0m,
        DailyHours = new int[] { 8, 8, 8, 8, 8, 6, 5 }
    };
    public void Run()
    {
        PayCalculator payCalculator = new PayCalculator();
        payCalculator.CalculatePay(e);
        HourReporter hourReporter = new HourReporter();
        hourReporter.ReportHours(e);
        EmployeeRepository employeeRepository = new EmployeeRepository();
        employeeRepository.Save(e);
    }
}
