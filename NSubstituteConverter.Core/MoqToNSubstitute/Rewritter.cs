using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstituteConverter.Core.Converters;

namespace NSubstituteConverter.Core.MoqToNSubstitute
{
    public partial class Rewritter : FileRewritter
    {
        private static readonly ConcurrentDictionary<string, int> VariableCount = new ConcurrentDictionary<string, int>();

        public override SyntaxNode VisitUsingDirective(UsingDirectiveSyntax node)
        {
            if (node.Name.ToString().Contains("Moq"))
                return node.WithName(SyntaxFactory.IdentifierName("NSubstitute"));

            return base.VisitUsingDirective(node);
        }

        public override SyntaxNode VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            if (node.Type is GenericNameSyntax nodeType && nodeType.Identifier.ValueText == "Mock")
            {
                var n = node.WithType(nodeType.TypeArgumentList.Arguments[0]);
                node = n;
            }

            return base.VisitVariableDeclaration(node);
        }

        public override SyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            if (node.Type is GenericNameSyntax nodeType && nodeType.Identifier.ValueText == "Mock")
            {
                var x = nodeType.WithIdentifier(SyntaxFactory.Identifier("For"));
                var z = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("Substitute"), x);
                var n = SyntaxFactory.InvocationExpression(z, node.ArgumentList);
                return n;
            }

            return base.VisitObjectCreationExpression(node);
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            try
            {
                if (node.Expression is MemberAccessExpressionSyntax memberAccessExpression &&
                    memberAccessExpression.Name.ToString() == "Verify"
                    && node.ArgumentList.Arguments[0].Expression is SimpleLambdaExpressionSyntax lambdaExpression)
                {
                    ExpressionSyntax G(ExpressionSyntax expressionSyntax, ExpressionSyntax argument)
                    {
                        MemberAccessExpressionSyntax memberE;
                        InvocationExpressionSyntax inv;
                        if (argument is InvocationExpressionSyntax invocationExpression)
                        {
                            memberE = invocationExpression.Expression as MemberAccessExpressionSyntax;
                            switch (memberE.Name.ToString())
                            {
                                case "Exactly":
                                    inv = SyntaxFactory.InvocationExpression(
                                        SyntaxFactory.MemberAccessExpression(
                                            node.Expression.Kind(),
                                            expressionSyntax,
                                            SyntaxFactory.Token(SyntaxKind.DotToken),
                                            SyntaxFactory.IdentifierName("Received")), invocationExpression.ArgumentList);
                                    return inv;
                                case "Never":
                                    inv = SyntaxFactory.InvocationExpression(
                                        SyntaxFactory.MemberAccessExpression(
                                            node.Expression.Kind(),
                                            expressionSyntax,
                                            SyntaxFactory.Token(SyntaxKind.DotToken),
                                            SyntaxFactory.IdentifierName("DidNotReceive")), invocationExpression.ArgumentList);
                                    return inv;
                            }
                        }
                        else
                        {
                            memberE = argument as MemberAccessExpressionSyntax;
                            if (memberE != null)
                            {
                                switch (memberE.Name.ToString())
                                {
                                    case "AtLeastOnce":
                                    case "Once":
                                        inv = SyntaxFactory.InvocationExpression(
                                            SyntaxFactory.MemberAccessExpression(
                                                node.Expression.Kind(),
                                                expressionSyntax,
                                                SyntaxFactory.Token(SyntaxKind.DotToken),
                                                SyntaxFactory.IdentifierName("Received")));
                                        return inv;
                                    case "Never":
                                        inv = SyntaxFactory.InvocationExpression(
                                            SyntaxFactory.MemberAccessExpression(
                                                node.Expression.Kind(),
                                                expressionSyntax,
                                                SyntaxFactory.Token(SyntaxKind.DotToken),
                                                SyntaxFactory.IdentifierName("DidNotReceive")));
                                        return inv;
                                }
                            }
                        }
                        return expressionSyntax;
                    }

                    if (lambdaExpression.Body is InvocationExpressionSyntax a)
                    {
                        var n = (MemberAccessExpressionSyntax)a.Expression;
                        var x = a.WithExpression(n.WithExpression(G(memberAccessExpression.Expression,
                            node.ArgumentList.Arguments[1].Expression)));
                        node = x;
                    }
                    else if (lambdaExpression.Body is MemberAccessExpressionSyntax me)
                    {
                        var x = me.WithExpression(G(memberAccessExpression.Expression, node.ArgumentList.Arguments[1].Expression));

                        string GetMethodName()
                        {
                            var nn = node.Parent;
                            while (nn != null)
                            {
                                if (nn is MethodDeclarationSyntax method)
                                    return method.Identifier.ValueText;

                                nn = nn.Parent;
                            }

                            return string.Empty;
                        }

                        var str = x.ToString();
                        var methodName = GetMethodName();
                        VariableCount.GetOrAdd(methodName, -1);
                        VariableCount[methodName]++;
                        var count = VariableCount[methodName];
                        str = $"var x{(count == 0 ? "" : count.ToString())} = " + str;
                        var statement = SyntaxFactory.ParseExpression(str);
                        return statement;
                    }
                }

                if (node.Expression is MemberAccessExpressionSyntax m &&
                    (m.Name.ToString() == "Setup" || m.Name.ToString() == "SetupGet" || m.Name.ToString() == "SetupSet" || m.Name.ToString() == "Expect")
                    && node.ArgumentList.Arguments[0].Expression is SimpleLambdaExpressionSyntax lambda)
                {
                    ExpressionSyntax GetSyntax()
                    {
                        if (node.Parent is MemberAccessExpressionSyntax parent && parent.Name.ToString() != "Returns")
                        {
                            return SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(
                                    node.Expression.Kind(),
                                    m.Expression,
                                    SyntaxFactory.Token(SyntaxKind.DotToken),
                                    SyntaxFactory.IdentifierName("Received")));
                        }

                        return m.Expression;
                    }

                    if (lambda.Body is InvocationExpressionSyntax a)
                    {
                        var n = (MemberAccessExpressionSyntax)a.Expression;
                        var x = a.WithExpression(n.WithExpression(GetSyntax()));
                        node = x;
                    }
                    else if (lambda.Body is MemberAccessExpressionSyntax me)
                    {
                        var x = me.WithExpression(GetSyntax());
                        return x;
                    }
                    else if (lambda.Body is AssignmentExpressionSyntax assignment)
                    {
                        var left = (MemberAccessExpressionSyntax)assignment.Left;
                        return assignment.WithLeft(left.WithExpression(GetSyntax()));
                    }
                }

                if (node.Expression is MemberAccessExpressionSyntax mee &&
                    mee.Expression is IdentifierNameSyntax identifierNameSyntax &&
                    identifierNameSyntax.ToString() == "Mock"
                    && mee.Name is GenericNameSyntax genericNameSyntax &&
                    genericNameSyntax.Identifier.ToString() == "Of")
                {
                    var invocationExpressionSyntax = node.WithExpression(mee.WithExpression(SyntaxFactory.IdentifierName("Substitute"))
                        .WithName(genericNameSyntax.WithIdentifier(SyntaxFactory.Identifier("For"))));
                    return invocationExpressionSyntax;
                }
            }
            catch
            {
                Logger.Log("Exception in VisitMemberAccessExpression", ConsoleColor.Yellow);
            }

            return base.VisitInvocationExpression(node);
        }

        public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            try
            {
                if (node.Expression is IdentifierNameSyntax i && i.ToString().Equals("It") && node.Name is GenericNameSyntax gn)
                {
                    var x = node.WithExpression(SyntaxFactory.IdentifierName("Arg"));
                    if (gn.Identifier.ToString() == "IsAny")
                    {
                        var z = gn.WithIdentifier(SyntaxFactory.Identifier("Any"));
                        var n = x.WithName(z);
                        return n;
                    }

                    if (gn.Identifier.ToString() == "Is")
                    {
                        return x;
                    }
                }

                if (node.Name.ToString() == "Object")
                {
                    return node.Expression;
                }

                if (node.Name.ToString() == "Returns")
                {
                    var n = node;
                    var ies = (InvocationExpressionSyntax)n.Expression;
                    var iex = ((MemberAccessExpressionSyntax)ies.Expression).Expression;

                    if (ies.ArgumentList.Arguments.Count > 0)
                    {
                        var innerExpression = ies.ArgumentList.Arguments[0].Expression;
                        ExpressionSyntax x = null;
                        if (innerExpression is SimpleLambdaExpressionSyntax lambda)
                        {
                            innerExpression = lambda.Body as ExpressionSyntax;
                            if (innerExpression is InvocationExpressionSyntax a)
                            {
                                x = a.WithExpression(((MemberAccessExpressionSyntax)a.Expression).WithExpression(iex));
                            }
                            else if (innerExpression is MemberAccessExpressionSyntax m)
                                x = m.WithExpression(iex);
                        }
                        else
                            x = innerExpression;

                        if (x != null)
                            n = n.WithExpression(x);

                        node = n;
                    }
                }
            }
            catch
            {
                Logger.Log("Exception in VisitMemberAccessExpression", ConsoleColor.Yellow);
            }

            return base.VisitMemberAccessExpression(node);
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            if (node.ReturnType is GenericNameSyntax nodeType && nodeType.Identifier.ValueText == "Mock")
            {
                var n = node.WithReturnType(nodeType.TypeArgumentList.Arguments[0]);
                node = n;
            }

            return base.VisitMethodDeclaration(node);
        }

        public override bool IsValidFile(CompilationUnitSyntax root)
        {
            return root != null && root.Usings.ToList().Any(u => u.Name.ToString().Contains("Moq"));
        }

        public override SyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            var nodeString = node.ToString();
            if (nodeString.Contains("VerifyAll()"))
            {
                return null;
            }

            return base.VisitExpressionStatement(node);
        }

        public override SyntaxNode VisitEmptyStatement(EmptyStatementSyntax node)
        {
            //Simply remove all Empty Statements
            return null;
        }
    }
}
