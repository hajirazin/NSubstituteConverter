using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using NSubstituteConverter.Core.Projects;
using RhinoMockVisitor = NSubstituteConverter.Core.RhinoMockToNSubstitute.Rewritter;
using MoqVisitor = NSubstituteConverter.Core.MoqToNSubstitute.Rewritter;

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
            if (!project.HasMoqReference && !project.HasRhinoMockReference)
                return;

            Logger.Log($"Starting Convertion of project {Path.GetFileNameWithoutExtension(project.ProjectPath)}", ConsoleColor.Green);

            if (project.HasRhinoMockReference)
                project.RemoveRhinoMockReference();

            if (project.HasMoqReference)
                project.RemoveMoqMockReference();

            if (!project.HasNSubstituteReference)
                project.AddNSubstituteReference();

            Parallel.ForEach(project.Files, projectFile =>
            {
                FileRewritter rewriter;
                if (project.HasMoqReference)
                {
                    rewriter = new MoqVisitor();
                    var fileConverter = new FileConverter(rewriter);
                    fileConverter.Convert(projectFile);
                }

                if (project.HasRhinoMockReference)
                {
                    rewriter = new RhinoMockVisitor();
                    var fileConverter = new FileConverter(rewriter);
                    fileConverter.Convert(projectFile);
                }
            });
        }
    }
}
