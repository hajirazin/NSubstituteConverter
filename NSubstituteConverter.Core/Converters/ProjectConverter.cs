using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NSubstituteConverter.Core.Projects;
using NSubstituteConverter.Core.RhinoMockToNSubstitude;

namespace NSubstituteConverter.Core.Converters
{
    public interface IProjectConverter
    {
        void Convert(Project project);
    }

    public class ProjectConverter : IProjectConverter
    {
        public void Convert(Project project)
        {
            //if (!project.HasRhinoMockReference)
            //    return;

            Logger.Log($"Starting Convertion of project {Path.GetFileNameWithoutExtension(project.ProjectPath)}", ConsoleColor.Green);

            project
                .RemoveRhinoMockReference()
                .AddNSubstitudeReference();

            if (!project.ProjectPath.Contains("test"))
                return;

            Parallel.ForEach(project.Files, projectFile =>
            {
                var fileConverter = new FileConverter(new Rewritter());

                fileConverter.Convert(projectFile);
            });
        }
    }
}
