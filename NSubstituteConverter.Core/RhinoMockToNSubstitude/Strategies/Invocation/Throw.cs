using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NSubstituteConverter.Core.RhinoMockToNSubstitude.Strategies.Invocation
{
    public class Throw : IInvocationStrategy
    {
        public bool IsEligible(InvocationExpressionSyntax node)
        {
            return node.ToString().Contains(".Throw(");
        }

        public SyntaxNode Visit(InvocationExpressionSyntax node)
        {
            var s = Regex.Replace(node.ToString(), "Stub", "When", RegexOptions.Singleline);
            s = Regex.Replace(s, "Expect", "When", RegexOptions.Singleline);
            s = Regex.Replace(s, "\\.Throw\\([\r\n ]*([A-Za-z0-9_ ]*)[\r\n ]*(\\((\".*\")*\\))?", ".Do(x => { throw $1$2; }", RegexOptions.Singleline);
            return SyntaxFactory.ParseExpression(s);
        }
    }
}
