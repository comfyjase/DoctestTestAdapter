namespace VS.Common.DoctestTestAdapter
{
    public class Constants
    {
        public class Options
        {
            // Name of the options for this test adapter in the tools option window
            public const string ToolsOptionName = "Test Adapter for Doctest";

            // Name of the general category to be shown under the tools option name.
            public const string GeneralCategoryName = "General";
            public const string StupidOptionsCategoryName = "Checking Stupid Options Are Still Working";

            // Logging option.
            public const string LoggingOptionName = "Enable Logging";
            public const string LoggingOptionDescription = "Writes doctest test adapter information to a log file.";
        }
    }
}
