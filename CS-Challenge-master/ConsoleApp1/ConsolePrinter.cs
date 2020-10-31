using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    public class ConsolePrinter
    {
        public static object PrintValue;

        public ConsolePrinter Value(string value)
        {
            PrintValue = value;
            return this;
        }

        public override string ToString()
        {
            Console.WriteLine(Environment.NewLine + PrintValue + Environment.NewLine);
            return null;
        }
    }
}
