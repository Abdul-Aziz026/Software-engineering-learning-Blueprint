
using System.Data.SqlClient;
using System.Net.Mail;
using Microsoft.Data;

namespace CouplingAndCohesion.Bad_Example;

public class FilingService
{
    private readonly SqlConnection _db = new SqlConnection("connection string...");
    private readonly SmtpClient _smtp = new SmtpClient("smtp.orbitax.com");

    public void Submit(Filing filing)
    {
        // 1. validate
        if (filing.TaxYear < 2000) throw new Exception("bad year");
        if (filing.Amount < 0) throw new Exception("negative");
        if (filing.Country.Length != 2) throw new Exception("bad country");

        // 2. calculate
        decimal tax = filing.Amount * 0.15m;
        if (filing.Country == "BD") tax = filing.Amount * 0.25m;

        // 3. build the XML
        var xml = $"<Filing><Year>{filing.TaxYear}</Year><Tax>{tax}</Tax></Filing>";

        // 4. save
        _db.Open();
        new SqlCommand($"INSERT INTO Filings VALUES ('{xml}')", _db).ExecuteNonQuery();
        _db.Close();

        // 5. notify
        _smtp.Send(new MailMessage("noreply@orbitax.com", filing.UserEmail,
                                   "Filed", $"Your {filing.TaxYear} filing is submitted."));

        // 6. log
        File.AppendAllText(@"C:\logs\filing.txt", $"{DateTime.Now}: filed {filing.Id}\n");
    }
}
