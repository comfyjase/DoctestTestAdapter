﻿using DoctestTestAdapter.Settings;
using DoctestTestAdapter.Shared.Helpers;
using FakeItEasy;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoctestTestAdapter.Tests.Execution
{
    [TestClass]
    public class GodotDoctestExecutorTest
    {
        [TestMethod]
        public void ExecuteExe()
        {
            string godotExecutableFilePath = Utilities.GetSolutionDirectory() + "\\..\\godot\\bin\\godot.windows.editor.dev.x86_64.exe";
            Assert.IsTrue(File.Exists(godotExecutableFilePath));

            string godotSolutionDirectory = Utilities.GetSolutionDirectory(Directory.GetParent(godotExecutableFilePath).FullName);
            Assert.IsTrue(Directory.Exists(godotSolutionDirectory));

            godotExecutableFilePath = godotSolutionDirectory + "\\bin\\godot.windows.editor.dev.x86_64.exe";
            Assert.IsTrue(File.Exists(godotExecutableFilePath));

            string godotConsoleExecutableFilePath = godotSolutionDirectory + "\\bin\\godot.windows.editor.dev.x86_64.console.exe";
            Assert.IsTrue(File.Exists(godotConsoleExecutableFilePath));

            string godotRunSettingsFile = godotSolutionDirectory + "\\.runsettings";
            Assert.IsTrue(File.Exists(godotRunSettingsFile));

            IEnumerable<string> sources = new List<string>() { godotExecutableFilePath };

            DoctestTestSettingsProvider settingsProvider = new DoctestTestSettingsProvider();
            using (XmlReader xmlReader = XmlReader.Create(godotRunSettingsFile))
            {
                xmlReader.MoveToContent();

                using (XmlReader subXmlReader = XmlReader.Create(godotRunSettingsFile))
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

            DoctestTestSettings settings = DoctestTestSettingsProvider.LoadSettings(runContext);
            Assert.IsTrue(settings != null);

            Assert.IsTrue(settings.DiscoverySettings != null);
            Assert.IsTrue(settings.DiscoverySettings.SearchDirectories.Count == 2);
            Assert.IsTrue(!string.IsNullOrEmpty(settings.DiscoverySettings.SearchDirectories[0]));
            Assert.IsTrue(!string.IsNullOrEmpty(settings.DiscoverySettings.SearchDirectories[1]));
            Assert.AreEqual(settings.DiscoverySettings.SearchDirectories[0], "modules");
            Assert.AreEqual(settings.DiscoverySettings.SearchDirectories[1], "tests");

            Assert.IsTrue(settings.ExecutorSettings != null);
            Assert.IsTrue(!string.IsNullOrEmpty(settings.ExecutorSettings.CommandArguments));
            Assert.AreEqual(settings.ExecutorSettings.CommandArguments, "--headless --test");
            //Assert.IsTrue(settings.ExecutorSettings.ExecutableOverrides.Count == 1);
            //Assert.IsTrue(!string.IsNullOrEmpty(settings.ExecutorSettings.ExecutableOverrides[0].Key));
            //Assert.IsTrue(!string.IsNullOrEmpty(settings.ExecutorSettings.ExecutableOverrides[0].Value));
            //Assert.AreEqual(settings.ExecutorSettings.ExecutableOverrides[0].Key, "bin\\godot.windows.editor.dev.x86_64.exe");
            //Assert.AreEqual(settings.ExecutorSettings.ExecutableOverrides[0].Value, "bin\\godot.windows.editor.dev.x86_64.console.exe");

            Captured<TestCase> capturedTestCasesFromDiscovery = A.Captured<TestCase>();
            IMessageLogger messageLogger = A.Fake<IMessageLogger>();
            ITestCaseDiscoverySink testCaseDiscoverySink = A.Fake<ITestCaseDiscoverySink>();

            A.CallTo(() => testCaseDiscoverySink.SendTestCase(capturedTestCasesFromDiscovery._))
                .DoesNothing();

            // Godot discover
            Captured<TestCase> capturedTestCasesFromExecutor = A.Captured<TestCase>();
            ITestDiscoverer doctestTestDiscoverer = new DoctestTestDiscoverer();
            doctestTestDiscoverer.DiscoverTests(sources, runContext, messageLogger, testCaseDiscoverySink);
            Assert.IsTrue(capturedTestCasesFromDiscovery.Values.Count == 874);

            // Godot executor
            Captured<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult> capturedTestResults = A.Captured<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult>();
            IFrameworkHandle frameworkHandle = A.Fake<IFrameworkHandle>();
            A.CallTo(() => frameworkHandle.RecordStart(capturedTestCasesFromExecutor._))
                .DoesNothing();
            A.CallTo(() => frameworkHandle.RecordResult(capturedTestResults._))
                .DoesNothing();

            ITestExecutor doctestTestExecutor = new DoctestTestExecutor();
            doctestTestExecutor.RunTests(capturedTestCasesFromDiscovery.Values, runContext, frameworkHandle);
            Assert.IsTrue(capturedTestCasesFromDiscovery.Values.Count == capturedTestCasesFromExecutor.Values.Count);
            Assert.IsTrue(capturedTestCasesFromDiscovery.Values.Count == capturedTestResults.Values.Count);
        }
    }
}
