
namespace Composition.Bad_Example;

public class CsvToDiskExporter : BadReportExporter
{
    public override void Export(Report report)
    {
        // TODO: csv বানাও (rows কে comma দিয়ে জোড়া দাও)
        // TODO: তারপর "disk এ লিখলাম" Console এ লেখো
        throw new NotImplementedException();
    }
}
