using System;
using System.Collections.Generic;

namespace NSubstituteConverter.Core.Statements
{
    public class StatementManager
    {
        private static readonly List<Func<string, Statement>> StatementFactories = new List<Func<string, Statement>>();

        static StatementManager()
        {
            // Pass list of statement type thorough which statement needs to be evaluated
            // statement types should be in order of priority.
            var dictionary = new Dictionary<string, string>
            {
                {@"Arg<(.*)>.Is.Anything", "Arg.Any<$1>()" },
                {@"Arg<(.*)>.Matches", "Arg.Is<$1>" },
                {@"Arg<([^>]*)>", "Arg.Is<$1>()" },
                { @".Expect\([^.]*=>[^.]*\.([^.]*)\.Return\((.*)\)", ".$2.Returns($3)"},
                { @"MockRepository.GenerateMock<(.*)>\((.*)\)", "Substitute.For<$1>($2)"},
                { @"using Rhino.Mocks", "using NSubstitute"},
                {@"\.VerifyAllExpectations\(\);", "" }
            };

            foreach (var kvp in dictionary)
            {
                StatementFactories.Add(s => new Statement(s, kvp.Key, kvp.Value));
            }
        }

        public static Statement ParseStatement(string statement)
        {
            foreach (var s in StatementFactories)
            {
                var statementObject = s(statement);
                if (statementObject.TryParseStatement())
                {
                    return statementObject;
                }
            }

            return new InvalidStatement(statement);
        }
    }
}
