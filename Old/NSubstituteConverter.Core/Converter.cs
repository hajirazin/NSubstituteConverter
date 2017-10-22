using System;
using NSubstituteConverter.Core.TestCase;

namespace NSubstituteConverter.Core
{
    public class Converter
    {
        private readonly string _path;

        public Converter(string path)
        {
            _path = path;
        }

        public void Convert()
        {
            var files = new TestCaseFilesBuilder(_path)
                            .WithFileNameContains("test")
                            .Build();

            files.Replace();
           // files.Files.ForEach(f => Console.WriteLine(f.FileContent));
        }
    }
}
