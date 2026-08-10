using System;
using System.Collections.Generic;
using System.Text;

namespace CouplingAndCohesion.Good_Example;

public class XmlBuilder
{
    public string BuildXml(Filing f, decimal tax)
    {
        var xml = $"<Filing><Year>{f.TaxYear}</Year><Tax>{tax}</Tax></Filing>";
        Console.WriteLine("Xml Building...");
        Console.WriteLine(xml);
        return xml;
    }
}
