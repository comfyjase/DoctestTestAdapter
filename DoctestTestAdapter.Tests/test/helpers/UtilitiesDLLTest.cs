using DoctestTestAdapter.Shared.Helpers;
using FakeItEasy;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DoctestTestAdapter.Tests.Helpers
{
    [TestClass]
    public class UtilitiesDLLTest
    {
        [TestMethod]
        public void SolutionDirectory()
            => Assert.IsTrue(Utilities.GetSolutionDirectory(Directory.GetParent(TestCommon.DLLExecutableFilePath).FullName).EndsWith("DoctestTestAdapter.Examples"));

        [TestMethod]
        public void PDBFilePath()
        {
            string pdbFilePath = Utilities.GetPDBFilePath(TestCommon.DLLExecutableFilePath);
            Assert.IsFalse(string.IsNullOrEmpty(pdbFilePath));
            Assert.IsTrue(File.Exists(pdbFilePath));
            Assert.AreEqual(TestCommon.DLLPdbFilePath, pdbFilePath);
        }

        [TestMethod]
        public void Dependencies()
        {
            List<string> dependencies = Utilities.GetDependencies(TestCommon.DLLExecutableFilePath);
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
        }

        [TestMethod]
        public void SourceFiles()
        {
            IFrameworkHandle frameworkHandle = A.Fake<IFrameworkHandle>();
            Captured<TestMessageLevel> capturedTestMessageLevels = A.Captured<TestMessageLevel>();
            Captured<string> capturedTestMessages = A.Captured<string>();
            A.CallTo(() => frameworkHandle.SendMessage(capturedTestMessageLevels._, capturedTestMessages._))
               .DoesNothing();

            List<string> sourceFiles = Utilities.GetSourceFiles(TestCommon.DLLExecutableFilePath, TestCommon.DLLPdbFilePath, frameworkHandle);
            Assert.IsNotEmpty(sourceFiles);
            Assert.HasCount(1, sourceFiles);

            Assert.IsEmpty(capturedTestMessageLevels.Values);
            Assert.IsEmpty(capturedTestMessages.Values);

            string sourceFile = sourceFiles[0];
            Assert.IsFalse(string.IsNullOrEmpty(sourceFile));
            Assert.IsTrue(File.Exists(sourceFile));
            Assert.AreEqual(TestCommon.DLLTestHeaderFilePath, sourceFile);
        }

        [TestMethod]
        public void TestSuiteNames()
        {
            // Note, you have to use the exe that loads the DLL, can't just use the DLL file like the other DLL tests do.
            // This is because we need to be able to _run_ a process off of this executable to get the test suite names.
            // You can't just run a DLL file.
            List<string> testSuiteNames = Utilities.GetAllTestSuiteNames(TestCommon.ExecutableUsingDLLExecutableFilePath);
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
        public void TestCaseNames()
        {
            // Same as the TestSuiteNamesDLL unit test - can't just run a DLL file, so use the exe that loads the DLL.
            List<string> testCaseNames = Utilities.GetAllTestCaseNames(TestCommon.ExecutableUsingDLLExecutableFilePath);
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

        [TestMethod]
        public void TestCases()
        {
            IFrameworkHandle frameworkHandle = A.Fake<IFrameworkHandle>();
            Captured<TestMessageLevel> capturedTestMessageLevels = A.Captured<TestMessageLevel>();
            Captured<string> capturedTestMessages = A.Captured<string>();
            A.CallTo(() => frameworkHandle.SendMessage(capturedTestMessageLevels._, capturedTestMessages._))
               .DoesNothing();

            List<TestCase> testCases = Utilities.GetTestCases(TestCommon.ExecutableUsingDLLExecutableFilePath, frameworkHandle);
            Assert.HasCount(50, testCases);

            Assert.IsEmpty(capturedTestMessageLevels.Values);
            Assert.IsEmpty(capturedTestMessages.Values);

            List<TestCase> dllTestCases = testCases
                .Where(t => t.DisplayName.Contains("[DLL]"))
                .ToList();
            List<TestCase> executableUsingDLLTestCases = testCases
                .Where(t => t.DisplayName.Contains("[ExecutableUsingDLL]"))
                .ToList();

            TestCommon.AssertTestCases(dllTestCases,
                TestCommon.ExecutableUsingDLLExecutableFilePath,
                "DLL",
                TestCommon.DLLTestHeaderFilePath
            );
            TestCommon.AssertTestCases(executableUsingDLLTestCases,
                TestCommon.ExecutableUsingDLLExecutableFilePath,
                "ExecutableUsingDLL",
                TestCommon.ExecutableUsingDLLTestHeaderFilePath
            );
        }
    }
}
