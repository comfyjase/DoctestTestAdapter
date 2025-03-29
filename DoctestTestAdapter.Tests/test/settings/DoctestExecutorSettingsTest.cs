// DoctestExecutorSettingsTest.cs
//
// Copyright (c) 2025-present Jase Mottershead
//
// MIT License
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using DoctestTestAdapter.Settings;
using DoctestTestAdapter.Shared.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoctestTestAdapter.Tests.Settings
{
    [TestClass]
    public class DoctestExecutorSettingsTest
    {
        private string _solutionDirectory = Utilities.GetSolutionDirectory(TestCommon.UsingDoctestMainExecutableFilePath);

        [TestMethod]
        public void ExecutableOverrideRelative()
        {
            TestCommon.AssertErrorOutput(() =>
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
            });
        }

        [TestMethod]
        public void ExecutableOverrideAbsolute()
        {
            TestCommon.AssertErrorOutput(() =>
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
            });
        }

        [TestMethod]
        public void ExecutableOverrideInvalid()
        {
            TestCommon.AssertErrorOutput(() =>
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
            });
        }
    }
}
