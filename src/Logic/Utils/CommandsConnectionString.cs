namespace Logic.Utils
{
    public class CommandsConnectionString
    {
        public CommandsConnectionString(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }

    public class QueriesConnectionString
    {
        public QueriesConnectionString(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}
