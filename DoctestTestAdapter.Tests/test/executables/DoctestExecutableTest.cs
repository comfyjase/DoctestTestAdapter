// DoctestExecutableTest.cs
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

using DoctestTestAdapter.Shared.Executables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace DoctestTestAdapter.Tests.Executables
{
    [TestClass]
    public class DoctestExecutableTest
    {
        private DoctestExecutable _doctestExecutableExe = new DoctestExecutable(TestCommon.UsingDoctestMainExecutableFilePath, TestCommon.ExamplesSolutionDirectory, null, null, null, null);
        private DoctestExecutable _doctestExecutableExeAndDLL = new DoctestExecutable(TestCommon.ExecutableUsingDLLExecutableFilePath, TestCommon.ExamplesSolutionDirectory, null, null, null, null);

        [TestMethod]
        public void TestSuiteNamesExe()
        {
            List<string> testSuiteNames = _doctestExecutableExe.GetTestSuiteNames();
            Assert.IsNotEmpty(testSuiteNames);
            Assert.HasCount(2, testSuiteNames);

            foreach (string testSuiteName in testSuiteNames)
            {
                Assert.IsFalse(string.IsNullOrEmpty(testSuiteName));
            }

            Assert.AreEqual("[UsingDoctestMainTestSuite]", testSuiteNames[0]);
            Assert.AreEqual("[UsingDoctestMainNamespaceAndTestSuite_TestSuite]", testSuiteNames[1]);
        }

        [TestMethod]
        public void TestCaseNamesExe()
        {
            List<string> testCaseNames = _doctestExecutableExe.GetTestCaseNames();
            Assert.IsNotEmpty(testCaseNames);
            Assert.HasCount(25, testCaseNames);
            TestCommon.AssertTestCaseNames(testCaseNames, "[UsingDoctestMain]");
        }

        [TestMethod]
        public void TestSuiteNamesDLL()
        {
            // Note, you have to use the exe that loads the DLL, can't just use the DLL file like the other DLL tests do.
            // This is because we need to be able to _run_ a process off of this executable to get the test suite names.
            // You can't just run a DLL file.
            List<string> testSuiteNames = _doctestExecutableExeAndDLL.GetTestSuiteNames();
            Assert.IsNotEmpty(testSuiteNames);
            Assert.HasCount(4, testSuiteNames);

            foreach (string testSuiteName in testSuiteNames)
            {
                Assert.IsFalse(string.IsNullOrEmpty(testSuiteName));
            }

            // DLL Test Suite Names
            Assert.AreEqual("[DLLTestSuite]", testSuiteNames[0]);
            Assert.AreEqual("[DLLNamespaceAndTestSuite_TestSuite]", testSuiteNames[1]);

            // Exe Using DLL Test Suite Names
            Assert.AreEqual("[ExecutableUsingDLLTestSuite]", testSuiteNames[2]);
            Assert.AreEqual("[ExecutableUsingDLLNamespaceAndTestSuite_TestSuite]", testSuiteNames[3]);
        }

        [TestMethod]
        public void TestCaseNamesDLL()
        {
            // Same as the TestSuiteNamesDLL unit test - can't just run a DLL file, so use the exe that loads the DLL.
            List<string> testCaseNames = _doctestExecutableExeAndDLL.GetTestCaseNames();
            Assert.IsNotEmpty(testCaseNames);
            Assert.HasCount(50, testCaseNames);

            List<string> testCaseNamesFromDLL = testCaseNames
                .Where(s => s.StartsWith("[DLL]"))
                .ToList();
            List<string> testCaseNamesFromExeUsingDLL = testCaseNames
                .Where(s => s.StartsWith("[ExecutableUsingDLL]"))
                .ToList();

            TestCommon.AssertTestCaseNames(testCaseNamesFromDLL, "[DLL]");
            TestCommon.AssertTestCaseNames(testCaseNamesFromExeUsingDLL, "[ExecutableUsingDLL]");
        }
    }
}
