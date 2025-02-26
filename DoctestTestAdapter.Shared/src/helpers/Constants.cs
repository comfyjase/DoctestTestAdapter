using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;

namespace DoctestTestAdapter.Shared.Helpers
{
    internal static class Constants
    {
        // Test adapter
        internal const string ExecutorUriString = "executor://DoctestTestExecutor";
        internal static readonly Uri ExecutorUri = new Uri(ExecutorUriString);

        // Process command arguments
        internal static readonly int MaxCommandArgumentLength = 32699;

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
