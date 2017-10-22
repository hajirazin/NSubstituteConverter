using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NSubstituteConverter.Core.RhinoMockToNSubstitude.Strategies.MemberAccess
{
    public class ArgAnything : IMemberAccessStrategy
    {
        public bool IsEligible(MemberAccessExpressionSyntax node)
        {
            var nodeString = node.ToString();
            var nodeNameString = node.Name.ToString();
            return nodeString.Contains("Arg") &&
                   (nodeNameString.Contains("Anything") || nodeNameString.Contains("TypeOf"));
        }

        public SyntaxNode Visit(MemberAccessExpressionSyntax node)
        {
            var nodeString = node.ToString();
            nodeString = Regex.Replace(nodeString, "Arg(<[a-zA-Z0-9_ <>\\[\\]]+>).Is.Anything", "Arg.Any$1()");
            nodeString = Regex.Replace(nodeString, "Arg(<[a-zA-Z0-9_ <>\\[\\]]+>).Is.TypeOf", "Arg.Any$1()");
            return SyntaxFactory.ParseExpression(nodeString);
        }
    }
}
