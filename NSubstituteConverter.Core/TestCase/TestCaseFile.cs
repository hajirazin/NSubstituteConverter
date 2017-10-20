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
        private readonly List<string> _fileContent;

        public TestCaseFile(string filePath)
        {
            _filePath = filePath;
            var lines = File.ReadAllLines(filePath);
            _fileContent = new List<string>();
            for (var index = 0; index < lines.Length; index++)
            {
                _fileContent.Add(RemoveEntersFromCode(lines, ref index));
            }
        }

        private static string RemoveEntersFromCode(IReadOnlyList<string> lines, ref int index)
        {
            if (lines.Count <= index)
                return string.Empty;

            var line = lines[index];
            if (string.IsNullOrWhiteSpace(line))
                return line;

            line = line.Trim();
            if (line.EndsWith(";") || line.EndsWith("]") || line.EndsWith("{") || line.EndsWith("}"))
                return line;

            index++;
            return line + RemoveEntersFromCode(lines, ref index);
        }

        public void Replace()
        {
            for (var index = 0; index < _fileContent.Count; index++)
            {
                var fileLine = _fileContent[index];
                var statement = StatementManager.ParseStatement(fileLine);
                if (statement is InvalidStatement)
                    continue;

                _fileContent[index] = statement.ToString();

            }

            File.WriteAllLines(_filePath, _fileContent);
        }
    }
}