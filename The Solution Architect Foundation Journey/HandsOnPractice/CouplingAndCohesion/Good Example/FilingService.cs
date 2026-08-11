using CouplingAndCohesion.Good_Example;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Text;

namespace CouplingAndCohesion;
public class FilingService
{
    private readonly FilingValidator _validator;
    private readonly TaxCalculator _calculator;
    private readonly IFilingStore _fileStore;
    private readonly INotifier _notifier;
    private readonly XmlBuilder _xmlBuilder;

    public FilingService(FilingValidator validator, TaxCalculator calculator, IFilingStore fileStore, INotifier notifier, XmlBuilder xmlBuilder)
    {
        _validator = validator;
        _calculator = calculator;
        _fileStore = fileStore;
        _notifier = notifier;
        _xmlBuilder = xmlBuilder;
    }

    public void Submit(Filing filing)
    {
        // 1 Validate
        _validator.Validate(filing);

        // 2 tax calculate
        var tax = _calculator.CalculateTax(filing);


        // 3. build the XML
        var xml = _xmlBuilder.BuildXml(filing, tax);
        

        // 4. save
        _fileStore.Save(filing, tax);

        // 5. notify
       _notifier.FilingSubmitted(filing);

        // 6. log
        Logger.WriteInfo(filing);
    }
}