// DoctestDiscoverySettingsTest.cs
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
    public class DoctestDiscoverySettingsTest
    {
        private string _solutionDirectory = Utilities.GetSolutionDirectory(TestCommon.UsingDoctestMainExecutableFilePath);

        [TestMethod]
        public void SearchDirectoriesRelative()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                DoctestTestSettings doctestSettings = TestCommon.LoadDoctestSettings(new DoctestTestSettingsProvider(), TestCommon.DiscoveryRunSettingsRelativeSearchDirectoryExample);

                Assert.IsNotNull(doctestSettings);
                Assert.IsNotNull(doctestSettings.DiscoverySettings);
                Assert.IsNotNull(doctestSettings.DiscoverySettings.SearchDirectories);
                Assert.IsNotEmpty(doctestSettings.DiscoverySettings.SearchDirectories);
                Assert.HasCount(1, doctestSettings.DiscoverySettings.SearchDirectories);
                Assert.AreEqual("UsingDoctestMain", doctestSettings.DiscoverySettings.SearchDirectories[0]);

                Assert.IsTrue(doctestSettings.DiscoverySettings.AreSearchDirectoriesValid(_solutionDirectory, out string message));
                Assert.IsTrue(string.IsNullOrEmpty(message));
            });
        }

        [TestMethod]
        public void SearchDirectoriesAbsolute()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                DoctestTestSettings doctestSettings = TestCommon.LoadDoctestSettings(new DoctestTestSettingsProvider(), TestCommon.DiscoveryRunSettingsAbsoluteSearchDirectoryExample);

                Assert.IsNotNull(doctestSettings);
                Assert.IsNotNull(doctestSettings.DiscoverySettings);
                Assert.IsNotNull(doctestSettings.DiscoverySettings.SearchDirectories);
                Assert.IsNotEmpty(doctestSettings.DiscoverySettings.SearchDirectories);
                Assert.HasCount(1, doctestSettings.DiscoverySettings.SearchDirectories);
                Assert.AreEqual(_solutionDirectory + "\\UsingDoctestMain", doctestSettings.DiscoverySettings.SearchDirectories[0]);

                Assert.IsTrue(doctestSettings.DiscoverySettings.AreSearchDirectoriesValid(_solutionDirectory, out string message));
                Assert.IsTrue(string.IsNullOrEmpty(message));
            });
        }

        [TestMethod]
        public void SearchDirectoriesInvalid()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                DoctestTestSettings doctestSettings = TestCommon.LoadDoctestSettings(new DoctestTestSettingsProvider(), TestCommon.DiscoveryRunSettingsInvalidSearchDirectoryExample);

                Assert.IsNotNull(doctestSettings);
                Assert.IsNotNull(doctestSettings.DiscoverySettings);
                Assert.IsNotNull(doctestSettings.DiscoverySettings.SearchDirectories);
                Assert.IsNotEmpty(doctestSettings.DiscoverySettings.SearchDirectories);
                Assert.HasCount(1, doctestSettings.DiscoverySettings.SearchDirectories);
                Assert.AreEqual("NonExistentDirectory", doctestSettings.DiscoverySettings.SearchDirectories[0]);

                Assert.IsFalse(doctestSettings.DiscoverySettings.AreSearchDirectoriesValid(_solutionDirectory, out string message));
                Assert.IsFalse(string.IsNullOrEmpty(message));
            });
        }
    }
}
