using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstituteConverter.Core.RhinoMockToNSubstitude.Strategies;
using NSubstituteConverter.Core.RhinoMockToNSubstitude.Strategies.Invocation;
using NSubstituteConverter.Core.RhinoMockToNSubstitude.Strategies.MemberAccess;

namespace NSubstituteConverter.Core.RhinoMockToNSubstitude
{
    public partial class Rewritter
    {
        private static readonly List<IMemberAccessStrategy> MemberAccessStrategies = new List<IMemberAccessStrategy>
        {
            new MockRepository(),
            new ArgMatches(),
            new ArgAnything(),
            new ArgIsEqual()
        };

        public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            try
            {
                foreach (var strategy in MemberAccessStrategies)
                {
                    if (strategy.IsEligible(node))
                    {
                        var convertedObject = strategy.Visit(node);
                        if (convertedObject is MemberAccessExpressionSyntax m)
                            return base.VisitMemberAccessExpression(m);
                        return convertedObject;
                    }
                }
            }
            catch
            {
                Logger.Log("Exception in VisitMemberAccessExpression", ConsoleColor.Yellow);
            }

            return base.VisitMemberAccessExpression(node);
        }
    }
}
