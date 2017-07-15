using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using NSubstituteConverter.Core.Extensions;

namespace NSubstituteConverter.Core.TestCase
{
    public class TestCaseFilesBuilder
    {
        private readonly string _path;
        private Expression<Predicate<string>> _expression;

        public TestCaseFilesBuilder(string path)
        {
            _path = path;
            _expression = s => true;
        }

        public TestCaseFilesBuilder WithFileNameContains(string contains)
        {
            contains = contains.ToLowerInvariant();
            _expression = _expression.AndAlso(s => s.ToLowerInvariant().Contains(contains));
            return this;
        }

        public TestCaseFiles Build()
        {
            var files = ReadFiles();
            var expression = _expression.Compile();
            files = files.FindAll(expression);
            return new TestCaseFiles(files);
        }

        private List<string> ReadFiles()
        {
            var filePaths = Directory.GetFiles(_path, "*.cs", SearchOption.AllDirectories);
            return filePaths.ToList();
        }
    }
}
