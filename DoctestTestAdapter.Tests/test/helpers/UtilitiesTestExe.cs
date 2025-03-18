using DoctestTestAdapter.Shared.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace DoctestTestAdapter.Tests.Helpers
{
    [TestClass]
    public class UtilitiesTestExe
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
            Assert.IsTrue(Path.GetExtension(pdbFilePath).Equals(".pdb"));
            Assert.AreEqual("UsingDoctestMain.pdb", Path.GetFileName(pdbFilePath));
            Assert.AreEqual(TestCommon.UsingDoctestMainPdbFilePath, pdbFilePath);
        }

        [TestMethod]
        public void Dependencies()
        {
            // Using ExecutableUsingDLL instead of UsingDoctestMain to make sure DLL is picked up as a dependency.
            List<string> dependencies = Utilities.GetDependencies(TestCommon.ExecutableUsingDLLExecutableFilePath);

            Assert.IsNotEmpty(dependencies);

#if DEBUG
            Assert.IsTrue(dependencies.Count == 5);

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
            Assert.IsTrue(dependencies.Count == 9);

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
            List<string> sourceFiles = Utilities.GetSourceFiles(TestCommon.UsingDoctestMainExecutableFilePath, TestCommon.UsingDoctestMainPdbFilePath);
            Assert.IsNotEmpty(sourceFiles);
            Assert.IsTrue(sourceFiles.Count == 1);

            string sourceFile = sourceFiles[0];
            Assert.IsFalse(string.IsNullOrEmpty(sourceFile));
            Assert.IsTrue(File.Exists(sourceFile));
            Assert.IsTrue(sourceFile.EndsWith("TestIsEvenUsingDoctestMain.h"));
        }

        [TestMethod]
        public void TestSuiteNames()
        {
            List<string> testSuiteNames = Utilities.GetAllTestSuiteNames(TestCommon.UsingDoctestMainExecutableFilePath);
            Assert.IsNotEmpty(testSuiteNames);
            Assert.IsTrue(testSuiteNames.Count == 2);

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
            Assert.IsTrue(testCaseNames.Count == 25);
            TestCommon.AssertTestCaseNames(testCaseNames, "[UsingDoctestMain]");
        }

        [TestMethod]
        public void TestCases()
        {
            List<TestCase> testCases = Utilities.GetTestCases(TestCommon.UsingDoctestMainExecutableFilePath);
            Assert.IsTrue(testCases.Count == 25);
            TestCommon.AssertTestCases(testCases, 
                TestCommon.UsingDoctestMainExecutableFilePath,
                "UsingDoctestMain",
                TestCommon.UsingDoctestMainTestHeaderFile
            );
        }
    }
}
