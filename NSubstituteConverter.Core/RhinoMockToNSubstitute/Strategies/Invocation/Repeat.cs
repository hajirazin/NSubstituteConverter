using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NSubstituteConverter.Core.RhinoMockToNSubstitute.Strategies.Invocation
{
    public class Repeat : IInvocationStrategy
    {
        private readonly Regex _repeatRegex;
        private readonly string _replacement;

        public Repeat(string repeatKeyword, string replacement)
        {
            _repeatRegex = new Regex($"Repeat[\n\r ]*\\.[\n\r ]*{repeatKeyword}[\n\r ]*\\(\\)");
            _replacement = replacement;
        }

        public bool IsEligible(InvocationExpressionSyntax node)
        {
            return _repeatRegex.IsMatch(node.ToString());
        }

        public SyntaxNode Visit(InvocationExpressionSyntax node)
        {
            var nodeString = node.ToString();
            var s = _repeatRegex.Replace(nodeString, nodeString.Contains(_replacement) ? string.Empty : _replacement);
            return SyntaxFactory.ParseExpression(s);
        }
    }
}
