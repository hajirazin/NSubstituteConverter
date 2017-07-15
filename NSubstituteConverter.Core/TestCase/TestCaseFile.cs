using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NSubstituteConverter.Core.Statements;

namespace NSubstituteConverter.Core.TestCase
{
    public class TestCaseFile
    {
        private readonly string _filePath;
        private readonly string _fileContent;

        public TestCaseFile(string filePath)
        {
            _filePath = filePath;
            _fileContent = File.ReadAllText(filePath);
        }


        public void Replace()
        {
            var statement = StatementManager.ParseStatement(_fileContent);
            if (statement is InvalidStatement)
                return;

            File.WriteAllText(_filePath, statement.ToString());
        }
    }
}