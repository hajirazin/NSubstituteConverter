using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NSubstituteConverter.Core.RhinoMockToNSubstitude.Strategies.Invocation
{
    public class ExpectWithoutReturn : IInvocationStrategy
    {
        public bool IsEligible(InvocationExpressionSyntax node)
        {
            var nodeString = node.ToString();
            return (nodeString.Contains(".Expect") || nodeString.Contains(".Stub")) && !nodeString.Contains("Return");
        }

        public SyntaxNode Visit(InvocationExpressionSyntax node)
        {
            var nodeString = node.ToString();
            
            var syntaxArgument = nodeString.Contains("Ignore") ? "ReceivedWithAnyArgs" : "Received";
            var memberAccessExpressionSyntax = (MemberAccessExpressionSyntax)node.Expression;
            if (memberAccessExpressionSyntax.Name.ToString().Contains("Ignore"))
            {
                node = (InvocationExpressionSyntax)memberAccessExpressionSyntax.Expression;
            }

            memberAccessExpressionSyntax = (MemberAccessExpressionSyntax)node.Expression;
            var expression = memberAccessExpressionSyntax.Expression;
            var syntax = SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    node.Expression.Kind(),
                    expression,
                    SyntaxFactory.Token(SyntaxKind.DotToken),
                    SyntaxFactory.IdentifierName(syntaxArgument)));

            var body = ((SimpleLambdaExpressionSyntax)node.ArgumentList.Arguments[0].Expression).Body;
            if (body is InvocationExpressionSyntax invocationExpressionSyntax)
            {
                var a = ((MemberAccessExpressionSyntax)invocationExpressionSyntax.Expression).WithExpression(syntax);

                return invocationExpressionSyntax.WithExpression(a);
            }
            if (body is AssignmentExpressionSyntax assignment)
            {
                var left = (MemberAccessExpressionSyntax)assignment.Left;
                return assignment.WithLeft(left.WithExpression(syntax));
            }

            return node;
        }

        private SyntaxNode RemoveIgnore(SyntaxNode syntaxNode)
        {
            var nodeString = syntaxNode.ToString();
            if (!nodeString.Contains("IgnoreArguments()"))
                return syntaxNode;

            nodeString = nodeString.Replace(".IgnoreArguments()", "");
            return SyntaxFactory.ParseExpression(nodeString);
        }
    }
}
