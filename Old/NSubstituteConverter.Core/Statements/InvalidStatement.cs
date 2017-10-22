namespace NSubstituteConverter.Core.Statements
{
    public class InvalidStatement : Statement
    {
        private readonly string _statement;

        public InvalidStatement(string statement)
            : base(statement, "", "")
        {
            _statement = statement;
        }

        public override string ToString()
        {
            return _statement;
        }
    }
}