using FakeItEasy;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DoctestTestAdapter.Tests.Discovery
{
    [TestClass]
    public class DoctestTestDiscovererTest
    {
        [TestMethod]
        public void DiscoverExe()
        {
            IEnumerable<string> sources = new List<string>(){ TestCommon.UsingDoctestMainExecutableFilePath };

            Captured<TestCase> capturedTestCases = A.Captured<TestCase>();
            IRunContext discoveryContext = A.Fake<IRunContext>();
            IMessageLogger messageLogger = A.Fake<IMessageLogger>();
            ITestCaseDiscoverySink testCaseDiscoverySink = A.Fake<ITestCaseDiscoverySink>();

            A.CallTo(() => testCaseDiscoverySink.SendTestCase(capturedTestCases._))
                .DoesNothing();

            ITestDiscoverer doctestTestDiscoverer = new DoctestTestDiscoverer();
            doctestTestDiscoverer.DiscoverTests(sources, discoveryContext, messageLogger, testCaseDiscoverySink);
            Assert.IsTrue(capturedTestCases.Values.Count == 4);

            TestCase testCase = capturedTestCases.Values[0];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.UsingDoctestMainExecutableFilePath,
                    "TestUsingDoctestMain::Empty Class::[UsingDoctestMain] Testing IsEven Always Pass",
                    "[UsingDoctestMain] Testing IsEven Always Pass",
                    TestCommon.UsingDoctestMainTestHeaderFile,
                    50);

            testCase = capturedTestCases.Values[1];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.UsingDoctestMainExecutableFilePath,
                    "TestUsingDoctestMain::Empty Class::[UsingDoctestMain] Testing IsEven Always Fail",
                    "[UsingDoctestMain] Testing IsEven Always Fail",
                    TestCommon.UsingDoctestMainTestHeaderFile,
                    57);

            testCase = capturedTestCases.Values[2];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.UsingDoctestMainExecutableFilePath,
                    "TestUsingDoctestMain::Empty Class::[UsingDoctestMain] Testing IsEven Always Skipped",
                    "[UsingDoctestMain] Testing IsEven Always Skipped",
                    TestCommon.UsingDoctestMainTestHeaderFile,
                    64);

            testCase = capturedTestCases.Values[3];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.UsingDoctestMainExecutableFilePath,
                    "TestUsingDoctestMain::Empty Class::[UsingDoctestMain] Testing IsEven, Always Pass, With Commas In Name",
                    "[UsingDoctestMain] Testing IsEven, Always Pass, With Commas In Name",
                    TestCommon.UsingDoctestMainTestHeaderFile,
                    71);
        }

        [TestMethod]
        public void DiscoverExeAndDLL()
        {
            IEnumerable<string> sources = new List<string>() { TestCommon.ExecutableUsingDLLExecutableFilePath };
            Captured<TestCase> capturedTestCases = A.Captured<TestCase>();

            IRunContext discoveryContext = A.Fake<IRunContext>();
            IMessageLogger messageLogger = A.Fake<IMessageLogger>();
            ITestCaseDiscoverySink testCaseDiscoverySink = A.Fake<ITestCaseDiscoverySink>();

            A.CallTo(() => testCaseDiscoverySink.SendTestCase(capturedTestCases._))
                .DoesNothing();

            ITestDiscoverer doctestTestDiscoverer = new DoctestTestDiscoverer();
            doctestTestDiscoverer.DiscoverTests(sources, discoveryContext, messageLogger, testCaseDiscoverySink);
            Assert.IsTrue(capturedTestCases.Values.Count == 8);

            // DLL Test Cases
            TestCase testCase = capturedTestCases.Values[0];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.ExecutableUsingDLLExecutableFilePath,
                    "TestDLL::Empty Class::[DLL] Testing IsEven Always Pass",
                    "[DLL] Testing IsEven Always Pass",
                    TestCommon.DLLTestHeaderFile,
                    46);

            testCase = capturedTestCases.Values[1];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.ExecutableUsingDLLExecutableFilePath,
                    "TestDLL::Empty Class::[DLL] Testing IsEven Always Fail",
                    "[DLL] Testing IsEven Always Fail",
                    TestCommon.DLLTestHeaderFile,
                    53);

            testCase = capturedTestCases.Values[2];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.ExecutableUsingDLLExecutableFilePath,
                    "TestDLL::Empty Class::[DLL] Testing IsEven Always Skipped",
                    "[DLL] Testing IsEven Always Skipped",
                    TestCommon.DLLTestHeaderFile,
                    60);

            testCase = capturedTestCases.Values[3];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.ExecutableUsingDLLExecutableFilePath,
                    "TestDLL::Empty Class::[DLL] Testing IsEven, Always Pass, With Commas In Name",
                    "[DLL] Testing IsEven, Always Pass, With Commas In Name",
                    TestCommon.DLLTestHeaderFile,
                    67);

            // ExecutableUsingDLL Test Cases
            testCase = capturedTestCases.Values[4];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.ExecutableUsingDLLExecutableFilePath,
                    "TestExecutableUsingDLL::Empty Class::[ExecutableUsingDLL] Testing IsEven Always Pass",
                    "[ExecutableUsingDLL] Testing IsEven Always Pass",
                    TestCommon.ExecutableUsingDLLTestHeaderFile,
                    46);

            testCase = capturedTestCases.Values[5];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.ExecutableUsingDLLExecutableFilePath,
                    "TestExecutableUsingDLL::Empty Class::[ExecutableUsingDLL] Testing IsEven Always Fail",
                    "[ExecutableUsingDLL] Testing IsEven Always Fail",
                    TestCommon.ExecutableUsingDLLTestHeaderFile,
                    53);

            testCase = capturedTestCases.Values[6];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.ExecutableUsingDLLExecutableFilePath,
                    "TestExecutableUsingDLL::Empty Class::[ExecutableUsingDLL] Testing IsEven Always Skipped",
                    "[ExecutableUsingDLL] Testing IsEven Always Skipped",
                    TestCommon.ExecutableUsingDLLTestHeaderFile,
                    60);

            testCase = capturedTestCases.Values[7];
            TestCommon.AssertTestCase(testCase,
                    TestCommon.ExecutableUsingDLLExecutableFilePath,
                    "TestExecutableUsingDLL::Empty Class::[ExecutableUsingDLL] Testing IsEven, Always Pass, With Commas In Name",
                    "[ExecutableUsingDLL] Testing IsEven, Always Pass, With Commas In Name",
                    TestCommon.ExecutableUsingDLLTestHeaderFile,
                    67);
        }
    }
}
