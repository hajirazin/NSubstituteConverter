using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NSubstituteConverter.Core.RhinoMockToNSubstitute.Strategies.MemberAccess
{
    public interface IMemberAccessStrategy
    {
        bool IsEligible(MemberAccessExpressionSyntax node);
        SyntaxNode Visit(MemberAccessExpressionSyntax node);
    }
}
