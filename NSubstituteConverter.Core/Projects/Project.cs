using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSubstituteConverter.Core.Projects
{
    public class Project
    {
        private readonly ProjectXml _projectXml;

        public Project(string projectFile)
        {
            _projectXml = new ProjectXml(projectFile);
            ProjectPath = projectFile;
        }

        public bool HasRhinoMockReference => _projectXml.HasRhinoMockReference;

        public string ProjectPath { get; }

        public Project RemoveRhinoMockReference()
        {
            _projectXml.RemoveRhinoMockReference();
            return this;
        }

        public Project AddNSubstitudeReference()
        {
            _projectXml.AddNSubstitudeReference();
            return this;
        }

        public List<string> Files
        {
            get
            {
                var directory = Path.GetDirectoryName(ProjectPath);
                if (directory == null) return new List<string>();
                var filePaths = Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories);
                return filePaths.Where(f => !f.Contains("bin") && !f.Contains("obj") && !f.Contains("AssemblyInfo")).ToList();
            }
        }
    }
}
