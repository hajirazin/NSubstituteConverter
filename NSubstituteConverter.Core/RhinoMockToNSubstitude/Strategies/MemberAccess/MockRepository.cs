using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NSubstituteConverter.Core.RhinoMockToNSubstitude.Strategies.MemberAccess
{
    public class MockRepository : IMemberAccessStrategy
    {
        private static readonly Regex Expression = new Regex("[A-Za-z0-9_]*\\.(Generate|Strict|GenerateStrict|Dynamic|GenerateDynamic|Partial|GeneratePartial)(Mock|Stub)",
            RegexOptions.Singleline | RegexOptions.IgnoreCase); 

        public bool IsEligible(MemberAccessExpressionSyntax node)
        {
            var nodeString = node.ToString();
            return Expression.IsMatch(nodeString);
        }

        public SyntaxNode Visit(MemberAccessExpressionSyntax node)
        {
            var nodeString = node.ToString();
            nodeString = Regex.Replace(nodeString, "MockRepository\\.GenerateStub", "Substitute.For");
            nodeString = Regex.Replace(nodeString, "MockRepository\\.GenerateMock", "Substitute.For");
            nodeString = Regex.Replace(nodeString, "MockRepository\\.GenerateStrictMock", "Substitute.For");
            nodeString = Expression.Replace(nodeString, "Substitute.For");
            return SyntaxFactory.ParseExpression(nodeString);
        }
    }
}
