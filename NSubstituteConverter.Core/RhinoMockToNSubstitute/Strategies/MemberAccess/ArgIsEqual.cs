﻿using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NSubstituteConverter.Core.RhinoMockToNSubstitute.Strategies.MemberAccess
{
    public class ArgIsEqual : IMemberAccessStrategy
    {
        public bool IsEligible(MemberAccessExpressionSyntax node)
        {
            var nodeString = node.ToString();
            var nodeNameString = node.Name.ToString();
            return nodeString.Contains("Arg") && 
                (nodeNameString.Contains("Equal") || nodeNameString.Contains("Same") || nodeNameString.Contains("Null"));
        }

        public SyntaxNode Visit(MemberAccessExpressionSyntax node)
        {
            var nodeString = node.ToString();
            nodeString = Regex.Replace(nodeString, "Arg(<[a-zA-Z0-9,_ <>\\[\\]]+>).Is.Equal", "Arg.Is");
            nodeString = Regex.Replace(nodeString, "Arg(<[a-zA-Z0-9,_ <>\\[\\]]+>).Is.Same", "Arg.Is");
            nodeString = Regex.Replace(nodeString, "Arg<([a-zA-Z0-9,_ <>\\[\\]]+)>.Is.Null", "Arg.Is(($1)null)");
            nodeString = Regex.Replace(nodeString, "Arg<([a-zA-Z0-9,_ <>\\[\\]]+)>.Is.NotNull", "Arg.Is<$1>(a => a != null)");
            return SyntaxFactory.ParseExpression(nodeString);
        }
    }
}
