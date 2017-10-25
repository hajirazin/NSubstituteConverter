using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NSubstituteConverter.Core.RhinoMockToNSubstitute.Strategies.Invocation
{
    public class WhenCalled : IInvocationStrategy
    {
        public bool IsEligible(InvocationExpressionSyntax node)
        {
            var nodeString = node.ToString();
            return nodeString.Contains("WhenCalled") && (nodeString.Contains("Expect") || nodeString.Contains("Stub"));
        }

        public SyntaxNode Visit(InvocationExpressionSyntax node)
        {
            var nodeString = node.ToString();
            nodeString = nodeString
                .Replace("Expect", "When")
                .Replace("Stub", "When")
                .Replace("WhenCalled", "Do");
            return SyntaxFactory.ParseExpression(nodeString);
        }
    }
}
