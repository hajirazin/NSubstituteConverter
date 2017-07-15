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
            StatementFactories.Add(s => new CreateMockStatement(s));
            StatementFactories.Add(s => new UsingStatement(s));
            //StatementFactories.Add(s => new ExpectStatement(s));

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
