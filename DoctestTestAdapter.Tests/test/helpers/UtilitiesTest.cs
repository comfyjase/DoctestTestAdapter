using DoctestTestAdapter.Shared.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

using Constants = DoctestTestAdapter.Shared.Helpers.Constants;

namespace DoctestTestAdapter.Tests.Helpers
{
    [TestClass]
    public class UtilitiesTest
    {
        [TestMethod]
        public void SolutionDirectory()
            => Assert.IsTrue(Utilities.GetSolutionDirectory().EndsWith("DoctestTestAdapter"));

        [TestMethod]
        public void ProjectDirectory()
            => Assert.IsTrue(Utilities.GetProjectDirectory(".csproj").EndsWith("DoctestTestAdapter.Tests"));

        [TestMethod]
        public void GetVsInstallDirectory()
            => Assert.IsTrue(!string.IsNullOrEmpty(Utilities.GetVSInstallDirectory()));

        [TestMethod]
        public void PDBFilePath()
        {
            string pdbFilePath = Utilities.GetPDBFilePath(TestCommon.ExampleExecutableFilePath);
            Assert.IsTrue(File.Exists(pdbFilePath));
            Assert.IsTrue(Path.GetExtension(pdbFilePath).Equals(".pdb"));
            Assert.IsTrue(pdbFilePath.EndsWith("UsingDoctestMain.pdb"));
        }

        [TestMethod]
        public void Dependencies()
        {
            List<string> dependencies = Utilities.GetDependencies(TestCommon.ExampleExecutableFilePath);

#if DEBUG
            Assert.IsTrue(dependencies.Count == 5);
#else
            Assert.IsTrue(dependencies.Count == 14);
#endif

#if DEBUG
            Assert.AreEqual("KERNEL32.dll", dependencies[0]);
            Assert.AreEqual("MSVCP140D.dll", dependencies[1]);
            Assert.AreEqual("VCRUNTIME140D.dll", dependencies[2]);
            Assert.AreEqual("VCRUNTIME140_1D.dll", dependencies[3]);
            Assert.AreEqual("ucrtbased.dll", dependencies[4]);
#else
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
            Assert.AreEqual("api-ms-win-crt-locale-l1-1-0.dll", dependencies[13]);
#endif
        }

        [TestMethod]
        public void SourceFiles()
        {
            List<string> sourceFiles = Utilities.GetSourceFiles(TestCommon.ExampleExecutableFilePath);
            Assert.IsTrue(sourceFiles.Count == 1);

            string sourceFile = sourceFiles[0];
            Assert.IsTrue(File.Exists(sourceFile));
            Assert.IsTrue(sourceFile.EndsWith("TestIsEvenUsingDoctestMain.h"));
        }

        [TestMethod]
        public void TestCaseProperty()
        {
            List<TestCase> testCases = Utilities.GetTestCases(TestCommon.ExampleExecutableFilePath);
            Assert.IsTrue(testCases.Count == 4);

            TestCase testCase = testCases[0];
            object shouldBeSkippedObject = Utilities.GetTestCasePropertyValue<object>(testCase, Constants.ShouldBeSkippedTestProperty);
            Assert.IsTrue(shouldBeSkippedObject != null);
        }

        [TestMethod]
        public void TestCaseMarkedAsSkip()
        {
            List<TestCase> testCases = Utilities.GetTestCases(TestCommon.ExampleExecutableFilePath);
            Assert.IsTrue(testCases.Count == 4);

            TestCase testCase = testCases[2];
            bool shouldBeSkipped = Utilities.GetTestCasePropertyValue<bool>(testCase, Constants.ShouldBeSkippedTestProperty);
            Assert.IsTrue(shouldBeSkipped);
        }

        [TestMethod]
        public void TestCaseNotMarkedAsSkip()
        {
            List<TestCase> testCases = Utilities.GetTestCases(TestCommon.ExampleExecutableFilePath);
            Assert.IsTrue(testCases.Count == 4);

            TestCase testCase = testCases[0];
            bool shouldBeSkipped = Utilities.GetTestCasePropertyValue<bool>(testCase, Constants.ShouldBeSkippedTestProperty);
            Assert.IsFalse(shouldBeSkipped);
        }

        [TestMethod]
        public void TestCases()
        {            
            List<string> sourceFiles = Utilities.GetSourceFiles(TestCommon.ExampleExecutableFilePath);
            Assert.IsTrue(sourceFiles.Count == 1);

            string sourceFile = sourceFiles[0];
            Assert.IsTrue(File.Exists(sourceFile));
            Assert.IsTrue(sourceFile.EndsWith("TestIsEvenUsingDoctestMain.h"));

            List<TestCase> testCases = Utilities.GetTestCases(TestCommon.ExampleExecutableFilePath);
            Assert.IsTrue(testCases.Count == 4);

            TestCase firstTestCase = testCases[0];
            TestCommon.AssertTestCase(firstTestCase,
                TestCommon.ExampleExecutableFilePath,
                "TestUsingDoctestMain::Empty Class::[UsingDoctestMain] Testing IsEven Always Pass",
                "[UsingDoctestMain] Testing IsEven Always Pass",
                sourceFile,
                50);
        }
    }
}
