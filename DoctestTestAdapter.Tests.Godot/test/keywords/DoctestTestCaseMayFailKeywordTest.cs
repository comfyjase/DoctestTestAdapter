// DoctestTestCaseMayFailKeywordTest.cs
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
using DoctestTestAdapter.Shared.Executables;
using DoctestTestAdapter.Shared.Keywords;
using FakeItEasy;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace DoctestTestAdapter.Tests.Godot.Keywords
{
    [TestClass]
    public class DoctestTestCaseMayFailKeywordTest : GodotTest
    {
        [TestMethod]
        public void Find()
        {
            DoctestTestSettingsProvider doctestTestSettingsProvider = new DoctestTestSettingsProvider();
            AssertAndLoadExampleRunSettings(doctestTestSettingsProvider);

            IRunContext runContext = A.Fake<IRunContext>();
            IMessageLogger messageLogger = A.Fake<IMessageLogger>();
            ITestCaseDiscoverySink testCaseDiscoverySink = A.Fake<ITestCaseDiscoverySink>();
            Captured<TestCase> capturedTestCasesFromDiscovery = A.Captured<TestCase>();
            A.CallTo(() => runContext.RunSettings.GetSettings(DoctestTestSettings.RunSettingsXmlNode))
                .Returns(doctestTestSettingsProvider);
            A.CallTo(() => runContext.IsBeingDebugged)
                .Returns(false);
            A.CallTo(() => testCaseDiscoverySink.SendTestCase(capturedTestCasesFromDiscovery._))
                .DoesNothing();

            List<string> _allTestCaseNames = new DoctestExecutable(TestCommon.GodotExecutableFilePath, TestCommon.GodotExamplesSolutionDirectory, settings, runContext, null, null).GetTestCaseNames();

            Assert.IsNotEmpty(_allTestCaseNames);

            List<IKeyword> keywords = new List<IKeyword>()
            {
                new NamespaceKeyword(),
                new DoctestTestCaseMayFailKeyword(_allTestCaseNames)
            };

            string relevantHeaderFile = TestCommon.GodotExamplesSolutionDirectory + "\\tests\\core\\math\\test_random_number_generator.h";
            TestCommon.AssertKeywords(TestCommon.GodotExecutableFilePath,
                relevantHeaderFile,
                keywords,
                (int lineNumber, string testNamespace, List<TestCase> testCases) =>
                {
                    if (lineNumber == 62)
                    {
                        TestCommon.AssertTestCase(testCases.Last(),
                            TestCommon.GodotExecutableFilePath,
                            "TestRandomNumberGenerator::Empty Class::[RandomNumberGenerator] Integer 32 bit",
                            "[RandomNumberGenerator] Integer 32 bit",
                            relevantHeaderFile,
                            62);

                        return true;
                    }

                    return false;
                });
        }
    }
}
