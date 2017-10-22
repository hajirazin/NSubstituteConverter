using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NSubstituteConverter.Core.RhinoMockToNSubstitude.Strategies.MemberAccess
{
    public interface IMemberAccessStrategy
    {
        bool IsEligible(MemberAccessExpressionSyntax node);
        SyntaxNode Visit(MemberAccessExpressionSyntax node);
    }
}
