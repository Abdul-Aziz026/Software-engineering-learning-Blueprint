using System;
using System.Collections.Generic;
using System.Text;

namespace Composition.Bad_Example;

public class BadExporterDemoRunner
{
    public static void Run()
    {
        var report = new Report("Sample Report");
        var jsonExporter = new JsonToDiskExporter();
        jsonExporter.Export(report);
        var xmlExporter = new XmlToEmailExporter();
        xmlExporter.Export(report);
        var jsonEmailExporter = new JsonToEmailExporter();
        jsonEmailExporter.Export(report);
    }
}
