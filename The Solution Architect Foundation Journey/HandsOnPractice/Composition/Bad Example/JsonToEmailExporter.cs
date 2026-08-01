
namespace Composition.Bad_Example;

public class JsonToEmailExporter : BadReportExporter
{
    public override void Export(Report report)
    {
        throw new NotImplementedException();
    }
}
