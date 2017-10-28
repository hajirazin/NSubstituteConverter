using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NSubstituteConverter.Core.RhinoMockToNSubstitute.Strategies.Statement
{
    public interface IStatementStrategy
    {
        bool IsEligible(ExpressionStatementSyntax expressionStatement);
        ExpressionStatementSyntax Visit(ExpressionStatementSyntax expressionStatement);
    }
}
