using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;

namespace DoctestTestAdapter.Shared.Helpers
{
    internal static class Constants
    {
        internal static readonly List<string> SupportedVisualStudioVersionNames = new List<string>()
        {
            "Visual Studio Community 2022",
            "Visual Studio Professional 2022",
            "Visual Studio Enterprise 2022",
        };

        // Test adapter
        internal const string ExecutorUriString = "executor://DoctestTestExecutor";
        internal static readonly Uri ExecutorUri = new Uri(ExecutorUriString);
        internal static string EmptyNamespaceString = "Empty Namespace";
        internal static string EmptyClassString = "Empty Class";

        // Command arguments
        // https://learn.microsoft.com/en-us/troubleshoot/windows-client/shell-experience/command-line-string-limitation
        // According to this documentation the limit is 8191 for command prompt.
        internal static readonly int MaxCommandPromptArgumentLength = 8191;

        // Profiling
        internal static readonly bool ProfilingEnabled = false;

        // doctest key strings
        internal const string TestResultErrorKeyword = "ERROR: ";
        public static readonly List<string> SkipTestKeywords = new List<string>() { "doctest::skip()", "doctest::skip(true)" };

        // Custom Test Properties
        // If the test case should be skipped.
        private const string ShouldBeSkippedTestPropertyId = "TestCase.ShouldBeSkipped";
        internal static readonly TestProperty ShouldBeSkippedTestProperty = TestProperty.Register(
            ShouldBeSkippedTestPropertyId,
            "ShouldBeSkipped",
            typeof(bool),
            TestPropertyAttributes.Hidden,
            typeof(TestCase));
    }
}
