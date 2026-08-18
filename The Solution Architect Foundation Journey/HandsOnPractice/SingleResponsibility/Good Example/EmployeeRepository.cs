using System;
using System.Collections.Generic;
using System.Text;

namespace SingleResponsibility.Good_Example;

public class EmployeeRepository
{
    public void Save(Employee e)
    {
        // Save employee to database
        Console.WriteLine($"Employee {e.Name} saved to database.");
    }
}
