using System;
using System.Collections.Generic;
using System.Text;

namespace NSubstituteConverter.Core.Statements
{
    public class UsingStatement : Statement
    {
        public UsingStatement(string statement) 
            : base(statement, "using Rhino.Mock", "using NSubstitute")
        {
        }
    }
}
