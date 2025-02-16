using System;
using System.IO;

namespace VS.Common.DoctestTestAdapter
{
    public class Constants
    {
        public class Package
        {
            public const string GuidString = "d952b4df-4d2a-4549-a5d3-5467ad8762c2";
            public static readonly Guid Guid = new Guid(GuidString);
        }

        public class XmlNodeNames
        {
            // Xml Root Node
            public const string Root = "DoctestTestAdapter";

            // Discovered Executable Nodes
            public const string ExecutableFile = "ExecutableFile";

            // Option Nodes
            public const string Options = "Options";
            public const string GeneralOptions = "General";
            public const string EnableLogging = "EnableLogging";
            public const string CommandArguments = "CommandArguments";
            public const string TestExecutableFilePath = "TestExecutableFilePath";
        }

        public class Options
        {
            public const string XmlFileExtension = ".xml";
            public static readonly string FilePath = Directory.GetCurrentDirectory() + "\\DoctestTestAdapter\\Options" + XmlFileExtension;

            // Name of the options for this test adapter in the tools option window
            public const string ToolsOptionName = "Test Adapter for Doctest";

            // Name of the general category to be shown under the tools option name.
            public const string GeneralCategoryName         = "General";

            // Enable logging.
            public const string LoggingOptionName           = "Enable Logging";
            public const string LoggingOptionDescription    = "Writes doctest test adapter information to a log file.";

            // Command arguments.
            public const string CommandArgumentsOptionName          = "Test Executable Command Arguments";
            public const string CommandArgumentsOptionDescription   = "Command arguments to run the test executables with.";

            // Test executable filepath.
            public const string TestExecutableFilePathOptionName        = "Test Executable File Path";
            public const string TestExecutableFilePathOptionDescription = "Full file path for the test executable to use to run the tests.";
        }
    }
}
