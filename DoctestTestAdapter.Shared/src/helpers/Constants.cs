// Constants.cs
//
// Copyright (c) 2025-present Jase Mottershead
//
// MIT License
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
        internal static string ErrorMessagePrefix = "[Test Adapter for Doctest] [Error]";
        internal static string WarningMessagePrefix = "[Test Adapter for Doctest] [Warning]";
        internal static string InformationMessagePrefix = "[Test Adapter for Doctest] [Information]";

        // Command arguments
        // https://learn.microsoft.com/en-us/troubleshoot/windows-client/shell-experience/command-line-string-limitation
        // According to this documentation the limit is 8191 for command prompt.
        internal static readonly int MaxCommandPromptArgumentLength = 8191;

        // Profiling
        internal static readonly bool ProfilingEnabled = false;
    }
}
