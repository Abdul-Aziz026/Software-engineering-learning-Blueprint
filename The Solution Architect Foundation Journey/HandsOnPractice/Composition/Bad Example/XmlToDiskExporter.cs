

namespace Composition.Bad_Example;

public class XmlToDiskExporter : BadReportExporter
{
    public override void Export(Report report)
    {
        throw new NotImplementedException();
    }
}
