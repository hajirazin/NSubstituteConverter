using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NSubstituteConverter.Core.RhinoMockToNSubstitute.Strategies.Statement
{
    public class Remover : IStatementStrategy
    {
        private static readonly List<string> StatementsToRemove = new List<string>
        {
            ".VerifyAllExpectations",
            ".VerifyAll()",
            ".Replay()",
            ".ReplayAll()",
            "MockRepository()"
        };

        public bool IsEligible(ExpressionStatementSyntax expressionStatement)
        {
            var nodeString = expressionStatement.ToString();
            return StatementsToRemove.Any(s => nodeString.Contains(s));
        }

        public ExpressionStatementSyntax Visit(ExpressionStatementSyntax expressionStatement)
        {
            return null;
        }
    }
}
