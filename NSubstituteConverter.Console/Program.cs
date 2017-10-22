using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstituteConverter.Core;
using NSubstituteConverter.Core.Converters;

namespace NSubstituteConverter.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Init();
            var converter = new SolutionConverter();
            //converter.Convert(@"C:\TTL\web\source\WebComponents\");
            converter.Convert(@"C:\TTL\web\source\WebComponents\JourneyPlanning\test\UnitTests");
            Logger.Log("End ...");
            System.Console.ReadLine();
        }
    }
}
