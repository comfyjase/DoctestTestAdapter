// TestCommon.cs
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
using DoctestTestAdapter.Shared.Helpers;
using DoctestTestAdapter.Shared.Keywords;
using FakeItEasy;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace DoctestTestAdapter.Tests
{
    internal static class TestCommon
    {
        internal static string SolutionDirectory = Utilities.GetSolutionDirectory();
        internal static string ExamplesSolutionDirectory = SolutionDirectory + "\\DoctestTestAdapter.Examples\\";
        internal static string GodotExamplesSolutionDirectory = SolutionDirectory + "\\DoctestTestAdapter.Examples.Godot\\";
        
        internal static string GodotExecutableFilePath = GodotExamplesSolutionDirectory + "bin\\godot.windows.editor.dev.x86_64.exe";

#if DEBUG
        internal static string UsingDoctestMainExecutableFilePath = ExamplesSolutionDirectory + "bin\\x64\\Debug\\UsingDoctestMain\\UsingDoctestMain.exe";
        internal static string ExecutableUsingDLLExecutableFilePath = ExamplesSolutionDirectory + "bin\\x64\\Debug\\ExecutableUsingDLL\\ExecutableUsingDLL.exe";
        internal static string DLLExecutableFilePath = ExamplesSolutionDirectory + "bin\\x64\\Debug\\ExecutableUsingDLL\\DLL.dll";

        internal static string UsingDoctestMainPdbFilePath = ExamplesSolutionDirectory + "bin\\x64\\Debug\\UsingDoctestMain\\UsingDoctestMain.pdb";
        internal static string ExecutableUsingDLLPdbFilePath = ExamplesSolutionDirectory + "bin\\x64\\Debug\\ExecutableUsingDLL\\ExecutableUsingDLL.pdb";
        internal static string DLLPdbFilePath = ExamplesSolutionDirectory + "bin\\x64\\Debug\\DLL\\DLL.pdb";
#else
        internal static string UsingDoctestMainExecutableFilePath = ExamplesSolutionDirectory + "bin\\x64\\Release\\UsingDoctestMain\\UsingDoctestMain.exe";
        internal static string ExecutableUsingDLLExecutableFilePath = ExamplesSolutionDirectory + "bin\\x64\\Release\\ExecutableUsingDLL\\ExecutableUsingDLL.exe";
        internal static string DLLExecutableFilePath = ExamplesSolutionDirectory + "bin\\x64\\Release\\ExecutableUsingDLL\\DLL.dll";

        internal static string UsingDoctestMainPdbFilePath = ExamplesSolutionDirectory + "bin\\x64\\Release\\UsingDoctestMain\\UsingDoctestMain.pdb";
        internal static string ExecutableUsingDLLPdbFilePath = ExamplesSolutionDirectory + "bin\\x64\\Release\\ExecutableUsingDLL\\ExecutableUsingDLL.pdb";
        internal static string DLLPdbFilePath = ExamplesSolutionDirectory + "bin\\x64\\Release\\DLL\\DLL.pdb";
#endif

        internal static string UsingDoctestMainTestHeaderFilePath = ExamplesSolutionDirectory + "UsingDoctestMain\\TestIsEvenUsingDoctestMain.h";
        internal static string UsingCustomMainTestHeaderFilePath = ExamplesSolutionDirectory + "UsingCustomMain\\TestIsEvenUsingCustomMain.h";
        internal static string ExecutableUsingDLLTestHeaderFilePath = ExamplesSolutionDirectory + "DLLExample\\ExecutableUsingDLL\\TestIsEvenExecutableUsingDLL.h";
        internal static string DLLTestHeaderFilePath = ExamplesSolutionDirectory + "DLLExample\\DLL\\TestIsEvenDLL.h";

        private static string RunSettingsStart =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n"
            + "<RunSettings>\n";
        private static string DoctestRunSettingsStart = "\t<Doctest>\n";
        private static string GeneralRunSettingsStart = "\t\t<GeneralSettings>\n";
        private static string GeneralRunSettingsEnd = "\t\t</GeneralSettings>\n";
        private static string DiscoveryRunSettingsStart = "\t\t<DiscoverySettings>\n";
        private static string DiscoveryRunSettingsSearchDirectoriesStart = "\t\t\t<SearchDirectories>\n";
        private static string DiscoveryRunSettingsSearchDirectoriesEnd = "\t\t\t</SearchDirectories>\n";
        private static string DiscoveryRunSettingsEnd = "\t\t</DiscoverySettings>\n";
        private static string ExecutorRunSettingsStart = "\t\t<ExecutorSettings>\n";
        private static string ExecutorRunSettingsExecutableOverrideStart = "\t\t\t<ExecutableOverrides>\n" + "\t\t\t\t<ExecutableOverride>\n";
        private static string ExecutorRunSettingsExecutableOverrideEnd = "\t\t\t\t</ExecutableOverride>\n" + "\t\t\t</ExecutableOverrides>\n";
        private static string ExecutorRunSettingsEnd = "\t\t</ExecutorSettings>\n";
        private static string DoctestRunSettingsEnd = "\t</Doctest>\n";
        private static string RunSettingsEnd = "</RunSettings>\n";

        // General settings
        internal static string GeneralRunSettingsExample =
            RunSettingsStart +
                DoctestRunSettingsStart +
                    GeneralRunSettingsStart +
                        "\t\t\t<CommandArguments>--test</CommandArguments>\n" +
                        "\t\t\t<PrintStandardOutput>true</PrintStandardOutput>\n" +
                    GeneralRunSettingsEnd +
                DoctestRunSettingsEnd +
            RunSettingsEnd;

        internal static string GeneralRunSettingsPrintStandardOutputExample =
            RunSettingsStart +
                DoctestRunSettingsStart +
                    GeneralRunSettingsStart +
                        "\t\t\t<PrintStandardOutput>true</PrintStandardOutput>\n" +
                    GeneralRunSettingsEnd +
                DoctestRunSettingsEnd +
            RunSettingsEnd;

        // Discovery settings
        internal static string DiscoveryRunSettingsRelativeSearchDirectoryExample =
            RunSettingsStart +
                DoctestRunSettingsStart +
                    DiscoveryRunSettingsStart +
                        DiscoveryRunSettingsSearchDirectoriesStart +
                            "\t\t\t\t<string>UsingDoctestMain</string>\n" +
                        DiscoveryRunSettingsSearchDirectoriesEnd +
                    DiscoveryRunSettingsEnd +
                DoctestRunSettingsEnd +
            RunSettingsEnd;

        internal static string DiscoveryRunSettingsAbsoluteSearchDirectoryExample =
            RunSettingsStart +
                DoctestRunSettingsStart +
                    DiscoveryRunSettingsStart +
                        DiscoveryRunSettingsSearchDirectoriesStart +
                            "\t\t\t\t<string>" + ExamplesSolutionDirectory + "UsingDoctestMain</string>\n" +
                        DiscoveryRunSettingsSearchDirectoriesEnd +
                    DiscoveryRunSettingsEnd +
                DoctestRunSettingsEnd +
            RunSettingsEnd;

        internal static string DiscoveryRunSettingsInvalidSearchDirectoryExample =
            RunSettingsStart +
                DoctestRunSettingsStart +
                    DiscoveryRunSettingsStart +
                        DiscoveryRunSettingsSearchDirectoriesStart +
                            "\t\t\t\t<string>NonExistentDirectory</string>\n" +
                        DiscoveryRunSettingsSearchDirectoriesEnd +
                    DiscoveryRunSettingsEnd +
                DoctestRunSettingsEnd +
            RunSettingsEnd;

        // Executor settings
        internal static string ExecutorRunSettingsRelativeExecutableOverrideExample =
            RunSettingsStart +
                DoctestRunSettingsStart +
                    ExecutorRunSettingsStart +
                        ExecutorRunSettingsExecutableOverrideStart +
#if DEBUG
                            "\t\t\t\t\t<Key>bin\\x64\\Debug\\UsingDoctestMain\\UsingDoctestMain.exe</Key>\n" +
                            "\t\t\t\t\t<Value>bin\\x64\\Debug\\UsingCustomMain\\UsingCustomMain.exe</Value>\n" +
#else
                            "\t\t\t\t\t<Key>bin\\x64\\Release\\UsingDoctestMain\\UsingDoctestMain.exe</Key>\n" +
                            "\t\t\t\t\t<Value>bin\\x64\\Release\\UsingCustomMain\\UsingCustomMain.exe</Value>\n" +
#endif
                        ExecutorRunSettingsExecutableOverrideEnd +
                    ExecutorRunSettingsEnd +
                DoctestRunSettingsEnd +
            RunSettingsEnd;

        internal static string ExecutorRunSettingsAbsoluteExecutableOverrideExample =
            RunSettingsStart +
                DoctestRunSettingsStart +
                    ExecutorRunSettingsStart +
                        ExecutorRunSettingsExecutableOverrideStart +
#if DEBUG
                            "\t\t\t\t\t<Key>" + ExamplesSolutionDirectory + "bin\\x64\\Debug\\UsingDoctestMain\\UsingDoctestMain.exe</Key>\n" +
                            "\t\t\t\t\t<Value>" + ExamplesSolutionDirectory + "bin\\x64\\Debug\\UsingCustomMain\\UsingCustomMain.exe</Value>\n" +
#else
                            "\t\t\t\t\t<Key>" + ExamplesSolutionDirectory + "bin\\x64\\Release\\UsingDoctestMain\\UsingDoctestMain.exe</Key>\n" +
                            "\t\t\t\t\t<Value>" + ExamplesSolutionDirectory + "bin\\x64\\Release\\UsingCustomMain\\UsingCustomMain.exe</Value>\n" +
#endif
                        ExecutorRunSettingsExecutableOverrideEnd +
                    ExecutorRunSettingsEnd +
                DoctestRunSettingsEnd +
            RunSettingsEnd;

        internal static string ExecutorRunSettingsInvalidExecutableOverrideExample =
            RunSettingsStart +
                DoctestRunSettingsStart +
                    ExecutorRunSettingsStart +
                        ExecutorRunSettingsExecutableOverrideStart +
                            "\t\t\t\t\t<Key>NonExistentDirectoryA\\To\\NonExistentExecutableA.exe</Key>\n" +
                            "\t\t\t\t\t<Value>NonExistentDirectoryB\\To\\NonExistentExecutableB.exe</Value>\n" +
                        ExecutorRunSettingsExecutableOverrideEnd +
                    ExecutorRunSettingsEnd +
                DoctestRunSettingsEnd +
            RunSettingsEnd;

        internal static string GodotRunSettingsExample =
            RunSettingsStart +
                DoctestRunSettingsStart +
                    GeneralRunSettingsStart +
                        "\t\t\t<CommandArguments>--headless --test</CommandArguments>\n" +
                    GeneralRunSettingsEnd +
                    DiscoveryRunSettingsStart +
                        DiscoveryRunSettingsSearchDirectoriesStart +
                            "\t\t\t\t<string>modules</string>\n" +
                            "\t\t\t\t<string>tests</string>\n" +
                        DiscoveryRunSettingsSearchDirectoriesEnd +
                    DiscoveryRunSettingsEnd +
                DoctestRunSettingsEnd +
            RunSettingsEnd;

        // Setup for convenience.
        private class ExampleTestCaseData
        {
            internal string TestNamespace { get; private set; }

            internal string NestedNamespace { get; private set; }

            internal string TestClassName { get; private set; }

            internal string TestCaseName { get; private set; }

            internal int LineNumber { get; private set; }

            internal TestOutcome Outcome { get; private set; }

            public ExampleTestCaseData(string testNamespace, string nestedNamespace, string testClassName, string testCaseName, int lineNumber, TestOutcome outcome)
            {
                TestNamespace = testNamespace;
                NestedNamespace = nestedNamespace;
                TestClassName = testClassName;
                TestCaseName = testCaseName;
                LineNumber = lineNumber;
                Outcome = outcome;
            }
        }

        private static List<ExampleTestCaseData> exampleTestCaseData = new List<ExampleTestCaseData>()
        {
            new ExampleTestCaseData("Empty Namespace", "", "Empty Class", "Testing IsEven Always Pass In No Namespace Or Test Suite", 48, TestOutcome.Passed),
            new ExampleTestCaseData("Empty Namespace", "", "Empty Class", "Testing IsEven Always Fail In No Namespace Or Test Suite", 53, TestOutcome.Failed),
            new ExampleTestCaseData("Empty Namespace", "", "Empty Class", "Testing IsEven With Doctest Escape Characters doctest,doctest doctest\\doctest In No Namespace Or Test Suite", 63, TestOutcome.Passed),
            new ExampleTestCaseData("Empty Namespace", "", "Empty Class", "Testing IsEven With Test Adapter Escape Characters doctest:doctest doctest::doctest doctest.doctest In No Namespace Or Test Suite", 68, TestOutcome.Passed),
            new ExampleTestCaseData("Empty Namespace", "", "Empty Class", "Testing IsEven Is Compiled In #if CUSTOM_TEST_MACRO_TRUE In No Namespace Or Test Suite", 107, TestOutcome.Passed),
            new ExampleTestCaseData("Empty Namespace", "", "UniqueTestsFixture", "Testing IsEven Test Case Fixture In No Namespace Or Test Suite", 113, TestOutcome.Passed),
            new ExampleTestCaseData("Empty Namespace", "", "Empty Class", "Testing IsEven Test Case Template In No Namespace Or Test Suite<int>", 118, TestOutcome.Passed),
            new ExampleTestCaseData("Empty Namespace", "", "Empty Class", "Testing IsEven From Custom Test Case Macro In No Namespace Or Test Suite", 123, TestOutcome.Passed),
            new ExampleTestCaseData("Empty Namespace", "", "UniqueTestsFixture", "Testing IsEven From Custom Test Fixture Macro In No Namespace Or Test Suite", 125, TestOutcome.Passed),
            new ExampleTestCaseData("Empty Namespace", "", "Empty Class", "Testing IsEven From Custom Test Case Template Macro In No Namespace Or Test Suite<int>", 127, TestOutcome.Passed),

            // Namespaces will be added onto later with a given prefix.
            new ExampleTestCaseData("TestSuite]", "", "Empty Class", "Testing IsEven Always Pass In Test Suite", 139, TestOutcome.Passed),
            new ExampleTestCaseData("TestSuite]", "", "Empty Class", "Testing IsEven Always Fail In Test Suite", 144, TestOutcome.Failed),
            new ExampleTestCaseData("TestSuite]", "", "Empty Class", "Testing IsEven With Doctest Escape Characters doctest,doctest doctest\\doctest In Test Suite", 154, TestOutcome.Passed),
            new ExampleTestCaseData("TestSuite]", "", "Empty Class", "Testing IsEven With Test Adapter Escape Characters doctest:doctest doctest::doctest doctest.doctest In Test Suite", 159, TestOutcome.Passed),
            new ExampleTestCaseData("TestSuite]", "", "Empty Class", "Testing IsEven Is Compiled In #if CUSTOM_TEST_MACRO_TRUE In Test Suite", 198, TestOutcome.Passed),
            new ExampleTestCaseData("TestSuite]", "", "UniqueTestsFixture", "Testing IsEven Test Case Fixture In Test Suite", 204, TestOutcome.Passed),
            new ExampleTestCaseData("TestSuite]", "", "Empty Class", "Testing IsEven Test Case Template In Test Suite<int>", 209, TestOutcome.Passed),
            new ExampleTestCaseData("TestSuite]", "", "Empty Class", "Testing IsEven From Custom Test Case Macro In Test Suite", 214, TestOutcome.Passed),
            new ExampleTestCaseData("TestSuite]", "", "UniqueTestsFixture", "Testing IsEven From Custom Test Fixture Macro In Test Suite", 216, TestOutcome.Passed),
            new ExampleTestCaseData("TestSuite]", "", "Empty Class", "Testing IsEven From Custom Test Case Template Macro In Test Suite<int>", 218, TestOutcome.Passed),

            new ExampleTestCaseData("Namespace", "", "Empty Class", "Testing IsEven Always Pass In Namespace", 323, TestOutcome.Passed),
            new ExampleTestCaseData("Namespace", "", "Empty Class", "Testing IsEven Always Fail In Namespace", 328, TestOutcome.Failed),
            new ExampleTestCaseData("Namespace", "", "Empty Class", "Testing IsEven With Doctest Escape Characters doctest,doctest doctest\\doctest In Namespace", 338, TestOutcome.Passed),
            new ExampleTestCaseData("Namespace", "", "Empty Class", "Testing IsEven With Test Adapter Escape Characters doctest:doctest doctest::doctest doctest.doctest In Namespace", 343, TestOutcome.Passed),
            new ExampleTestCaseData("Namespace", "", "Empty Class", "Testing IsEven Is Compiled In #if CUSTOM_TEST_MACRO_TRUE In Namespace", 382, TestOutcome.Passed),
            new ExampleTestCaseData("Namespace", "", "UniqueTestsFixture", "Testing IsEven Test Case Fixture In Namespace", 388, TestOutcome.Passed),
            new ExampleTestCaseData("Namespace", "", "Empty Class", "Testing IsEven Test Case Template In Namespace<int>", 393, TestOutcome.Passed),
            new ExampleTestCaseData("Namespace", "", "Empty Class", "Testing IsEven From Custom Test Case Macro In Namespace", 398, TestOutcome.Passed),
            new ExampleTestCaseData("Namespace", "", "UniqueTestsFixture", "Testing IsEven From Custom Test Fixture Macro In Namespace", 400, TestOutcome.Passed),
            new ExampleTestCaseData("Namespace", "", "Empty Class", "Testing IsEven From Custom Test Case Template Macro In Namespace<int>", 402, TestOutcome.Passed),

            new ExampleTestCaseData("NestedNamespaceOne", "NestedNamespaceTwo", "Empty Class", "Testing IsEven Always Pass In Nested Namespace", 417, TestOutcome.Passed),
            new ExampleTestCaseData("NestedNamespaceOne", "NestedNamespaceTwo", "Empty Class", "Testing IsEven Always Fail In Nested Namespace", 422, TestOutcome.Failed),
            new ExampleTestCaseData("NestedNamespaceOne", "NestedNamespaceTwo", "Empty Class", "Testing IsEven With Doctest Escape Characters doctest,doctest doctest\\doctest In Nested Namespace", 432, TestOutcome.Passed),
            new ExampleTestCaseData("NestedNamespaceOne", "NestedNamespaceTwo", "Empty Class", "Testing IsEven With Test Adapter Escape Characters doctest:doctest doctest::doctest doctest.doctest In Nested Namespace", 437, TestOutcome.Passed),
            new ExampleTestCaseData("NestedNamespaceOne", "NestedNamespaceTwo", "Empty Class", "Testing IsEven Is Compiled In #if CUSTOM_TEST_MACRO_TRUE In Nested Namespace", 476, TestOutcome.Passed),
            new ExampleTestCaseData("NestedNamespaceOne", "NestedNamespaceTwo", "UniqueTestsFixture", "Testing IsEven Test Case Fixture In Nested Namespace", 482, TestOutcome.Passed),
            new ExampleTestCaseData("NestedNamespaceOne", "NestedNamespaceTwo", "Empty Class", "Testing IsEven Test Case Template In Nested Namespace<int>", 487, TestOutcome.Passed),
            new ExampleTestCaseData("NestedNamespaceOne", "NestedNamespaceTwo", "Empty Class", "Testing IsEven From Custom Test Case Macro In Nested Namespace", 492, TestOutcome.Passed),
            new ExampleTestCaseData("NestedNamespaceOne", "NestedNamespaceTwo", "UniqueTestsFixture", "Testing IsEven From Custom Test Fixture Macro In Nested Namespace", 494, TestOutcome.Passed),
            new ExampleTestCaseData("NestedNamespaceOne", "NestedNamespaceTwo", "Empty Class", "Testing IsEven From Custom Test Case Template Macro In Nested Namespace<int>", 496, TestOutcome.Passed),

            new ExampleTestCaseData("NamespaceAndTestSuite_Namespace", "NamespaceAndTestSuite_TestSuite]", "Empty Class", "Testing IsEven Always Pass In Namespace And Test Suite", 512, TestOutcome.Passed),
            new ExampleTestCaseData("NamespaceAndTestSuite_Namespace", "NamespaceAndTestSuite_TestSuite]", "Empty Class", "Testing IsEven Always Fail In Namespace And Test Suite", 517, TestOutcome.Failed),
            new ExampleTestCaseData("NamespaceAndTestSuite_Namespace", "NamespaceAndTestSuite_TestSuite]", "Empty Class", "Testing IsEven With Doctest Escape Characters doctest,doctest doctest\\doctest In Namespace And Test Suite", 527, TestOutcome.Passed),
            new ExampleTestCaseData("NamespaceAndTestSuite_Namespace", "NamespaceAndTestSuite_TestSuite]", "Empty Class", "Testing IsEven With Test Adapter Escape Characters doctest:doctest doctest::doctest doctest.doctest In Namespace And Test Suite", 532, TestOutcome.Passed),
            new ExampleTestCaseData("NamespaceAndTestSuite_Namespace", "NamespaceAndTestSuite_TestSuite]", "Empty Class", "Testing IsEven Is Compiled In #if CUSTOM_TEST_MACRO_TRUE In Namespace And Test Suite", 571, TestOutcome.Passed),
            new ExampleTestCaseData("NamespaceAndTestSuite_Namespace", "NamespaceAndTestSuite_TestSuite]", "UniqueTestsFixture", "Testing IsEven Test Case Fixture In Namespace And Test Suite", 577, TestOutcome.Passed),
            new ExampleTestCaseData("NamespaceAndTestSuite_Namespace", "NamespaceAndTestSuite_TestSuite]", "Empty Class", "Testing IsEven Test Case Template In Namespace And Test Suite<int>", 582, TestOutcome.Passed),
            new ExampleTestCaseData("NamespaceAndTestSuite_Namespace", "NamespaceAndTestSuite_TestSuite]", "Empty Class", "Testing IsEven From Custom Test Case Macro In Namespace And Test Suite", 587, TestOutcome.Passed),
            new ExampleTestCaseData("NamespaceAndTestSuite_Namespace", "NamespaceAndTestSuite_TestSuite]", "UniqueTestsFixture", "Testing IsEven From Custom Test Fixture Macro In Namespace And Test Suite", 589, TestOutcome.Passed),
            new ExampleTestCaseData("NamespaceAndTestSuite_Namespace", "NamespaceAndTestSuite_TestSuite]", "Empty Class", "Testing IsEven From Custom Test Case Template Macro In Namespace And Test Suite<int>", 591, TestOutcome.Passed),
        };

        internal static DoctestTestSettings LoadDoctestSettings(ISettingsProvider settingsProvider, string settingsAsString)
        {
            using (XmlReader xmlReader = XmlReader.Create(new StringReader(settingsAsString)))
            {
                xmlReader.MoveToContent();

                using (XmlReader subXmlReader = XmlReader.Create(new StringReader(settingsAsString)))
                {
                    subXmlReader.MoveToContent();

                    while (!xmlReader.EOF)
                    {
                        subXmlReader.Read();

                        if (subXmlReader.Name == "Doctest")
                        {
                            // About to find doctest node so break so the settings can read the values correctly.
                            break;
                        }

                        xmlReader.Read();
                    }

                    settingsProvider.Load(xmlReader);
                }
            }

            IRunContext runContext = A.Fake<IRunContext>();
            A.CallTo(() => runContext.RunSettings.GetSettings(DoctestTestSettings.RunSettingsXmlNode))
                .Returns(settingsProvider);

            return DoctestTestSettingsProvider.LoadSettings(runContext);
        }

        internal static void AssertStandardOutputSettingOutput(string output, string expectedHeaderFilePath)
        {
            Assert.IsFalse(string.IsNullOrEmpty(output));
            Assert.IsTrue(output.Contains("Image has the following dependencies:"));
            Assert.IsTrue(output.Contains("[doctest] listing all test suites"));
            Assert.IsTrue(output.Contains("[UsingDoctestMainTestSuite]"));
            Assert.IsTrue(output.Contains("[UsingDoctestMainNamespaceAndTestSuite_TestSuite]"));
            Assert.IsTrue(output.Contains("[doctest] listing all test case names"));
            Assert.IsTrue(output.Contains("[UsingDoctestMain] Testing IsEven Always Pass In No Namespace Or Test Suite"));
            Assert.IsTrue(output.Contains("PDB file found at "));
            Assert.IsTrue(output.Contains("*** STRINGTABLE"));
            Assert.IsTrue(output.Contains(expectedHeaderFilePath));
        }

        internal static void AssertTestCase(TestCase testCase, string expectedSource, string expectedFullyQualifiedName, string expectedDisplayName, string expectedCodeFilePath, int expectedLineNumber)
        {
            Assert.IsNotNull(testCase);
            Assert.AreEqual(expectedSource, testCase.Source);
            Assert.AreEqual(expectedFullyQualifiedName, testCase.FullyQualifiedName);
            Assert.AreEqual(expectedDisplayName, testCase.DisplayName);
            Assert.AreEqual(expectedCodeFilePath, testCase.CodeFilePath);
            Assert.AreEqual(expectedLineNumber, testCase.LineNumber);
        }

        /// <summary>
        /// Asserts against the exampleTestCaseData list.
        /// </summary>
        /// <param name="testCases"></param>
        /// <param name="expectedTestSource"></param>
        /// <param name="expectedNamespace"></param>
        /// <param name="expectedClassName"></param>
        /// <param name="expectedPrefix"></param>
        /// <param name="expectedCodeFilePath"></param>
        internal static void AssertTestCases(List<TestCase> testCases, string expectedTestSource, string expectedPrefix, string expectedCodeFilePath)
        {
            Assert.IsNotEmpty(testCases);
            Assert.AreEqual(exampleTestCaseData.Count, testCases.Count);

            for (int i = 0; i < testCases.Count; ++i)
            {
                ExampleTestCaseData currentExampleTestCaseData = exampleTestCaseData[i];

                string testNamespace = string.Empty;
                if (currentExampleTestCaseData.TestNamespace == "Empty Namespace")
                    testNamespace = currentExampleTestCaseData.TestNamespace;
                else if (currentExampleTestCaseData.TestNamespace.Contains("]"))
                    testNamespace = "[" + expectedPrefix + currentExampleTestCaseData.TestNamespace;
                else
                    testNamespace = expectedPrefix + currentExampleTestCaseData.TestNamespace;

                if (!string.IsNullOrEmpty(currentExampleTestCaseData.NestedNamespace))
                {
                    if (currentExampleTestCaseData.NestedNamespace.Contains("]"))
                        testNamespace += (@"::[" + expectedPrefix + currentExampleTestCaseData.NestedNamespace);
                    else
                        testNamespace += (@"::" + expectedPrefix + currentExampleTestCaseData.NestedNamespace);
                }

                AssertTestCase(testCases[i],
                    expectedTestSource,
                    testNamespace + "::" + currentExampleTestCaseData.TestClassName + "::[" + expectedPrefix + "] " + currentExampleTestCaseData.TestCaseName.Replace(@"::", @"\:\:").Replace(@".", "\u2024"),
                    "[" + expectedPrefix + "] " + currentExampleTestCaseData.TestCaseName,
                    expectedCodeFilePath,
                    currentExampleTestCaseData.LineNumber
                );
            }
        }

        /// <summary>
        /// Checks testCaseNames against exampleTestCaseData.TestCaseName
        /// </summary>
        /// <param name="testCaseNames"></param>
        /// <param name="prefixTag">E.g. "[UsingDoctestMain]"</param>
        internal static void AssertTestCaseNames(List<string> testCaseNames, string prefixTag)
        {
            Assert.IsTrue(testCaseNames.Count == exampleTestCaseData.Count);

            for (int i = 0; i < exampleTestCaseData.Count; ++i)
            {
                Assert.IsFalse(string.IsNullOrEmpty(testCaseNames[i]));
                Assert.AreEqual(prefixTag + " " + exampleTestCaseData[i].TestCaseName, testCaseNames[i]);
            }
        }

        internal static void AssertKeywords(string executableFilePath, string headerFilePath, List<IKeyword> keywords, Func<int, string, List<TestCase>, bool> testFunction)
        {
            Assert.IsNotEmpty(keywords);

            string[] allLines = File.ReadAllLines(headerFilePath);
            Assert.IsNotEmpty(allLines);

            List<TestCase> testCases = new List<TestCase>();
            string testNamespace = string.Empty;
            string testClassName = string.Empty;
            int currentLineNumber = 0;

            foreach (string line in allLines)
            {
                ++currentLineNumber;
                keywords.ForEach(k => k.Check(executableFilePath,
                    headerFilePath,
                    ref testNamespace,
                    ref testClassName,
                    line,
                    currentLineNumber,
                    ref testCases));

                if (testFunction(currentLineNumber, testNamespace, testCases))
                {
                    return;
                }
            }

            // Should not get here if the test is successful.
            Assert.IsTrue(false);
        }

        internal static void AssertTestResults(List<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult> testResults)
        {
            Assert.IsNotEmpty(testResults);
            Assert.IsTrue(testResults.Count == exampleTestCaseData.Count);

            for (int i = 0; i < exampleTestCaseData.Count; ++i)
            {
                Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult testResult = testResults[i];
                Assert.AreEqual(exampleTestCaseData[i].Outcome, testResult.Outcome);

                if (testResult.Outcome == TestOutcome.Failed)
                {
                    Assert.IsTrue(!string.IsNullOrEmpty(testResult.ErrorMessage));
                    Assert.IsTrue(testResult.ErrorMessage.Contains("CHECK( IsEven(3) ) is NOT correct!"));
                }
            }
        }
    }
}
