using System;
using System.Collections.Generic;
using System.Text;

namespace NSubstituteConverter.Core.Statements
{
    public class ExpectStatement : Statement
    {
        public ExpectStatement(string statement) 
            : base(statement, 
                  @"(.*)\.Expect\(.*=>.*\.(.*)\.Return\((.*)\)", 
                  "$1.$2.Returns($3)")
        {
        }
    }
}
