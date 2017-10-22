using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NSubstituteConverter.Core.RhinoMockToNSubstitude.Strategies.Invocation
{
    public interface IInvocationStrategy
    {
        bool IsEligible(InvocationExpressionSyntax node);
        SyntaxNode Visit(InvocationExpressionSyntax node);
    }
}
