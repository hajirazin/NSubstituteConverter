﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstituteConverter.Core.Converters;
using NSubstituteConverter.Core.Helpers;

namespace NSubstituteConverter.Core.RhinoMockToNSubstitute
{
    public partial class Rewritter : FileRewritter
    {
        public override SyntaxNode VisitUsingDirective(UsingDirectiveSyntax node)
        {
            if (node.Name.ToString().Equals("Rhino.Mocks"))
                return node.WithName(SyntaxFactory.IdentifierName("NSubstitute"));

            if (node.Name.ToString().Contains("Rhino.Mocks"))
                return null;

            return base.VisitUsingDirective(node);
        }

        public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            if (node.Declaration.Type.ToString().Equals("MockRepository"))
            {
                return null;
            }

            return base.VisitFieldDeclaration(node);
        }

        public override SyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            if (node.Declaration.Type.ToString().Equals("MockRepository"))
            {
                return null;
            }

            var nodeString = node.ToString();
            if (nodeString.Contains("MockRepository()"))
            {
                return null;
            }

            return base.VisitLocalDeclarationStatement(node);
        }

        public override bool IsValidFile(CompilationUnitSyntax root)
        {
            return root != null && root.Usings.ToList().Any(u => u.Name.ToString().Contains("Rhino.Mocks"));
        }

        //public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        //{
        //    var syntax = base.VisitMethodDeclaration(node);
        //    if (syntax is MethodDeclarationSyntax method)
        //    {
        //        var statements = method.Body.Statements;
        //        var childWithReceive = new List<StatementSyntax>();
        //        var otherChild = new List<StatementSyntax>();
        //        foreach (var statementSyntax in statements)
        //        {
        //            var statementString = statementSyntax.ToString();
        //            if (statementString.Contains(".Receive"))
        //            {
        //                try
        //                {
        //                    if (statementString.Contains("Returns"))
        //                    {
        //                        if (!(statementSyntax is ExpressionStatementSyntax expressionStatementSyntax) ||
        //                        !(expressionStatementSyntax.Expression is InvocationExpressionSyntax i) ||
        //                        !(i.Expression is MemberAccessExpressionSyntax mmm))
        //                        {
        //                            otherChild.Add(statementSyntax);
        //                            continue;
        //                        }

        //                        var s = mmm.Expression;
        //                        switch (s)
        //                        {
        //                            case InvocationExpressionSyntax inv when inv.Expression is MemberAccessExpressionSyntax mm:
        //                            {
        //                                var receiveExpression = expressionStatementSyntax.WithExpression(s);
        //                                childWithReceive.Add(receiveExpression);
        //                                var exp = ((MemberAccessExpressionSyntax)((InvocationExpressionSyntax)mm.Expression).Expression).Expression;

        //                                var otherExpression =
        //                                    expressionStatementSyntax.WithExpression(
        //                                        i.WithExpression(
        //                                            mmm.WithExpression(inv.WithExpression(mm.WithExpression(exp)))));
        //                                otherChild.Add(otherExpression);
        //                                break;
        //                            }
        //                            case MemberAccessExpressionSyntax m:
        //                            {
        //                                var z = Mollifier.MakeCompilerHappy(m);
        //                                var receiveExpression = expressionStatementSyntax.WithExpression(z);
        //                                childWithReceive.Add(receiveExpression);
        //                                var exp = ((MemberAccessExpressionSyntax)((InvocationExpressionSyntax)m.Expression).Expression).Expression;

        //                                var otherExpression =
        //                                    expressionStatementSyntax.WithExpression(
        //                                        i.WithExpression(mmm.WithExpression(m.WithExpression(exp))));
        //                                otherChild.Add(otherExpression);
        //                                break;
        //                            }
        //                            default:
        //                                otherChild.Add(statementSyntax);
        //                                break;
        //                        }

        //                    }
        //                    else
        //                    {
        //                        childWithReceive.Add(statementSyntax);
        //                    }
        //                }
        //                catch (Exception e)
        //                {
        //                }
        //            }
        //            else
        //            {
        //                otherChild.Add(statementSyntax);
        //            }
        //        }

        //        var newStatements = new SyntaxList<StatementSyntax>();
        //        otherChild.AddRange(childWithReceive);
        //        newStatements = newStatements.AddRange(otherChild);
        //        method = method.WithBody(method.Body.WithStatements(newStatements));
        //        return method;
        //    }

        //    return syntax;
        //}

        public override SyntaxNode VisitEmptyStatement(EmptyStatementSyntax node)
        {
            //Simply remove all Empty Statements
            return null;
        }
    }
}
