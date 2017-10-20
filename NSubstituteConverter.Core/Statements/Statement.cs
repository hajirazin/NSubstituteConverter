using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NSubstituteConverter.Core.Statements
{
    public class Statement
    {
        private readonly string _statement;
        private readonly string _replacement;
        private readonly Regex _statementComparer;
        
        public Statement(string statement, string pattern, string replacement)
        {
            _statement = statement;
            _replacement = replacement;
            _statementComparer = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }

        public override string ToString()
        {
            return _statementComparer.Replace(_statement, _replacement);
        }

        public bool TryParseStatement()
        {
            var match = _statementComparer.Match(this._statement);
            return match.Success;
        }
    }
}
