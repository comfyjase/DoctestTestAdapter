using DoctestTestAdapter.Settings;
using DoctestTestAdapter.Shared.Helpers;
using FakeItEasy;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using TestResult = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult;

namespace DoctestTestAdapter.Tests.Execution
{
	[TestClass]
	public class DoctestTestExecutorTest
	{
        [TestMethod]
		public void ExecuteExe()
		{
            IFrameworkHandle frameworkHandle = A.Fake<IFrameworkHandle>();
            Captured<TestMessageLevel> capturedTestMessageLevels = A.Captured<TestMessageLevel>();
            Captured<string> capturedTestMessages = A.Captured<string>();
            A.CallTo(() => frameworkHandle.SendMessage(capturedTestMessageLevels._, capturedTestMessages._))
               .DoesNothing();

            List<TestCase> testCases = Utilities.GetTestCases(TestCommon.UsingDoctestMainExecutableFilePath, frameworkHandle);
            Assert.HasCount(25, testCases);

            Assert.IsEmpty(capturedTestMessageLevels.Values);
            Assert.IsEmpty(capturedTestMessages.Values);

            TestCommon.AssertTestCases(testCases, 
                TestCommon.UsingDoctestMainExecutableFilePath,
                "UsingDoctestMain",
                TestCommon.UsingDoctestMainTestHeaderFilePath);

            Captured<TestCase> capturedTestCases = A.Captured<TestCase>();
            Captured<TestResult> capturedTestResults = A.Captured<TestResult>();
            IRunContext runContext = A.Fake<IRunContext>();
            A.CallTo(() => frameworkHandle.RecordStart(capturedTestCases._))
                .DoesNothing();
            A.CallTo(() => frameworkHandle.RecordResult(capturedTestResults._))
                .DoesNothing();

            ITestExecutor doctestTestExecutor = new DoctestTestExecutor();
            doctestTestExecutor.RunTests(testCases, runContext, frameworkHandle);

            Assert.HasCount(1, capturedTestMessageLevels.Values);
            Assert.HasCount(1, capturedTestMessages.Values);
            Assert.AreEqual(TestMessageLevel.Informational, capturedTestMessageLevels.Values[0]);
            Assert.IsTrue(capturedTestMessages.Values[0].Contains(Shared.Helpers.Constants.InformationMessagePrefix + " - About to start exe UsingDoctestMain.exe with command arguments: "));

            Assert.HasCount(25, capturedTestResults.Values);
            TestCommon.AssertTestResults(capturedTestResults.Values.ToList());
        }

        [TestMethod]
        public void ExecuteExeAndDLL()
        {
            IFrameworkHandle frameworkHandle = A.Fake<IFrameworkHandle>();
            Captured<TestMessageLevel> capturedTestMessageLevels = A.Captured<TestMessageLevel>();
            Captured<string> capturedTestMessages = A.Captured<string>();
            A.CallTo(() => frameworkHandle.SendMessage(capturedTestMessageLevels._, capturedTestMessages._))
               .DoesNothing();

            List<TestCase> testCases = Utilities.GetTestCases(TestCommon.ExecutableUsingDLLExecutableFilePath, frameworkHandle);
            Assert.HasCount(50, testCases);

            Assert.IsEmpty(capturedTestMessageLevels.Values);
            Assert.IsEmpty(capturedTestMessages.Values);

            List<TestCase> dllTestCases = testCases
                .ToList()
                .Where(t => t.DisplayName.Contains("[DLL]"))
                .ToList();
            List<TestCase> executableUsingDLLTestCases = testCases
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

            Captured<TestCase> capturedTestCases = A.Captured<TestCase>();
            Captured<TestResult> capturedTestResults = A.Captured<TestResult>();
            IRunContext runContext = A.Fake<IRunContext>();
            A.CallTo(() => frameworkHandle.RecordStart(capturedTestCases._))
                .DoesNothing();
            A.CallTo(() => frameworkHandle.RecordResult(capturedTestResults._))
                .DoesNothing();

            ITestExecutor doctestTestExecutor = new DoctestTestExecutor();
            doctestTestExecutor.RunTests(testCases, runContext, frameworkHandle);

            Assert.HasCount(1, capturedTestMessageLevels.Values);
            Assert.HasCount(1, capturedTestMessages.Values);
            Assert.AreEqual(TestMessageLevel.Informational, capturedTestMessageLevels.Values[0]);
            Assert.IsTrue(capturedTestMessages.Values[0].Contains(Shared.Helpers.Constants.InformationMessagePrefix + " - About to start exe ExecutableUsingDLL.exe with command arguments: "));

            Assert.HasCount(50, capturedTestResults.Values);
            List<TestResult> dllTestResults = capturedTestResults.Values
                .Where(t => t.TestCase.DisplayName.Contains("[DLL]"))
                .ToList();
            List<TestResult> executableUsingDLLTestResults = capturedTestResults.Values
                .Where(t => t.TestCase.DisplayName.Contains("[ExecutableUsingDLL]"))
                .ToList();
            TestCommon.AssertTestResults(dllTestResults);
            TestCommon.AssertTestResults(executableUsingDLLTestResults);
        }

        [TestMethod]
        public void ExecuteExeWithExeOverrideSetting()
        {
            IFrameworkHandle frameworkHandle = A.Fake<IFrameworkHandle>();
            Captured<TestMessageLevel> capturedTestMessageLevels = A.Captured<TestMessageLevel>();
            Captured<string> capturedTestMessages = A.Captured<string>();
            A.CallTo(() => frameworkHandle.SendMessage(capturedTestMessageLevels._, capturedTestMessages._))
               .DoesNothing();

            List<TestCase> testCases = Utilities.GetTestCases(TestCommon.UsingDoctestMainExecutableFilePath, frameworkHandle);
            Assert.HasCount(25, testCases);

            Assert.IsEmpty(capturedTestMessageLevels.Values);
            Assert.IsEmpty(capturedTestMessages.Values);

            TestCommon.AssertTestCases(testCases,
                TestCommon.UsingDoctestMainExecutableFilePath,
                "UsingDoctestMain",
                TestCommon.UsingDoctestMainTestHeaderFilePath);

            Captured<TestCase> capturedTestCases = A.Captured<TestCase>();
            Captured<TestResult> capturedTestResults = A.Captured<TestResult>();
            IRunContext runContext = A.Fake<IRunContext>();
            A.CallTo(() => frameworkHandle.RecordStart(capturedTestCases._))
                .DoesNothing();
            A.CallTo(() => frameworkHandle.RecordResult(capturedTestResults._))
                .DoesNothing();
            DoctestTestSettingsProvider settingsProvider = new DoctestTestSettingsProvider();
            DoctestTestSettings doctestSettings = TestCommon.LoadDoctestSettings(settingsProvider, TestCommon.ExecutorRunSettingsRelativeExecutableOverrideExample);
            A.CallTo(() => runContext.RunSettings.GetSettings(DoctestTestSettings.RunSettingsXmlNode))
                .Returns(settingsProvider);

            ITestExecutor doctestTestExecutor = new DoctestTestExecutor();
            doctestTestExecutor.RunTests(testCases, runContext, frameworkHandle);

            Assert.HasCount(1, capturedTestMessageLevels.Values);
            Assert.HasCount(1, capturedTestMessages.Values);
            Assert.AreEqual(TestMessageLevel.Informational, capturedTestMessageLevels.Values[0]);
            Assert.IsTrue(capturedTestMessages.Values[0].Contains(Shared.Helpers.Constants.InformationMessagePrefix + " - About to start exe UsingCustomMain.exe with command arguments: "));
        }
	}
}
