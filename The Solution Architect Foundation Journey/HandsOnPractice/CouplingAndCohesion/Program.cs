

using CouplingAndCohesion;
using CouplingAndCohesion.Good_Example;

var filingService = new FilingService(new FilingValidator(), new TaxCalculator(), new SqlFilingStore(), new EmailNotifier(), new XmlBuilder());


var filing = new Filing
{
    UserEmail = "azizulcsebsmrstu@gmail.com",
    TaxYear = 2026,
    Country = "BD",
    Amount = 100
};


filingService.Submit(filing);