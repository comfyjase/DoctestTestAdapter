using DoctestTestAdapter.Shared.Helpers;
using FakeItEasy;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace DoctestTestAdapter.Tests.Discovery
{
    [TestClass]
    public class DoctestTestDiscovererTest
    {
        [TestMethod]
        public void DiscoverExe()
        {
            IEnumerable<string> sources = new List<string>(){ TestCommon.ExampleExecutableFilePath };

            Captured<TestCase> capturedTestCases = A.Captured<TestCase>();
            IRunContext discoveryContext = A.Fake<IRunContext>();
            IMessageLogger messageLogger = A.Fake<IMessageLogger>();
            ITestCaseDiscoverySink testCaseDiscoverySink = A.Fake<ITestCaseDiscoverySink>();

            A.CallTo(() => testCaseDiscoverySink.SendTestCase(capturedTestCases._))
                .DoesNothing();

            ITestDiscoverer doctestTestDiscoverer = new DoctestTestDiscoverer();
            doctestTestDiscoverer.DiscoverTests(sources, discoveryContext, messageLogger, testCaseDiscoverySink);
            Assert.IsTrue(capturedTestCases.Values.Count == 4);

            List<string> sourceFiles = Utilities.GetSourceFiles(TestCommon.ExampleExecutableFilePath);
            Assert.IsTrue(sourceFiles.Count == 1);

            string sourceFile = sourceFiles[0];
            Assert.IsTrue(File.Exists(sourceFile));
            Assert.IsTrue(sourceFile.EndsWith("TestIsEvenUsingDoctestMain.h"));

            TestCase testCase = capturedTestCases.Values[0];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.ExampleExecutableFilePath,
                    "TestUsingDoctestMain::Empty Class::[UsingDoctestMain] Testing IsEven Always Pass",
                    "[UsingDoctestMain] Testing IsEven Always Pass",
                    sourceFile,
                    50);

            testCase = capturedTestCases.Values[1];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.ExampleExecutableFilePath,
                    "TestUsingDoctestMain::Empty Class::[UsingDoctestMain] Testing IsEven Always Fail",
                    "[UsingDoctestMain] Testing IsEven Always Fail",
                    sourceFile,
                    57);

            testCase = capturedTestCases.Values[2];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.ExampleExecutableFilePath,
                    "TestUsingDoctestMain::Empty Class::[UsingDoctestMain] Testing IsEven Always Skipped",
                    "[UsingDoctestMain] Testing IsEven Always Skipped",
                    sourceFile,
                    64);

            testCase = capturedTestCases.Values[3];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.ExampleExecutableFilePath,
                    "TestUsingDoctestMain::Empty Class::[UsingDoctestMain] Testing IsEven, Always Pass, With Commas In Name",
                    "[UsingDoctestMain] Testing IsEven, Always Pass, With Commas In Name",
                    sourceFile,
                    71);
        }

        [TestMethod]
        public void DiscoverExeAndDLL()
        {
            IEnumerable<string> sources = new List<string>() { TestCommon.ExampleExecutableUsingDLLFilePath };
            Captured<TestCase> capturedTestCases = A.Captured<TestCase>();

            IRunContext discoveryContext = A.Fake<IRunContext>();
            IMessageLogger messageLogger = A.Fake<IMessageLogger>();
            ITestCaseDiscoverySink testCaseDiscoverySink = A.Fake<ITestCaseDiscoverySink>();

            A.CallTo(() => testCaseDiscoverySink.SendTestCase(capturedTestCases._))
                .DoesNothing();

            ITestDiscoverer doctestTestDiscoverer = new DoctestTestDiscoverer();
            doctestTestDiscoverer.DiscoverTests(sources, discoveryContext, messageLogger, testCaseDiscoverySink);
            Assert.IsTrue(capturedTestCases.Values.Count == 8);

            List<string> sourceFiles = Utilities.GetSourceFiles(TestCommon.ExampleExecutableUsingDLLFilePath);
            Assert.IsTrue(sourceFiles.Count == 4);

            string dllTestSourceFile = sourceFiles[2];
            Assert.IsTrue(File.Exists(dllTestSourceFile));
            Assert.IsTrue(dllTestSourceFile.EndsWith("TestIsEvenDLL.h"));

            string executableUsingDLLTestSourceFile = sourceFiles[3];
            Assert.IsTrue(File.Exists(executableUsingDLLTestSourceFile));
            Assert.IsTrue(executableUsingDLLTestSourceFile.EndsWith("TestIsEvenExecutableUsingDLL.h"));

            // DLL Test Cases
            TestCase testCase = capturedTestCases.Values[0];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.ExampleExecutableUsingDLLFilePath,
                    "TestDLL::Empty Class::[DLL] Testing IsEven Always Pass",
                    "[DLL] Testing IsEven Always Pass",
                    dllTestSourceFile,
                    46);

            testCase = capturedTestCases.Values[1];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.ExampleExecutableUsingDLLFilePath,
                    "TestDLL::Empty Class::[DLL] Testing IsEven Always Fail",
                    "[DLL] Testing IsEven Always Fail",
                    dllTestSourceFile,
                    53);

            testCase = capturedTestCases.Values[2];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.ExampleExecutableUsingDLLFilePath,
                    "TestDLL::Empty Class::[DLL] Testing IsEven Always Skipped",
                    "[DLL] Testing IsEven Always Skipped",
                    dllTestSourceFile,
                    60);

            testCase = capturedTestCases.Values[3];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.ExampleExecutableUsingDLLFilePath,
                    "TestDLL::Empty Class::[DLL] Testing IsEven, Always Pass, With Commas In Name",
                    "[DLL] Testing IsEven, Always Pass, With Commas In Name",
                    dllTestSourceFile,
                    67);

            // ExecutableUsingDLL Test Cases
            testCase = capturedTestCases.Values[4];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.ExampleExecutableUsingDLLFilePath,
                    "TestExecutableUsingDLL::Empty Class::[ExecutableUsingDLL] Testing IsEven Always Pass",
                    "[ExecutableUsingDLL] Testing IsEven Always Pass",
                    executableUsingDLLTestSourceFile,
                    46);

            testCase = capturedTestCases.Values[5];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.ExampleExecutableUsingDLLFilePath,
                    "TestExecutableUsingDLL::Empty Class::[ExecutableUsingDLL] Testing IsEven Always Fail",
                    "[ExecutableUsingDLL] Testing IsEven Always Fail",
                    executableUsingDLLTestSourceFile,
                    53);

            testCase = capturedTestCases.Values[6];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.ExampleExecutableUsingDLLFilePath,
                    "TestExecutableUsingDLL::Empty Class::[ExecutableUsingDLL] Testing IsEven Always Skipped",
                    "[ExecutableUsingDLL] Testing IsEven Always Skipped",
                    executableUsingDLLTestSourceFile,
                    60);

            testCase = capturedTestCases.Values[7];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.ExampleExecutableUsingDLLFilePath,
                    "TestExecutableUsingDLL::Empty Class::[ExecutableUsingDLL] Testing IsEven, Always Pass, With Commas In Name",
                    "[ExecutableUsingDLL] Testing IsEven, Always Pass, With Commas In Name",
                    executableUsingDLLTestSourceFile,
                    67);
        }
    }
}
