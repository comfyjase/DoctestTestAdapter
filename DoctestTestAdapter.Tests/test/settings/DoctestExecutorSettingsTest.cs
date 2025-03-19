using DoctestTestAdapter.Settings;
using DoctestTestAdapter.Shared.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace DoctestTestAdapter.Tests.Settings
{
    [TestClass]
    public class DoctestExecutorSettingsTest
    {
        private string _solutionDirectory = Utilities.GetSolutionDirectory(Directory.GetParent(TestCommon.UsingDoctestMainExecutableFilePath).FullName);

        [TestMethod]
        public void ExecutableOverrideRelative()
        {
            DoctestTestSettings doctestSettings = TestCommon.LoadDoctestSettings(new DoctestTestSettingsProvider(), TestCommon.ExecutorRunSettingsRelativeExecutableOverrideExample);

            Assert.IsNotNull(doctestSettings);
            Assert.IsNotNull(doctestSettings.ExecutorSettings);
            Assert.IsNotNull(doctestSettings.ExecutorSettings.ExecutableOverrides);
            Assert.IsNotEmpty(doctestSettings.ExecutorSettings.ExecutableOverrides);
            Assert.HasCount(1, doctestSettings.ExecutorSettings.ExecutableOverrides);

#if DEBUG
            Assert.AreEqual("bin\\x64\\Debug\\UsingDoctestMain\\UsingDoctestMain.exe", doctestSettings.ExecutorSettings.ExecutableOverrides[0].Key);
            Assert.AreEqual("bin\\x64\\Debug\\UsingCustomMain\\UsingCustomMain.exe", doctestSettings.ExecutorSettings.ExecutableOverrides[0].Value);
#else
            Assert.AreEqual("bin\\x64\\Release\\UsingDoctestMain\\UsingDoctestMain.exe", doctestSettings.ExecutorSettings.ExecutableOverrides[0].Key);
            Assert.AreEqual("bin\\x64\\Release\\UsingCustomMain\\UsingCustomMain.exe", doctestSettings.ExecutorSettings.ExecutableOverrides[0].Value);
#endif

            Assert.IsTrue(doctestSettings.ExecutorSettings.AreExecutableOverridesValid(_solutionDirectory, out string message));
            Assert.IsTrue(string.IsNullOrEmpty(message));
        }

        [TestMethod]
        public void ExecutableOverrideAbsolute()
        {
            DoctestTestSettings doctestSettings = TestCommon.LoadDoctestSettings(new DoctestTestSettingsProvider(), TestCommon.ExecutorRunSettingsAbsoluteExecutableOverrideExample);

            Assert.IsNotNull(doctestSettings);
            Assert.IsNotNull(doctestSettings.ExecutorSettings);
            Assert.IsNotNull(doctestSettings.ExecutorSettings.ExecutableOverrides);
            Assert.IsNotEmpty(doctestSettings.ExecutorSettings.ExecutableOverrides);
            Assert.HasCount(1, doctestSettings.ExecutorSettings.ExecutableOverrides);

#if DEBUG
            Assert.AreEqual(_solutionDirectory + "\\bin\\x64\\Debug\\UsingDoctestMain\\UsingDoctestMain.exe", doctestSettings.ExecutorSettings.ExecutableOverrides[0].Key);
            Assert.AreEqual(_solutionDirectory + "\\bin\\x64\\Debug\\UsingCustomMain\\UsingCustomMain.exe", doctestSettings.ExecutorSettings.ExecutableOverrides[0].Value);
#else
            Assert.AreEqual(_solutionDirectory + "\\bin\\x64\\Release\\UsingDoctestMain\\UsingDoctestMain.exe", doctestSettings.ExecutorSettings.ExecutableOverrides[0].Key);
            Assert.AreEqual(_solutionDirectory + "\\bin\\x64\\Release\\UsingCustomMain\\UsingCustomMain.exe", doctestSettings.ExecutorSettings.ExecutableOverrides[0].Value);
#endif

            Assert.IsTrue(doctestSettings.ExecutorSettings.AreExecutableOverridesValid(_solutionDirectory, out string message));
            Assert.IsTrue(string.IsNullOrEmpty(message));
        }

        [TestMethod]
        public void ExecutableOverrideInvalid()
        {
            DoctestTestSettings doctestSettings = TestCommon.LoadDoctestSettings(new DoctestTestSettingsProvider(), TestCommon.ExecutorRunSettingsInvalidExecutableOverrideExample);

            Assert.IsNotNull(doctestSettings);
            Assert.IsNotNull(doctestSettings.ExecutorSettings);
            Assert.IsNotNull(doctestSettings.ExecutorSettings.ExecutableOverrides);
            Assert.IsNotEmpty(doctestSettings.ExecutorSettings.ExecutableOverrides);
            Assert.HasCount(1, doctestSettings.ExecutorSettings.ExecutableOverrides);

            Assert.AreEqual("NonExistentDirectoryA\\To\\NonExistentExecutableA.exe", doctestSettings.ExecutorSettings.ExecutableOverrides[0].Key);
            Assert.AreEqual("NonExistentDirectoryB\\To\\NonExistentExecutableB.exe", doctestSettings.ExecutorSettings.ExecutableOverrides[0].Value);

            Assert.IsFalse(doctestSettings.ExecutorSettings.AreExecutableOverridesValid(_solutionDirectory, out string message));
            Assert.IsFalse(string.IsNullOrEmpty(message));
        }
    }
}
