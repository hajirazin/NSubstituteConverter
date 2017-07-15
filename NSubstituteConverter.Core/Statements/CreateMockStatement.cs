namespace NSubstituteConverter.Core.Statements
{
    internal class CreateMockStatement : Statement
    {
        public CreateMockStatement(string statement)
            : base(statement, @"MockRepository.GenerateMock<(.*)>\((.*)\)", "Substitute.For<$1>($2)")
        {
        }
    }
}