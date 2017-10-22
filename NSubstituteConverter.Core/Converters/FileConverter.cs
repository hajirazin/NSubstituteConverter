using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.Formatting;
using NSubstituteConverter.Core.Projects;

namespace NSubstituteConverter.Core.Converters
{
    public interface IFileConverter
    {
        void Convert(string file);
    }
    public class FileConverter : IFileConverter
    {
        private readonly CSharpSyntaxRewriter _syntaxRewriter;

        public FileConverter(CSharpSyntaxRewriter syntaxRewriter)
        {
            _syntaxRewriter = syntaxRewriter;
        }
        public void Convert(string file)
        {
            Logger.Log($"Starting Convertion of file {Path.GetFileNameWithoutExtension(file)}", ConsoleColor.Red);

            var text = File.ReadAllText(file);
            var syntaxTree = CSharpSyntaxTree.ParseText(text);
            var root = syntaxTree.GetRoot();
            root = _syntaxRewriter.Visit(root);
            var code = Prettify(root);
            File.WriteAllText(file, code);
        }

        private static string Prettify(SyntaxNode root)
        {
            var workspace = new AdhocWorkspace();
            // var rewriter = new EmtpyStatementRemover();
            //root = rewriter.Convert(root);
            var options = workspace.Options;
            options = options.WithChangedOption(CSharpFormattingOptions.IndentBlock, true);
            options = options.WithChangedOption(CSharpFormattingOptions.IndentBraces, false);

            var formattedNode = Formatter.Format(root, workspace, options);
            var formattedString = formattedNode.ToFullString();


            return Regex.Replace(formattedString, @"^\s+$[\r\n]*", "\r\n", RegexOptions.Multiline);
        }
    }
}
