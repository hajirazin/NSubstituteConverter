using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NSubstituteConverter.Core.Converters
{
    public abstract class FileRewritter : CSharpSyntaxRewriter
    {
        public abstract bool IsValidFile(CompilationUnitSyntax root);
    }
}
