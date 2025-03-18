using DoctestTestAdapter.Shared.Helpers;
using DoctestTestAdapter.Shared.Keywords;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

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
