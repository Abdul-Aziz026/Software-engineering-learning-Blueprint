using System;
using System.Collections.Generic;
using System.Text;

namespace CouplingAndCohesion.Good_Example;

public class Logger
{
    public static void WriteInfo(Filing f)
    {
        //File.AppendAllText(@"C:\logs\filing.txt", $"{DateTime.Now}: filed {f.Id}\n");
        Console.WriteLine("Write log successfully!!!");
    }
}
