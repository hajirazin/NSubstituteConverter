using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NSubstituteConverter.Core.RhinoMockToNSubstitute.Strategies.Statement
{
    public class Return : IStatementStrategy
    {
        public bool IsEligible(ExpressionStatementSyntax expressionStatement)
        {
            if (!(expressionStatement.Expression is InvocationExpressionSyntax node))
                return false;
            var nodeString = node.ToString();
            if (!(node.Expression is MemberAccessExpressionSyntax smes))
                return false;

            var nodeNameString = smes.Name.ToString();

            return (nodeString.Contains("Expect") || nodeString.Contains("Stub")) &&
                   nodeNameString.Contains("Return");
        }

        public ExpressionStatementSyntax Visit(ExpressionStatementSyntax expressionStatement)
        {
            if (!(expressionStatement.Expression is InvocationExpressionSyntax node)) return expressionStatement;
            var smes = (MemberAccessExpressionSyntax)node.Expression;
            var ies = (InvocationExpressionSyntax)smes.Expression;
            var iex = ((MemberAccessExpressionSyntax)ies.Expression).Expression;

            var innerExpression = ies.ArgumentList.Arguments[0].Expression;
            ExpressionSyntax x = null;
            if (innerExpression is SimpleLambdaExpressionSyntax lambda)
            {
                innerExpression = lambda.Body as ExpressionSyntax;
                if (innerExpression is InvocationExpressionSyntax a)
                {
                    x = a.WithExpression(((MemberAccessExpressionSyntax) a.Expression).WithExpression(iex));
                }
                else if (innerExpression is MemberAccessExpressionSyntax m)
                    x = m.WithExpression(iex);
            }
            else
                x = innerExpression;
            

            if (x != null)
                smes = smes.WithExpression(x).WithName(SyntaxFactory.IdentifierName("Returns"));

            var n = node.WithExpression(smes);
            if (node.ArgumentList.Arguments[0].Expression.ToString().Equals("null"))
            {
                var str = n.ToString();
                str = str.Replace("null", "(object)null");
                return expressionStatement.WithExpression(SyntaxFactory.ParseExpression(str));
            }

            return expressionStatement.WithExpression(n);
        }
    }
}
