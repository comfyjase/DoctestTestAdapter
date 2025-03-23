// DoctestTestSettingsTest.cs
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
using FakeItEasy;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DoctestTestAdapter.Tests.Settings
{
	[TestClass]
	public class DoctestTestSettingsTest
	{
		[TestMethod]
		public void Load() =>
            Assert.IsNotNull(DoctestTestSettingsProvider.LoadSettings(A.Fake<IRunContext>()));

        [TestMethod]
        public void CommandArgumentsHelper()
        {
            DoctestTestSettings doctestSettings = TestCommon.LoadDoctestSettings(new DoctestTestSettingsProvider(), TestCommon.GeneralRunSettingsExample);
            Assert.IsNotNull(doctestSettings);
            Assert.IsTrue(doctestSettings.TryGetCommandArguments(out string commandArguments));
            Assert.IsFalse(string.IsNullOrEmpty(commandArguments));
            Assert.AreEqual("--test", commandArguments);
        }

        [TestMethod]
        public void PrintStandardOutputHelper()
        {
            DoctestTestSettings doctestSettings = TestCommon.LoadDoctestSettings(new DoctestTestSettingsProvider(), TestCommon.GeneralRunSettingsExample);
            Assert.IsNotNull(doctestSettings);
            Assert.IsTrue(doctestSettings.TryGetPrintStandardOutput(out bool printStandardOutput));
            Assert.IsTrue(printStandardOutput);
        }

        [TestMethod]
        public void SearchDirectoriesHelper()
        {
            DoctestTestSettings doctestSettings = TestCommon.LoadDoctestSettings(new DoctestTestSettingsProvider(), TestCommon.DiscoveryRunSettingsRelativeSearchDirectoryExample);

            Assert.IsNotNull(doctestSettings);
            Assert.IsTrue(doctestSettings.TryGetSearchDirectories(out List<string> searchDirectories));
            Assert.IsNotNull(searchDirectories);
            Assert.IsNotEmpty(searchDirectories);
            Assert.HasCount(1, searchDirectories);
            Assert.AreEqual("UsingDoctestMain", searchDirectories[0]);
        }

        [TestMethod]
        public void ExecutableOverridesHelper()
        {
            DoctestTestSettings doctestSettings = TestCommon.LoadDoctestSettings(new DoctestTestSettingsProvider(), TestCommon.ExecutorRunSettingsRelativeExecutableOverrideExample);

            Assert.IsNotNull(doctestSettings);
            Assert.IsTrue(doctestSettings.TryGetExecutableOverrides(out List<ExecutableOverride> executableOverrides));
            Assert.IsNotNull(executableOverrides);
            Assert.IsNotEmpty(executableOverrides);
            Assert.HasCount(1, executableOverrides);
#if DEBUG
            Assert.AreEqual("bin\\x64\\Debug\\UsingDoctestMain\\UsingDoctestMain.exe", executableOverrides[0].Key);
            Assert.AreEqual("bin\\x64\\Debug\\UsingCustomMain\\UsingCustomMain.exe", executableOverrides[0].Value);
#else
            Assert.AreEqual("bin\\x64\\Release\\UsingDoctestMain\\UsingDoctestMain.exe", executableOverrides[0].Key);
            Assert.AreEqual("bin\\x64\\Release\\UsingCustomMain\\UsingCustomMain.exe", executableOverrides[0].Value);
#endif
        }
    }
}
