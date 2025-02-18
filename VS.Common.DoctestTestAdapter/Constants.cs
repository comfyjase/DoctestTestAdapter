using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace VS.Common.DoctestTestAdapter
{
    public class Constants
    {
        // Process.StartInfo.Arguments mentions Win7 onwards has a length limit of 32699
        public const int MaxCommandArgumentLength = 32699;

        public class TestAdapter
        {
            // Settings strings
            public const string SettingsName = "DoctestTestAdapter";

            public const string XmlFileExtension = ".xml";
            public static readonly string DiscoveredExecutablesInformationFilePath = Directory.GetCurrentDirectory() + "\\DoctestTestAdapter\\DiscoveredExecutables" + XmlFileExtension;

            // Supported file types
            public const string ExeFileExtension = ".exe";
            public const string DLLFileExtension = ".dll";
            public const string HFileExtension = ".h";
            public const string HPPFileExtension = ".hpp";

            // Custom Test Properties
            // If the test case should be skipped.
            private const string ShouldBeSkippedTestPropertyId = "TestCase.ShouldBeSkipped";
            public static readonly TestProperty ShouldBeSkippedTestProperty = TestProperty.Register(
                ShouldBeSkippedTestPropertyId,
                "ShouldBeSkipped",
                typeof(bool),
                TestPropertyAttributes.Hidden,
                typeof(TestCase));

            // Doctest strings
            public const string TestResultErrorKeyword = "ERROR: ";
            public static readonly List<string> SkipTestKeywords = new List<string>() { "doctest::skip()", "doctest::skip(true)" };
        }

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
            //public const string XmlFileExtension = ".xml";
            //public static readonly string FilePath = Directory.GetCurrentDirectory() + "\\DoctestTestAdapter\\Options" + XmlFileExtension;

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
