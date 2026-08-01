
namespace Composition.Good_Example;

public class CsvFormatter : IReportFormatter
{
    public string Format(Report report)
    {
        throw new NotImplementedException();
    }
}

public class Report
{
    public string Name { get; }
    public List<string[]> Rows { get; } = new();

    public Report(string name) => Name = name;
}