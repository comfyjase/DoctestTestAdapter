using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;

namespace VS2022.DoctestTestAdapter
{
    public static class DoctestTestAdapterConstants
    {
        // Executor strings/URI
        public const string ExecutorUriString =  "executor://DoctestTestExecutor/v1";
        public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);

        // Settings strings
        public const string SettingsName                = "DoctestTestAdapter";
        public const string OutputFileNodeName          = "OutputFile";
        public const string ProjectFilePathNodeName     = "ProjectFilePath";
        public const string ExecutableFilePathNodeName  = "ExecutableFilePath";
        public const string CommandArgumentsNodeName    = "CommandArguments";

        // Supported file types
        public const string ExeFileExtension    = ".exe";
        public const string DLLFileExtension    = ".dll";
        public const string HFileExtension      = ".h";
        public const string HPPFileExtension    = ".hpp";

        // Custom Test Properties
        // If the test case should be skipped.
        private const string ShouldBeSkippedTestPropertyId = "TestCase.ShouldBeSkipped";
        public static readonly TestProperty ShouldBeSkippedTestProperty = TestProperty.Register(
            ShouldBeSkippedTestPropertyId, 
            "ShouldBeSkipped",
            typeof(bool), 
            TestPropertyAttributes.Hidden, 
            typeof(TestCase));

        // Dictionary of executables available in the current solution.
        // Key: Output file path (e.g. Path\To\game.exe or Path\To\library.dll)
        // Value: Exeuctable to run for test explorer: (e.g. for Path\To\library.dll, it should run Path\To\game.exe because this loads Path\To\library.dll)
        private const string ExecutablesTestPropertyId = "TestCase.Executables";
        public static readonly TestProperty ExecutablesTestProperty = TestProperty.Register(
            ExecutablesTestPropertyId,
            "Executables",
            typeof(Dictionary<string, List<string>>),
            TestPropertyAttributes.Hidden,
            typeof(TestCase));

        // Doctest strings
        public const string TestResultErrorKeyword = "ERROR: ";
        public static readonly List<string> SkipTestKeywords = new List<string>() { "doctest::skip()", "doctest::skip(true)" };
    }
}
