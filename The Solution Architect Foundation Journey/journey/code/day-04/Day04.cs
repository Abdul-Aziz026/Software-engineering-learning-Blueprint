// ============================================================================
// Day 4 of 90 — Composition over Inheritance
// ============================================================================
// নিয়ম: TODO গুলো নিজে হাতে টাইপ করো। copy-paste করলে আজকের পাঠ হারিয়ে যাবে।
// আসল কাজটা করো HandsOnPractice/Composition/ project এ:
//     Bad Example/   -> PART 1
//     Good Example/  -> PART 2 + PART 3
// এই ফাইলটা শুধু roadmap.
// ============================================================================

using System;
using System.Collections.Generic;
using System.IO;

namespace Day04
{
    // ------------------------------------------------------------------------
    // শেয়ার করা domain — এটা যেমন আছে তেমনই থাকবে
    // ------------------------------------------------------------------------
    public class Report
    {
        public string Name { get; }
        public List<string[]> Rows { get; } = new();

        public Report(string name) => Name = name;
    }


    // ========================================================================
    // PART 1 — ❌ BAD: inheritance দিয়ে দুইটা axis সামলানোর চেষ্টা
    // ========================================================================
    // TODO 1a: এই abstract base টা রাখো।
    // TODO 1b: নিচের ৬টা class ই সত্যি সত্যি লেখো। খালি রেখে দিও না।
    //          CSV বানানোর line গুলো প্রতিটা Csv* class এ আবার লেখো — copy করে না,
    //          হাতে। ওই পুনরাবৃত্তির ক্লান্তিটাই আজকের প্রথম পাঠ।
    // TODO 1c: লেখা শেষে গোনো —
    //            (i)  CSV বানানোর logic কতবার লিখতে হলো?
    //            (ii) FTP তে পাঠানোর logic কতবার লিখতে হলো?
    //          উত্তর দুইটা notes.md এ লিখে রাখো।

    public abstract class BadReportExporter
    {
        public abstract void Export(Report report);
    }

    public class CsvToDiskExporter : BadReportExporter
    {
        public override void Export(Report report)
        {
            // TODO: csv বানাও (rows কে comma দিয়ে জোড়া দাও)
            // TODO: তারপর "disk এ লিখলাম" Console এ লেখো
            throw new NotImplementedException();
        }
    }

    // TODO 1b: বাকি ৫টা —
    //   CsvToFtpExporter, CsvToEmailExporter,
    //   XmlToDiskExporter, XmlToFtpExporter, XmlToEmailExporter


    // TODO 1d (শুধু ভাবো, লিখতে হবে না):
    //   JSON format যোগ করলে আর কয়টা class লাগবে?  ______
    //   তারপর S3 destination যোগ করলে মোট কয়টা?      ______


    // TODO 1e: ⭐ আটকে যাওয়ার মুহূর্তটা।
    //   একটা CsvToFtpExporter বানাও। FTP fail করল ধরো।
    //   এখন **একই object টাকে** disk এ লেখাতে বলো।
    //   ...চেষ্টা করো। পারবে না। কেন পারছ না — এক লাইনে notes.md এ লেখো।
    //   এইটাই আজকের সবচেয়ে দামি লাইন।


    // ========================================================================
    // PART 2 — ✅ GOOD: যা বদলায় সেটাকে object বানাও
    // ========================================================================
    // ধারণা: format বদলায়, destination বদলায়। দুইটাই আলাদা "যন্ত্র"।
    //         Exporter টা মিস্ত্রি — ও শুধু যন্ত্র ব্যবহার করে।

    public interface IReportFormatter
    {
        string Format(Report report);
    }

    public interface IExportDestination
    {
        void Send(string content, string fileName);
    }

    // TODO 2a: CsvFormatter : IReportFormatter          (CSV logic — এবার একবারই)
    // TODO 2b: XmlFormatter : IReportFormatter
    // TODO 2c: DiskDestination / FtpDestination / EmailDestination : IExportDestination
    //          প্রতিটার Send() এ শুধু Console.WriteLine করলেই হবে — নকল হোক, সমস্যা নেই।

    public class ReportExporter
    {
        private readonly IReportFormatter _formatter;
        private IExportDestination _destination;      // readonly না — এইটাই আজকের চাবি

        public ReportExporter(IReportFormatter formatter, IExportDestination destination)
        {
            _formatter = formatter;
            _destination = destination;
        }

        public void Export(Report report)
        {
            // TODO 2d: formatter দিয়ে content বানাও, destination দিয়ে পাঠাও।
            //          এই class টা যেন কোনোদিন না জানে CSV কী বা FTP কী।
            throw new NotImplementedException();
        }

        // TODO 2e: এই একটা method inheritance এ লেখাই সম্ভব ছিল না। কেন — ভাবো।
        public void UseDestination(IExportDestination destination) => _destination = destination;
    }

    // TODO 2f: JsonFormatter যোগ করো।
    //          গোনো: কয়টা নতুন class লিখলে?  ______
    //          আর কয়টা নতুন combination পেলে? ______
    //          PART 1 এ একই কাজে কত লাগত?     ______


    // ========================================================================
    // PART 3 — ⭐ runtime swap: আজকের আসল প্রমাণ
    // ========================================================================
    public static class Day04Demo
    {
        public static void Run()
        {
            var report = new Report("Q3-Filing");
            report.Rows.Add(new[] { "Jurisdiction", "Amount" });
            report.Rows.Add(new[] { "BD", "120000" });

            // TODO 3a: FtpDestination.Send() কে জোর করে throw করাও
            //          (ভেতরে: throw new IOException("FTP down");)  // using System.IO;

            // TODO 3b: নিচেরটা চালাও এবং Console এ নিজের চোখে দেখো —
            //          একই object টা প্রথমে FTP তে fail করল, তারপর disk এ লিখল।

            // var exporter = new ReportExporter(new XmlFormatter(), new FtpDestination());
            // try
            // {
            //     exporter.Export(report);
            // }
            // catch (IOException)
            // {
            //     Console.WriteLine("FTP fail. disk এ fallback করছি...");
            //     exporter.UseDestination(new DiskDestination());   // একই object, নতুন যন্ত্র
            //     exporter.Export(report);
            // }

            // TODO 3c: এখন PART 1 এর class গুলো দিয়ে ঠিক এই কাজটা করার চেষ্টা করো।
            //          আটকে যাবে। সেটাই ঠিক আছে — সেটাই আজকের পাঠ।
        }
    }


    // ========================================================================
    // OPTIONAL — trade-off drill (সময় থাকলে)
    // ========================================================================
    // TODO 4a: ReportExporter এর _destination কে readonly করে দাও, UseDestination মুছে দাও।
    //          কী হারালে? কী পেলে?
    //          হিন্ট: Day 1 এর invariant-সুরক্ষা বনাম আজকের নমনীয়তা।
    //          দুইটাই বৈধ design — কোনটা নেবে সেটা নির্ভর করে fallback দরকার কি না তার উপর।
    //          ⚠ এইটাই architect-এর চিন্তা: "কোনটা ভালো" না, "কোন দরকারে কোনটা"।
    //
    // TODO 4b: notes.md এ লেখো — আজকের কোন জায়গায় composition over-engineering হতো?
    //          (হিন্ট: destination যদি কখনোই না বদলাত, তাহলে?)
}
