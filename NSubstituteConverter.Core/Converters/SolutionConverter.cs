using System.Threading.Tasks;
using NSubstituteConverter.Core.Projects;

namespace NSubstituteConverter.Core.Converters
{
    public class SolutionConverter
    {
        private readonly IProjectConverter _projectConverter;

        public SolutionConverter() : this(new ProjectConverter())
        {
        }

        public SolutionConverter(IProjectConverter projectConverter)
        {
            _projectConverter = projectConverter;
        }

        public void Convert(string solutionFile)
        {
            var solution = new Solution(solutionFile);
            Parallel.ForEach(solution.Projects, project =>
            {
                _projectConverter.Convert(project);
            });
        }
    }
}
