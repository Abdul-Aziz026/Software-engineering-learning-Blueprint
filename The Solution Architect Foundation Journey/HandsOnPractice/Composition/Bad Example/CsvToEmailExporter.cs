
namespace Composition.Bad_Example;

public class CsvToEmailExporter : BadReportExporter
{
    public override void Export(Report report)
    {
        throw new NotImplementedException();
    }
}
