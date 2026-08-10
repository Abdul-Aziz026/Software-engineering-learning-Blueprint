using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;

namespace CouplingAndCohesion;

public static class Extension
{
    public static void open(this SqlConnection sq)
    {
        Console.WriteLine("SQL Connection Open!");
    }

    public static void close(this SqlConnection sq)
    {
        Console.WriteLine("SQL Connection Closed!");
    }

    public static void executeQuery(this SqlConnection sq)
    {
        Console.WriteLine("Excuting DB Operation...");
    }

    public static void send(this SmtpClient s, MailMessage m)
    {
        Console.WriteLine("Mail Send successfully!!!");
    }
}
