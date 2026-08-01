
namespace Composition.Bad_Example;

public abstract class BadReportExporter
{
    public abstract void Export(Report report);
}

public class Report
{
    public string Name { get; }
    public List<string[]> Rows { get; } = new();

    public Report(string name) => Name = name;
}