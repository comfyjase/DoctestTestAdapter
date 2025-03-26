// GodotTest.cs
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

using DoctestTestAdapter.Settings;
using DoctestTestAdapter.Shared.Executables;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoctestTestAdapter.Tests.Godot
{
    public class GodotTest
    {
        protected DoctestTestSettings settings = null;

        protected void AssertAndLoadExampleRunSettings(ISettingsProvider settingsProvider)
        {
            settings = TestCommon.LoadDoctestSettings(settingsProvider, TestCommon.GodotRunSettingsExample);
            Assert.IsNotNull(settings);

            Assert.IsNotNull(settings.GeneralSettings);
            Assert.IsFalse(string.IsNullOrEmpty(settings.GeneralSettings.CommandArguments));
            Assert.AreEqual("--headless --test", settings.GeneralSettings.CommandArguments);

            Assert.IsNotNull(settings.DiscoverySettings);
            Assert.HasCount(2, settings.DiscoverySettings.SearchDirectories);
            Assert.IsFalse(string.IsNullOrEmpty(settings.DiscoverySettings.SearchDirectories[0]));
            Assert.IsFalse(string.IsNullOrEmpty(settings.DiscoverySettings.SearchDirectories[1]));
            Assert.AreEqual("modules", settings.DiscoverySettings.SearchDirectories[0]);
            Assert.AreEqual("tests", settings.DiscoverySettings.SearchDirectories[1]);
        }

        // Verifying that all test cases have been discovered and print out the names of any that are missing (if any).
        protected void AssertMissingTestCases(IRunContext runContext, IReadOnlyList<TestCase> discoveredTestCases)
        {
            DoctestExecutable godotExe = new DoctestExecutable(TestCommon.GodotExecutableFilePath, TestCommon.GodotExamplesSolutionDirectory, settings, runContext, null, null);
            List<string> godotTestCaseNames = godotExe.GetTestCaseNames();
            List<string> discoveredTestCaseNames = new List<string>();
            foreach (TestCase testCase in discoveredTestCases)
            {
                discoveredTestCaseNames.Add(testCase.DisplayName);
            }
            List<string> missingTestCases = godotTestCaseNames
                .Except(discoveredTestCaseNames)
                .ToList();
            missingTestCases.ForEach(s => Console.Error.WriteLine("Missing test case: " + s));

            Assert.IsEmpty(missingTestCases);
        }
    }
}
