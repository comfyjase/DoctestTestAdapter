// CVDumpExectableTest.cs
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
using FakeItEasy;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace DoctestTestAdapter.Tests.Executables
{
    [TestClass]
    public class CVDumpExecutableTest
    {
        private CVDumpExecutable _cvDumpExecutable = new CVDumpExecutable(TestCommon.UsingDoctestMainPdbFilePath, TestCommon.ExamplesSolutionDirectory, null, null, null);

        [TestMethod]
        public void SourceFilesExe()
        {
            IFrameworkHandle frameworkHandle = A.Fake<IFrameworkHandle>();
            Captured<TestMessageLevel> capturedTestMessageLevels = A.Captured<TestMessageLevel>();
            Captured<string> capturedTestMessages = A.Captured<string>();
            A.CallTo(() => frameworkHandle.SendMessage(capturedTestMessageLevels._, capturedTestMessages._))
               .DoesNothing();

            _cvDumpExecutable.SetPdbFilePath(TestCommon.UsingDoctestMainPdbFilePath);
            List<string> sourceFiles = _cvDumpExecutable.GetSourceFiles(); 
            Assert.IsNotNull(sourceFiles);
            Assert.HasCount(1, sourceFiles);

            Assert.IsEmpty(capturedTestMessageLevels.Values);
            Assert.IsEmpty(capturedTestMessages.Values);

            string sourceFile = sourceFiles[0];
            Assert.IsFalse(string.IsNullOrEmpty(sourceFile));
            Assert.IsTrue(File.Exists(sourceFile));
            Assert.AreEqual(TestCommon.UsingDoctestMainTestHeaderFilePath, sourceFile);
        }

        [TestMethod]
        public void SourceFilesDLL()
        {
            IFrameworkHandle frameworkHandle = A.Fake<IFrameworkHandle>();
            Captured<TestMessageLevel> capturedTestMessageLevels = A.Captured<TestMessageLevel>();
            Captured<string> capturedTestMessages = A.Captured<string>();
            A.CallTo(() => frameworkHandle.SendMessage(capturedTestMessageLevels._, capturedTestMessages._))
               .DoesNothing();

            _cvDumpExecutable.SetPdbFilePath(TestCommon.DLLPdbFilePath);
            List<string> sourceFiles = _cvDumpExecutable.GetSourceFiles();
            Assert.IsNotNull(sourceFiles);
            Assert.HasCount(1, sourceFiles);

            Assert.IsEmpty(capturedTestMessageLevels.Values);
            Assert.IsEmpty(capturedTestMessages.Values);

            string sourceFile = sourceFiles[0];
            Assert.IsFalse(string.IsNullOrEmpty(sourceFile));
            Assert.IsTrue(File.Exists(sourceFile));
            Assert.AreEqual(TestCommon.DLLTestHeaderFilePath, sourceFile);
        }
    }
}
