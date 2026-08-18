
namespace SingleResponsibility.Good_Example;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public int[] DailyHours { get; set; } = new int[7];
}
