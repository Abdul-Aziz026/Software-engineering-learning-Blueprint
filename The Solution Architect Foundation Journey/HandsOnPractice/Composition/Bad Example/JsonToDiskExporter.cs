namespace Composition.Bad_Example;

public class JsonToDiskExporter : BadReportExporter
{
    public override void Export(Report report)
    {
        throw new NotImplementedException();
    }
}
