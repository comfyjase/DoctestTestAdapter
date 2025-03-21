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
#if DEBUG
        internal static string UsingDoctestMainExecutableFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Debug\\UsingDoctestMain\\UsingDoctestMain.exe";
        internal static string ExecutableUsingDLLExecutableFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Debug\\ExecutableUsingDLL\\ExecutableUsingDLL.exe";
        internal static string DLLExecutableFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Debug\\ExecutableUsingDLL\\DLL.dll";

        internal static string UsingDoctestMainPdbFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Debug\\UsingDoctestMain\\UsingDoctestMain.pdb";
        internal static string ExecutableUsingDLLPdbFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Debug\\ExecutableUsingDLL\\ExecutableUsingDLL.pdb";
        internal static string DLLPdbFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Debug\\DLL\\DLL.pdb";
#else
        internal static string UsingDoctestMainExecutableFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Release\\UsingDoctestMain\\UsingDoctestMain.exe";
        internal static string ExecutableUsingDLLExecutableFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Release\\ExecutableUsingDLL\\ExecutableUsingDLL.exe";
        internal static string DLLExecutableFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Release\\ExecutableUsingDLL\\DLL.dll";

        internal static string UsingDoctestMainPdbFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Release\\UsingDoctestMain\\UsingDoctestMain.pdb";
        internal static string ExecutableUsingDLLPdbFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Release\\ExecutableUsingDLL\\ExecutableUsingDLL.pdb";
        internal static string DLLPdbFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Release\\DLL\\DLL.pdb";
#endif

        internal static string UsingDoctestMainTestHeaderFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\UsingDoctestMain\\TestIsEvenUsingDoctestMain.h";
        internal static string UsingCustomMainTestHeaderFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\UsingCustomMain\\TestIsEvenUsingCustomMain.h";
        internal static string ExecutableUsingDLLTestHeaderFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\DLLExample\\ExecutableUsingDLL\\TestIsEvenExecutableUsingDLL.h";
        internal static string DLLTestHeaderFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\DLLExample\\DLL\\TestIsEvenDLL.h";

        private static string RunSettingsStart =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n"
            + "<RunSettings>\n";
        private static string DoctestRunSettingsStart = "\t<Doctest>\n";
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
                    "\t\t<GeneralSettings>\n" +
                        "\t\t\t<CommandArguments>--test</CommandArguments>\n" +
                        "\t\t\t<PrintStandardOutput>true</PrintStandardOutput>\n" +
                    "\t\t</GeneralSettings>\n" +
                DoctestRunSettingsEnd +
            RunSettingsEnd;

        internal static string GeneralRunSettingsPrintStandardOutputExample =
            RunSettingsStart +
                DoctestRunSettingsStart +
                    "\t\t<GeneralSettings>\n" +
                        "\t\t\t<PrintStandardOutput>true</PrintStandardOutput>\n" +
                    "\t\t</GeneralSettings>\n" +
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
                            "\t\t\t\t<string>" + Utilities.GetSolutionDirectory(Directory.GetParent(TestCommon.UsingDoctestMainExecutableFilePath).FullName) + "\\UsingDoctestMain</string>\n" +
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
                            "\t\t\t\t\t<Key>" + Utilities.GetSolutionDirectory(Directory.GetParent(TestCommon.UsingDoctestMainExecutableFilePath).FullName) + "\\bin\\x64\\Debug\\UsingDoctestMain\\UsingDoctestMain.exe</Key>\n" +
                            "\t\t\t\t\t<Value>" + Utilities.GetSolutionDirectory(Directory.GetParent(TestCommon.UsingDoctestMainExecutableFilePath).FullName) + "\\bin\\x64\\Debug\\UsingCustomMain\\UsingCustomMain.exe</Value>\n" +
#else
                            "\t\t\t\t\t<Key>" + Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Release\\UsingDoctestMain\\UsingDoctestMain.exe</Key>\n" +
                            "\t\t\t\t\t<Value>" + Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Release\\UsingCustomMain\\UsingCustomMain.exe</Value>\n" +
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
            new ExampleTestCaseData("Empty Namespace", "", "Empty Class", "Testing IsEven Always Pass In No Namespace Or Test Suite", 19, TestOutcome.Passed),
            new ExampleTestCaseData("Empty Namespace", "", "Empty Class", "Testing IsEven Always Fail In No Namespace Or Test Suite", 24, TestOutcome.Failed),
            new ExampleTestCaseData("Empty Namespace", "", "Empty Class", "Testing IsEven With Doctest Escape Characters doctest,doctest doctest\\doctest In No Namespace Or Test Suite", 34, TestOutcome.Passed),
            new ExampleTestCaseData("Empty Namespace", "", "Empty Class", "Testing IsEven With Test Adapter Escape Characters doctest:doctest doctest::doctest In No Namespace Or Test Suite", 39, TestOutcome.Passed),
            new ExampleTestCaseData("Empty Namespace", "", "Empty Class", "Testing IsEven Is Compiled In #if CUSTOM_TEST_MACRO_TRUE In No Namespace Or Test Suite", 78, TestOutcome.Passed),

            // Namespaces will be added onto later with a given prefix.
            new ExampleTestCaseData("TestSuite]", "", "Empty Class", "Testing IsEven Always Pass In Test Suite", 94, TestOutcome.Passed),
            new ExampleTestCaseData("TestSuite]", "", "Empty Class", "Testing IsEven Always Fail In Test Suite", 99, TestOutcome.Failed),
            new ExampleTestCaseData("TestSuite]", "", "Empty Class", "Testing IsEven With Doctest Escape Characters doctest,doctest doctest\\doctest In Test Suite", 109, TestOutcome.Passed),
            new ExampleTestCaseData("TestSuite]", "", "Empty Class", "Testing IsEven With Test Adapter Escape Characters doctest:doctest doctest::doctest In Test Suite", 114, TestOutcome.Passed),
            new ExampleTestCaseData("TestSuite]", "", "Empty Class", "Testing IsEven Is Compiled In #if CUSTOM_TEST_MACRO_TRUE In Test Suite", 153, TestOutcome.Passed),

            new ExampleTestCaseData("Namespace", "", "Empty Class", "Testing IsEven Always Pass In Namespace", 246, TestOutcome.Passed),
            new ExampleTestCaseData("Namespace", "", "Empty Class", "Testing IsEven Always Fail In Namespace", 251, TestOutcome.Failed),
            new ExampleTestCaseData("Namespace", "", "Empty Class", "Testing IsEven With Doctest Escape Characters doctest,doctest doctest\\doctest In Namespace", 261, TestOutcome.Passed),
            new ExampleTestCaseData("Namespace", "", "Empty Class", "Testing IsEven With Test Adapter Escape Characters doctest:doctest doctest::doctest In Namespace", 266, TestOutcome.Passed),
            new ExampleTestCaseData("Namespace", "", "Empty Class", "Testing IsEven Is Compiled In #if CUSTOM_TEST_MACRO_TRUE In Namespace", 305, TestOutcome.Passed),

            new ExampleTestCaseData("NestedNamespaceOne", "NestedNamespaceTwo", "Empty Class", "Testing IsEven Always Pass In Nested Namespace", 324, TestOutcome.Passed),
            new ExampleTestCaseData("NestedNamespaceOne", "NestedNamespaceTwo", "Empty Class", "Testing IsEven Always Fail In Nested Namespace", 329, TestOutcome.Failed),
            new ExampleTestCaseData("NestedNamespaceOne", "NestedNamespaceTwo", "Empty Class", "Testing IsEven With Doctest Escape Characters doctest,doctest doctest\\doctest In Nested Namespace", 339, TestOutcome.Passed),
            new ExampleTestCaseData("NestedNamespaceOne", "NestedNamespaceTwo", "Empty Class", "Testing IsEven With Test Adapter Escape Characters doctest:doctest doctest::doctest In Nested Namespace", 344, TestOutcome.Passed),
            new ExampleTestCaseData("NestedNamespaceOne", "NestedNamespaceTwo", "Empty Class", "Testing IsEven Is Compiled In #if CUSTOM_TEST_MACRO_TRUE In Nested Namespace", 383, TestOutcome.Passed),

            new ExampleTestCaseData("NamespaceAndTestSuite_Namespace", "NamespaceAndTestSuite_TestSuite]", "Empty Class", "Testing IsEven Always Pass In Namespace And Test Suite", 403, TestOutcome.Passed),
            new ExampleTestCaseData("NamespaceAndTestSuite_Namespace", "NamespaceAndTestSuite_TestSuite]", "Empty Class", "Testing IsEven Always Fail In Namespace And Test Suite", 408, TestOutcome.Failed),
            new ExampleTestCaseData("NamespaceAndTestSuite_Namespace", "NamespaceAndTestSuite_TestSuite]", "Empty Class", "Testing IsEven With Doctest Escape Characters doctest,doctest doctest\\doctest In Namespace And Test Suite", 418, TestOutcome.Passed),
            new ExampleTestCaseData("NamespaceAndTestSuite_Namespace", "NamespaceAndTestSuite_TestSuite]", "Empty Class", "Testing IsEven With Test Adapter Escape Characters doctest:doctest doctest::doctest In Namespace And Test Suite", 423, TestOutcome.Passed),
            new ExampleTestCaseData("NamespaceAndTestSuite_Namespace", "NamespaceAndTestSuite_TestSuite]", "Empty Class", "Testing IsEven Is Compiled In #if CUSTOM_TEST_MACRO_TRUE In Namespace And Test Suite", 462, TestOutcome.Passed),
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
                    testNamespace + "::" + currentExampleTestCaseData.TestClassName + "::[" + expectedPrefix + "] " + currentExampleTestCaseData.TestCaseName.Replace(@"::", @"\:\:"),
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

        internal static void AssertKeywords(string executableFilePath, string headerFilePath, List<Keyword> keywords, Func<int, string, List<TestCase>, bool> testFunction)
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
