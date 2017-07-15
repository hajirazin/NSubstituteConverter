using System.Collections.Generic;
using System.Linq;

namespace NSubstituteConverter.Core.TestCase
{
    public class TestCaseFiles
    {
        public TestCaseFiles(List<string> files)
        {
            Files = files
                .Select(f => new TestCaseFile(f))
                .ToList();
        }
        public List<TestCaseFile> Files { get; }

        public void Replace()
        {
            Files.ForEach(f => f.Replace());
        }
    }
}
