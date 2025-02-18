namespace VS.Common.DoctestTestAdapter.Options
{
    public interface ITestAdapterOptions
    {
        bool EnableLogging { get; }

        string CommandArguments { get; }

        string TestExecutableFilePath { get; }
    }
}
