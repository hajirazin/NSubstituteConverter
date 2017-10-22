using System;
using System.Linq;
using NSubstituteConverter.Core;

namespace NSubstituteConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            string path;
            if (args.Any())
                path = args[0];
            else
            {
                //Console.WriteLine("Enter unit test cases directory path");
                //path = Console.ReadLine();
                path = @"C:\TTL\web\source\WebComponents\Services\ETicketAvailabilityRules\ETicketAvailabilityRules\test";
            }

            var converter = new Converter(path);
            converter.Convert();
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }
    }
}