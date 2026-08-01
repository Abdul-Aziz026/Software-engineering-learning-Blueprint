
namespace Composition.Bad_Example;

public class XmlToEmailExporter : BadReportExporter
{
    public override void Export(Report report)
    {
        throw new NotImplementedException();
    }
}
