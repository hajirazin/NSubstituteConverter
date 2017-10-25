using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstituteConverter.Core.RhinoMockToNSubstitute.Strategies.Invocation;

namespace NSubstituteConverter.Core.RhinoMockToNSubstitute
{
    public partial class Rewritter
    {
        private static readonly List<IInvocationStrategy> InvocationStrategies = new List<IInvocationStrategy>
        {
            new Throw(),
            new WhenCalled(),
            new Assert("AssertWasCalled", "Received"),
            new Assert("AssertWasNotCalled", "DidNotReceive"),
            new ReturnWithIgnore(),
            new Return(),
            new ExpectWithoutReturn(),
            new Ignore(),
            new Repeat("Any", "Received()" ),
            new Repeat("Twice", "Received(2)" ),
            new Repeat("AtLeastOnce", "Received(1)" ),
            new Repeat("Once", "Received(1)" ),
            new Repeat("Never", "DidNotReceive()" )
        };

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            try
            {
                foreach (var strategy in InvocationStrategies)
                {
                    try
                    {
                        if (strategy.IsEligible(node))
                        {
                            var convertedObject = strategy.Visit(node);
                            if (convertedObject is InvocationExpressionSyntax m)
                            {
                                node = m;
                                continue;
                            }

                            return convertedObject;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("Exception in VisitInvocationExpression", ConsoleColor.Yellow);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Exception in VisitInvocationExpression", ConsoleColor.Yellow);
            }

            return base.VisitInvocationExpression(node);
        }
    }
}
