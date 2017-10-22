using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NSubstituteConverter.Core.RhinoMockToNSubstitude.Strategies.Invocation
{
    public class Assert : IInvocationStrategy
    {
        private readonly string _fromString;
        private readonly string _toString;

        public Assert(string fromString, string toString)
        {
            _fromString = fromString;
            _toString = toString;
        }

        public bool IsEligible(InvocationExpressionSyntax node)
        {
            return node.ToString().Contains(_fromString);
        }

        public SyntaxNode Visit(InvocationExpressionSyntax node)
        {
            var body = ((SimpleLambdaExpressionSyntax)node.ArgumentList.Arguments[0].Expression).Body;
            if (body is InvocationExpressionSyntax call)
            {
                var sme = (MemberAccessExpressionSyntax) call.Expression;
                var nodeExpression = ((MemberAccessExpressionSyntax) node.Expression).Expression;
                var x = SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(sme.Kind(),
                    nodeExpression, SyntaxFactory.Token(SyntaxKind.DotToken),
                    SyntaxFactory.IdentifierName(_toString)));
                var d = call.WithExpression(sme.WithExpression(x));
                return d;
            }
            else if (body is AssignmentExpressionSyntax assignment)
            {
                var left = (MemberAccessExpressionSyntax)assignment.Left;
                var nodeExpression = ((MemberAccessExpressionSyntax)node.Expression).Expression;
                var x = SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(left.Kind(),
                    nodeExpression, SyntaxFactory.Token(SyntaxKind.DotToken),
                    SyntaxFactory.IdentifierName(_toString)));
                var d = assignment.WithLeft(left.WithExpression(x));
                return d;
            }
            else if (body is BlockSyntax)
            {
                var nodeString = node.ToString();
                nodeString = nodeString.Replace(_fromString, "When");
                nodeString = nodeString + "." + _toString + "()";
                return SyntaxFactory.ParseExpression(nodeString);
            }

            return node;
        }
    }
}
