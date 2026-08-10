using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace CouplingAndCohesion.Good_Example;

public class EmailNotifier : INotifier
{
    private readonly SmtpClient _smtp;
    
    public void FilingSubmitted(Filing filing)
    {

        _smtp.send(new MailMessage("noreply@orbitax.com", filing.UserEmail,
                                   "Filed", $"Your {filing.TaxYear} filing is submitted."));
    }
}
