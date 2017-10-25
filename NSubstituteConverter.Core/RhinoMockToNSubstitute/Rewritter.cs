using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstituteConverter.Core.Converters;

namespace NSubstituteConverter.Core.RhinoMockToNSubstitute
{
    public partial class Rewritter : FileRewritter
    {
        public override SyntaxNode VisitUsingDirective(UsingDirectiveSyntax node)
        {
            if (node.Name.ToString().Contains("Rhino.Mocks"))
                return node.WithName(SyntaxFactory.IdentifierName("NSubstitute"));

            return base.VisitUsingDirective(node);
        }

        public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            if (node.Declaration.Type.ToString().Equals("MockRepository"))
            {
                return null;
            }

            return base.VisitFieldDeclaration(node);
        }

        public override SyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            var nodeString = node.ToString();
            if (nodeString.Contains("MockRepository()"))
            {
                return null;
            }
            return base.VisitLocalDeclarationStatement(node);
        }

        public override SyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            var nodeString = node.ToString();
            if (nodeString.Contains("VerifyAllExpectations"))
            {
                return null;
            }

            if (nodeString.Contains("VerifyAll()"))
            {
                return null;
            }

            if (nodeString.Contains(".Replay()"))
            {
                return null;
            }

            if (nodeString.Contains(".ReplayAll()"))
            {
                return null;
            }

            if (nodeString.Contains("MockRepository()"))
            {
                return null;
            }

            return base.VisitExpressionStatement(node);
        }

        public override bool IsValidFile(CompilationUnitSyntax root)
        {
            return root != null && root.Usings.ToList().Any(u => u.Name.ToString().Contains("Rhino.Mocks"));
        }

        public override SyntaxNode VisitEmptyStatement(EmptyStatementSyntax node)
        {
            //Simply remove all Empty Statements
            return null;
        }
    }
}
