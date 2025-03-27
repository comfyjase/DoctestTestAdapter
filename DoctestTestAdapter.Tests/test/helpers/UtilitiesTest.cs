// UtilitiesTest.cs
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace DoctestTestAdapter.Tests.Helpers
{
    [TestClass]
    public class UtilitiesTest
    {
        [TestMethod]
        public void SolutionDirectory()
            => Assert.IsTrue(Utilities.GetSolutionDirectory().EndsWith("DoctestTestAdapter"));

        [TestMethod]
        public void SolutionDirectoryExe()
            => Assert.IsTrue(Utilities.GetSolutionDirectory(TestCommon.UsingDoctestMainExecutableFilePath).EndsWith("DoctestTestAdapter.Examples"));

        [TestMethod]
        public void SolutionDirectoryDLL()
            => Assert.IsTrue(Utilities.GetSolutionDirectory(TestCommon.DLLExecutableFilePath).EndsWith("DoctestTestAdapter.Examples"));

        [TestMethod]
        public void VsInstallDirectory()
            => Assert.IsTrue(Directory.Exists(Utilities.GetVSInstallDirectory()));
    }
}
