using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NSubstituteConverter.Core.RhinoMockToNSubstitute.Strategies.Invocation
{
    public class Ignore : IInvocationStrategy
    {
        public bool IsEligible(InvocationExpressionSyntax node)
        {
            if (!(node.Expression is MemberAccessExpressionSyntax member))
                return false;

            var nodeNameString = member.Name.ToString();
            return nodeNameString.Equals("IgnoreArguments");
        }

        public SyntaxNode Visit(InvocationExpressionSyntax node)
        {
            var nodeString = node.ToString();
            if (nodeString.Contains("ReturnsForAnyArgs") || nodeString.Contains("ReceivedWithAnyArgs"))
            {
                nodeString = nodeString.Replace("IgnoreArguments()", "");
            }

            nodeString = nodeString.Contains("Returns")
                ? nodeString.Replace("IgnoreArguments()", "").Replace("Returns", "ReturnsForAnyArgs")
                : nodeString.Replace("IgnoreArguments", "ReceivedWithAnyArgs");
            return SyntaxFactory.ParseExpression(nodeString);
        }
    }
}