using System;
using System.Collections.Generic;

namespace DoctestTestAdapter.Shared.Helpers
{
    internal static class Constants
    {
        // Visual Studio
        internal static readonly List<string> SupportedVisualStudioNames = new List<string>()
        {
            "Visual Studio Community",
            "Visual Studio Professional",
            "Visual Studio Enterprise",
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
    }
}
