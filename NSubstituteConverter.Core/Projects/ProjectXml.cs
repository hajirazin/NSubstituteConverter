using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NSubstituteConverter.Core.Projects
{
    public class ProjectXml
    {
        private readonly string _projectPath;
        private string _projectFileText;

        public ProjectXml(string projectPath)
        {
            _projectPath = projectPath;
            _projectFileText = File.ReadAllText(projectPath);
            HasRhinoMockReference = _projectFileText.Contains("Rhino.Mocks");
        }

        public bool HasRhinoMockReference { get; }

        public void RemoveRhinoMockReference()
        {
            var xml = XDocument.Parse(_projectFileText.Replace("xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\"", ""));
            xml.Descendants("Reference").Where(f => f.Attribute("Include")?.Value.Contains("Rhino.Mocks") ?? false).Remove();
            var sb = new StringBuilder();
            xml.Save(new StringWriter(sb));
            sb = sb.Replace("DefaultTargets=\"Build\"",
                "DefaultTargets=\"Build\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\"");
            _projectFileText = sb.ToString();
            File.WriteAllText(_projectPath, _projectFileText);
        }

        public void AddNSubstitudeReference()
        {
            var xml = XDocument.Parse(_projectFileText.Replace("xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\"", ""));
            var x = xml.Descendants("ItemGroup").ToList();
            var d = x.FirstOrDefault(f => f.Descendants("Reference").Any());
            if (d != null)
            {
                var xElement = new XElement("Reference",
                    new XElement("HintPath", @"$(SolutionDir)\..\packages\NSubstitute\lib\net45\NSubstitute.dll"));
                xElement.SetAttributeValue("Include", "NSubstitute");
                d.Add(xElement);
                var sb = new StringBuilder();
                xml.Save(new StringWriter(sb));
                _projectFileText = sb.ToString();
            }

            _projectFileText = _projectFileText.Replace("DefaultTargets=\"Build\"",
                "DefaultTargets=\"Build\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\"");
            File.WriteAllText(_projectPath, _projectFileText);
        }
    }
}
