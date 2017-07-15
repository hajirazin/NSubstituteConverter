using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NSubstituteConverter.Core.Statements
{
    public abstract class Statement
    {
        private readonly string _statement;
        private readonly string _replacement;
        private readonly Regex _statementComparer;
        
        protected Statement(string statement, string pattern, string replacement)
        {
            _statement = statement;
            _replacement = replacement;
            _statementComparer = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        public override string ToString()
        {
            return _statementComparer.Replace(_statement.Replace("\n", ""), _replacement);
        }

        public bool TryParseStatement()
        {
            var match = _statementComparer.Match(this._statement);
            return match.Success;
        }
    }
}
