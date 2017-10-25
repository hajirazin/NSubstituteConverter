﻿using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace NSubstituteConverter.Core.Projects
{
    public class ProjectXml
    {
        private readonly string _projectPath;
        private string _projectFileText;
        private const string BuildNameSpace = "xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\"";
        private const string BuildTarget = "DefaultTargets=\"Build\"";

        public ProjectXml(string projectPath)
        {
            _projectPath = projectPath;
            _projectFileText = File.ReadAllText(projectPath);
            HasRhinoMockReference = _projectFileText.Contains("Rhino.Mocks");
            HasMoqReference = _projectFileText.Contains("Moq");
            HasNSubstituteReference = _projectFileText.Contains("NSubstitute");
        }

        public bool HasRhinoMockReference { get; }
        public bool HasMoqReference { get; }
        public bool HasNSubstituteReference { get; }
        public void RemoveRhinoMockReference()
        {
            RemoveReference("Rhino.Mocks");
        }

        public void RemoveMoqMockReference()
        {
            RemoveReference("Moq");
        }

        private void RemoveReference(string mockString)
        {
            var xml = XDocument.Parse(_projectFileText.Replace(BuildNameSpace, ""));
            xml.Descendants("Reference").Where(f => f.Attribute("Include")?.Value.Contains(mockString) ?? false).Remove();
            var sb = new StringBuilder();
            xml.Save(new StringWriter(sb));
            sb = sb.Replace(BuildTarget, $"{BuildTarget} {BuildNameSpace}");
            _projectFileText = sb.ToString();
            File.WriteAllText(_projectPath, _projectFileText);
        }

        public void AddNSubstituteReference()
        {
            var xml = XDocument.Parse(_projectFileText.Replace(BuildNameSpace, ""));
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

            _projectFileText = _projectFileText.Replace(BuildTarget, $"{BuildTarget} {BuildNameSpace}");
            File.WriteAllText(_projectPath, _projectFileText);
        }
    }
}
