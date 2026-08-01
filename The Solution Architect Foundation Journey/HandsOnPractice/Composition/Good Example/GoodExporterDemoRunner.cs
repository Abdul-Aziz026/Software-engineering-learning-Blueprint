
namespace Composition.Good_Example;

public class GoodExporterDemoRunner
{
    public static void Run()
    {
        var report = new Report("Sales Report, This is the sales report for Q1 2024.");
        IReportFormatter jsonFormatter = new JsonFormatter();
        string formattedReport = jsonFormatter.Format(report);
        IExportDestination destination = new DiskDestination();
        destination.Send(formattedReport, "SalesReport.json");


        IReportFormatter csvFormatter = new CsvFormatter();
        string formattedCsvReport = csvFormatter.Format(report);
        IExportDestination csvDestination = new EmailDestination();
        csvDestination.Send(formattedCsvReport, "SalesReport.csv");
    }
}
