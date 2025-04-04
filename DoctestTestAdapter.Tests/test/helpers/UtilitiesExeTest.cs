﻿// UtilitiesExeTest.cs
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

using DoctestTestAdapter.Shared.Helpers;
using FakeItEasy;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace DoctestTestAdapter.Tests.Helpers
{
    [TestClass]
    public class UtilitiesExeTest
    {
        [TestMethod]
        public void SolutionDirectory()
            => Assert.IsTrue(Utilities.GetSolutionDirectory(Directory.GetParent(TestCommon.UsingDoctestMainExecutableFilePath).FullName).EndsWith("DoctestTestAdapter.Examples"));

        [TestMethod]
        public void PDBFilePath()
        {
            string pdbFilePath = Utilities.GetPDBFilePath(TestCommon.UsingDoctestMainExecutableFilePath);
            Assert.IsFalse(string.IsNullOrEmpty(pdbFilePath));
            Assert.IsTrue(File.Exists(pdbFilePath));
            Assert.AreEqual(TestCommon.UsingDoctestMainPdbFilePath, pdbFilePath);
        }

        [TestMethod]
        public void Dependencies()
        {
            // Using ExecutableUsingDLL instead of UsingDoctestMain to make sure DLL is picked up as a dependency.
            List<string> dependencies = Utilities.GetDependencies(TestCommon.ExecutableUsingDLLExecutableFilePath);

            Assert.IsNotEmpty(dependencies);

#if DEBUG
            Assert.HasCount(5, dependencies);

            foreach (string dependency in dependencies)
            {
                Assert.IsFalse(string.IsNullOrEmpty(dependency));
            }

            Assert.AreEqual("DLL.dll", dependencies[0]);
            Assert.AreEqual("KERNEL32.dll", dependencies[1]);
            Assert.AreEqual("VCRUNTIME140_1D.dll", dependencies[2]);
            Assert.AreEqual("VCRUNTIME140D.dll", dependencies[3]);
            Assert.AreEqual("ucrtbased.dll", dependencies[4]);
#else
            Assert.HasCount(9, dependencies);

            foreach (string dependency in dependencies)
            {
                Assert.IsFalse(string.IsNullOrEmpty(dependency));
            }

            Assert.AreEqual("DLL.dll", dependencies[0]);
            Assert.AreEqual("KERNEL32.dll", dependencies[1]);
            Assert.AreEqual("VCRUNTIME140_1.dll", dependencies[2]);
            Assert.AreEqual("VCRUNTIME140.dll", dependencies[3]);
            Assert.AreEqual("api-ms-win-crt-runtime-l1-1-0.dll", dependencies[4]);
            Assert.AreEqual("api-ms-win-crt-math-l1-1-0.dll", dependencies[5]);
            Assert.AreEqual("api-ms-win-crt-stdio-l1-1-0.dll", dependencies[6]);
            Assert.AreEqual("api-ms-win-crt-locale-l1-1-0.dll", dependencies[7]);
            Assert.AreEqual("api-ms-win-crt-heap-l1-1-0.dll", dependencies[8]);
#endif
        }

        [TestMethod]
        public void SourceFiles()
        {
            IFrameworkHandle frameworkHandle = A.Fake<IFrameworkHandle>();
            Captured<TestMessageLevel> capturedTestMessageLevels = A.Captured<TestMessageLevel>();
            Captured<string> capturedTestMessages = A.Captured<string>();
            A.CallTo(() => frameworkHandle.SendMessage(capturedTestMessageLevels._, capturedTestMessages._))
               .DoesNothing();

            List<string> sourceFiles = Utilities.GetSourceFiles(TestCommon.UsingDoctestMainExecutableFilePath, TestCommon.UsingDoctestMainPdbFilePath, frameworkHandle);
            Assert.IsNotEmpty(sourceFiles);
            Assert.HasCount(1, sourceFiles);

            Assert.IsEmpty(capturedTestMessageLevels.Values);
            Assert.IsEmpty(capturedTestMessages.Values);

            string sourceFile = sourceFiles[0];
            Assert.IsFalse(string.IsNullOrEmpty(sourceFile));
            Assert.IsTrue(File.Exists(sourceFile));
            Assert.AreEqual(TestCommon.UsingDoctestMainTestHeaderFilePath, sourceFile);
        }

        [TestMethod]
        public void TestSuiteNames()
        {
            List<string> testSuiteNames = Utilities.GetAllTestSuiteNames(TestCommon.UsingDoctestMainExecutableFilePath);
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
        public void TestCaseNames()
        {
            List<string> testCaseNames = Utilities.GetAllTestCaseNames(TestCommon.UsingDoctestMainExecutableFilePath);
            Assert.IsNotEmpty(testCaseNames);
            Assert.HasCount(25, testCaseNames);
            TestCommon.AssertTestCaseNames(testCaseNames, "[UsingDoctestMain]");
        }

        [TestMethod]
        public void TestCases()
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
                TestCommon.UsingDoctestMainTestHeaderFilePath
            );
        }
    }
}
