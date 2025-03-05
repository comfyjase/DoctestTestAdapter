using DoctestTestAdapter.Shared.EqualityComparers;
using DoctestTestAdapter.Shared.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace DoctestTestAdapter.Tests.EqualityComparers
{
    [TestClass]
    public class TestCaseComparerTest
    {
        [TestMethod]
        public void CompareSameTestCases()
        {
            List<TestCase> testCases = Utilities.GetTestCases(TestCommon.ExampleExecutableFilePath);
            Assert.IsTrue(testCases.Count == 4);

            List<string> sourceFiles = Utilities.GetSourceFiles(TestCommon.ExampleExecutableFilePath);
            Assert.IsTrue(sourceFiles.Count == 1);

            string sourceFile = sourceFiles[0];
            Assert.IsTrue(File.Exists(sourceFile));
            Assert.IsTrue(sourceFile.EndsWith("TestIsEvenUsingDoctestMain.h"));

            TestCase testCase = testCases[0];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.ExampleExecutableFilePath,
                    "TestUsingDoctestMain::Empty Class::[UsingDoctestMain] Testing IsEven Always Pass",
                    "[UsingDoctestMain] Testing IsEven Always Pass",
                    sourceFile,
                    50);

            TestCase copyOfTestCase = testCase;

            TestCaseComparer comparer = new TestCaseComparer();
            Assert.IsTrue(comparer.Equals(testCase, copyOfTestCase));
        }

        [TestMethod]
        public void CompareDifferentTestCases()
        {
            List<TestCase> testCases = Utilities.GetTestCases(TestCommon.ExampleExecutableFilePath);
            Assert.IsTrue(testCases.Count == 4);

            List<string> sourceFiles = Utilities.GetSourceFiles(TestCommon.ExampleExecutableFilePath);
            Assert.IsTrue(sourceFiles.Count == 1);

            string sourceFile = sourceFiles[0];
            Assert.IsTrue(File.Exists(sourceFile));
            Assert.IsTrue(sourceFile.EndsWith("TestIsEvenUsingDoctestMain.h"));

            TestCase firstTestCase = testCases[0];
            TestCommon.AssertTestCase(firstTestCase,
                    TestCommon.ExampleExecutableFilePath,
                    "TestUsingDoctestMain::Empty Class::[UsingDoctestMain] Testing IsEven Always Pass",
                    "[UsingDoctestMain] Testing IsEven Always Pass",
                    sourceFile,
                    50);

            TestCase secondTestCase = testCases[1];
            TestCommon.AssertTestCase(secondTestCase,
                    TestCommon.ExampleExecutableFilePath,
                    "TestUsingDoctestMain::Empty Class::[UsingDoctestMain] Testing IsEven Always Fail",
                    "[UsingDoctestMain] Testing IsEven Always Fail",
                    sourceFile,
                    57);

            TestCaseComparer comparer = new TestCaseComparer();
            Assert.IsFalse(comparer.Equals(firstTestCase, secondTestCase));
        }
    }
}
