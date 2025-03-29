// DumpBinExecutableTest.cs
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
using System.IO;

namespace DoctestTestAdapter.Tests.Executables
{
    [TestClass]
    public class DumpBinExecutableTest
    {
        private DumpBinExecutable _dumpBinExecutable = new DumpBinExecutable(TestCommon.UsingDoctestMainExecutableFilePath, TestCommon.ExamplesSolutionDirectory, null, null, null);

        [TestMethod]
        public void DependenciesExe()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                _dumpBinExecutable.SetDiscoveredExecutable(TestCommon.ExecutableUsingDLLExecutableFilePath);
                List<string> dependencies = _dumpBinExecutable.GetDependencies();
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
            });
        }

        [TestMethod]
        public void PDBFilePathExe()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                _dumpBinExecutable.SetDiscoveredExecutable(TestCommon.UsingDoctestMainExecutableFilePath);
                string pdbFilePath = _dumpBinExecutable.GetPDBFilePath();
                Assert.IsFalse(string.IsNullOrEmpty(pdbFilePath));
                Assert.IsTrue(File.Exists(pdbFilePath));
                Assert.AreEqual(TestCommon.UsingDoctestMainPdbFilePath, pdbFilePath);
            });
        }

        [TestMethod]
        public void DependenciesDLL()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                _dumpBinExecutable.SetDiscoveredExecutable(TestCommon.DLLExecutableFilePath);
                List<string> dependencies = _dumpBinExecutable.GetDependencies();
                Assert.IsNotEmpty(dependencies);

#if DEBUG
                Assert.HasCount(5, dependencies);

                foreach (string dependency in dependencies)
                {
                    Assert.IsFalse(string.IsNullOrEmpty(dependency));
                }

                Assert.AreEqual("KERNEL32.dll", dependencies[0]);
                Assert.AreEqual("MSVCP140D.dll", dependencies[1]);
                Assert.AreEqual("VCRUNTIME140D.dll", dependencies[2]);
                Assert.AreEqual("VCRUNTIME140_1D.dll", dependencies[3]);
                Assert.AreEqual("ucrtbased.dll", dependencies[4]);
#else
            Assert.HasCount(13, dependencies);

            foreach (string dependency in dependencies)
            {
                Assert.IsFalse(string.IsNullOrEmpty(dependency));
            }

            Assert.AreEqual("KERNEL32.dll", dependencies[0]);
            Assert.AreEqual("MSVCP140.dll", dependencies[1]);
            Assert.AreEqual("VCRUNTIME140_1.dll", dependencies[2]);
            Assert.AreEqual("VCRUNTIME140.dll", dependencies[3]);
            Assert.AreEqual("api-ms-win-crt-runtime-l1-1-0.dll", dependencies[4]);
            Assert.AreEqual("api-ms-win-crt-stdio-l1-1-0.dll", dependencies[5]);
            Assert.AreEqual("api-ms-win-crt-heap-l1-1-0.dll", dependencies[6]);
            Assert.AreEqual("api-ms-win-crt-utility-l1-1-0.dll", dependencies[7]);
            Assert.AreEqual("api-ms-win-crt-time-l1-1-0.dll", dependencies[8]);
            Assert.AreEqual("api-ms-win-crt-string-l1-1-0.dll", dependencies[9]);
            Assert.AreEqual("api-ms-win-crt-filesystem-l1-1-0.dll", dependencies[10]);
            Assert.AreEqual("api-ms-win-crt-convert-l1-1-0.dll", dependencies[11]);
            Assert.AreEqual("api-ms-win-crt-math-l1-1-0.dll", dependencies[12]);
#endif
            });
        }

        [TestMethod]
        public void PDBFilePathDLL()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                _dumpBinExecutable.SetDiscoveredExecutable(TestCommon.DLLExecutableFilePath);
                string pdbFilePath = _dumpBinExecutable.GetPDBFilePath();
                Assert.IsFalse(string.IsNullOrEmpty(pdbFilePath));
                Assert.IsTrue(File.Exists(pdbFilePath));
                Assert.AreEqual(TestCommon.DLLPdbFilePath, pdbFilePath);
            });
        }
    }
}
