using FakeItEasy;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

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
            Assert.HasCount(25, capturedTestCases.Values);

            TestCommon.AssertTestCases(capturedTestCases.Values.ToList(),
                TestCommon.UsingDoctestMainExecutableFilePath,
                "UsingDoctestMain",
                TestCommon.UsingDoctestMainTestHeaderFilePath);
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
            Assert.HasCount(50, capturedTestCases.Values);

            List<TestCase> dllTestCases = capturedTestCases.Values
                .ToList()
                .Where(t => t.DisplayName.Contains("[DLL]"))
                .ToList();
            List<TestCase> executableUsingDLLTestCases = capturedTestCases.Values
                .ToList()
                .Where(t => t.DisplayName.Contains("[ExecutableUsingDLL]"))
                .ToList();

            TestCommon.AssertTestCases(dllTestCases,
                TestCommon.ExecutableUsingDLLExecutableFilePath,
                "DLL",
                TestCommon.DLLTestHeaderFilePath
            );
            TestCommon.AssertTestCases(executableUsingDLLTestCases,
                TestCommon.ExecutableUsingDLLExecutableFilePath,
                "ExecutableUsingDLL",
                TestCommon.ExecutableUsingDLLTestHeaderFilePath
            );
        }
    }
}
