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
        private string _exampleExecutableFilePath           = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Debug\\UsingDoctestMain\\UsingDoctestMain.exe";
        private string _exampleExecutableUsingDLLFilePath   = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Debug\\ExecutableUsingDLL\\ExecutableUsingDLL.exe";
        private string _exampleDLLFilePath                  = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Debug\\ExecutableUsingDLL\\DLL.dll";

        [TestMethod]
        public void DiscoverExe()
        {
            IEnumerable<string> sources = new List<string>(){ _exampleExecutableFilePath };
            Captured<TestCase> capturedTestCases = A.Captured<TestCase>();

            IRunContext discoveryContext = A.Fake<IRunContext>();
            IMessageLogger messageLogger = A.Fake<IMessageLogger>();
            ITestCaseDiscoverySink testCaseDiscoverySink = A.Fake<ITestCaseDiscoverySink>();

            A.CallTo(() => testCaseDiscoverySink.SendTestCase(capturedTestCases._))
                .DoesNothing();

            ITestDiscoverer doctestTestDiscoverer = new DoctestTestDiscoverer();
            doctestTestDiscoverer.DiscoverTests(sources, discoveryContext, messageLogger, testCaseDiscoverySink);
            Assert.IsTrue(capturedTestCases.Values.Count == 3);

            List<string> sourceFiles = Utilities.GetSourceFiles(_exampleExecutableFilePath);
            Assert.IsTrue(sourceFiles.Count == 1);

            string sourceFile = sourceFiles[0];
            Assert.IsTrue(File.Exists(sourceFile));
            Assert.IsTrue(sourceFile.EndsWith("TestIsEvenUsingDoctestMain.h"));

            TestCase testCase = capturedTestCases.Values[0];
            TestCommon.AssertTestCase(testCase,
                    _exampleExecutableFilePath,
                    "TestUsingDoctestMain::Empty Class::[UsingDoctestMain] Testing IsEven Always Pass",
                    "[UsingDoctestMain] Testing IsEven Always Pass",
                    sourceFile,
                    12);

            testCase = capturedTestCases.Values[1];
            TestCommon.AssertTestCase(testCase,
                    _exampleExecutableFilePath,
                    "TestUsingDoctestMain::Empty Class::[UsingDoctestMain] Testing IsEven Always Fail",
                    "[UsingDoctestMain] Testing IsEven Always Fail",
                    sourceFile,
                    19);

            testCase = capturedTestCases.Values[2];
            TestCommon.AssertTestCase(testCase,
                    _exampleExecutableFilePath,
                    "TestUsingDoctestMain::Empty Class::[UsingDoctestMain] Testing IsEven Always Skipped",
                    "[UsingDoctestMain] Testing IsEven Always Skipped",
                    sourceFile,
                    26);
        }

        [TestMethod]
        public void DiscoverExeAndDll()
        {
            IEnumerable<string> sources = new List<string>() { _exampleExecutableUsingDLLFilePath };
            Captured<TestCase> capturedTestCases = A.Captured<TestCase>();

            IRunContext discoveryContext = A.Fake<IRunContext>();
            IMessageLogger messageLogger = A.Fake<IMessageLogger>();
            ITestCaseDiscoverySink testCaseDiscoverySink = A.Fake<ITestCaseDiscoverySink>();

            A.CallTo(() => testCaseDiscoverySink.SendTestCase(capturedTestCases._))
                .DoesNothing();

            ITestDiscoverer doctestTestDiscoverer = new DoctestTestDiscoverer();
            doctestTestDiscoverer.DiscoverTests(sources, discoveryContext, messageLogger, testCaseDiscoverySink);
            Assert.IsTrue(capturedTestCases.Values.Count == 6);

            List<string> sourceFiles = Utilities.GetSourceFiles(_exampleExecutableUsingDLLFilePath);
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
                    _exampleExecutableUsingDLLFilePath,
                    "TestDLL::Empty Class::[DLL] Testing IsEven Always Pass",
                    "[DLL] Testing IsEven Always Pass",
                    dllTestSourceFile,
                    8);

            testCase = capturedTestCases.Values[1];
            TestCommon.AssertTestCase(testCase,
                    _exampleExecutableUsingDLLFilePath,
                    "TestDLL::Empty Class::[DLL] Testing IsEven Always Fail",
                    "[DLL] Testing IsEven Always Fail",
                    dllTestSourceFile,
                    15);

            testCase = capturedTestCases.Values[2];
            TestCommon.AssertTestCase(testCase,
                    _exampleExecutableUsingDLLFilePath,
                    "TestDLL::Empty Class::[DLL] Testing IsEven Always Skipped",
                    "[DLL] Testing IsEven Always Skipped",
                    dllTestSourceFile,
                    22);

            // ExecutableUsingDLL Test Cases
            testCase = capturedTestCases.Values[3];
            TestCommon.AssertTestCase(testCase,
                    _exampleExecutableUsingDLLFilePath,
                    "TestExecutableUsingDLL::Empty Class::[ExecutableUsingDLL] Testing IsEven Always Pass",
                    "[ExecutableUsingDLL] Testing IsEven Always Pass",
                    executableUsingDLLTestSourceFile,
                    8);

            testCase = capturedTestCases.Values[4];
            TestCommon.AssertTestCase(testCase,
                    _exampleExecutableUsingDLLFilePath,
                    "TestExecutableUsingDLL::Empty Class::[ExecutableUsingDLL] Testing IsEven Always Fail",
                    "[ExecutableUsingDLL] Testing IsEven Always Fail",
                    executableUsingDLLTestSourceFile,
                    15);

            testCase = capturedTestCases.Values[5];
            TestCommon.AssertTestCase(testCase,
                    _exampleExecutableUsingDLLFilePath,
                    "TestExecutableUsingDLL::Empty Class::[ExecutableUsingDLL] Testing IsEven Always Skipped",
                    "[ExecutableUsingDLL] Testing IsEven Always Skipped",
                    executableUsingDLLTestSourceFile,
                    22);
        }
    }
}
