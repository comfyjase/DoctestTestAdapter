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
        private string _exampleExecutableFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Debug\\UsingDoctestMain\\UsingDoctestMain.exe";

        [TestMethod]
        public void SolutionDirectory()
            => Assert.IsTrue(Utilities.GetSolutionDirectory().EndsWith("DoctestTestAdapter"));

        [TestMethod]
        public void ProjectDirectory()
            => Assert.IsTrue(Utilities.GetProjectDirectory(".csproj").EndsWith("DoctestTestAdapter.Tests"));

        [TestMethod]
        public void PDBFilePath()
        {
            string pdbFilePath = Utilities.GetPDBFilePath(_exampleExecutableFilePath);
            Assert.IsTrue(File.Exists(pdbFilePath));
            Assert.IsTrue(Path.GetExtension(pdbFilePath).Equals(".pdb"));
            Assert.IsTrue(pdbFilePath.EndsWith("UsingDoctestMain.pdb"));
        }

        [TestMethod]
        public void Dependencies()
        {
            List<string> dependencies = Utilities.GetDependencies(_exampleExecutableFilePath);
            Assert.IsTrue(dependencies.Count == 5);

            Assert.AreEqual("KERNEL32.dll", dependencies[0]);
            Assert.AreEqual("MSVCP140D.dll", dependencies[1]);
            Assert.AreEqual("VCRUNTIME140D.dll", dependencies[2]);
            Assert.AreEqual("VCRUNTIME140_1D.dll", dependencies[3]);
            Assert.AreEqual("ucrtbased.dll", dependencies[4]);
        }

        [TestMethod]
        public void SourceFiles()
        {
            List<string> sourceFiles = Utilities.GetSourceFiles(_exampleExecutableFilePath);
            Assert.IsTrue(sourceFiles.Count == 1);

            string sourceFile = sourceFiles[0];
            Assert.IsTrue(File.Exists(sourceFile));
            Assert.IsTrue(sourceFile.EndsWith("TestIsEvenUsingDoctestMain.h"));
        }

        [TestMethod]
        public void TestCaseProperty()
        {
            List<TestCase> testCases = Utilities.GetTestCases(_exampleExecutableFilePath);
            Assert.IsTrue(testCases.Count == 3);

            TestCase testCase = testCases[0];
            object shouldBeSkippedObject = Utilities.GetTestCasePropertyValue<object>(testCase, Constants.ShouldBeSkippedTestProperty);
            Assert.IsTrue(shouldBeSkippedObject != null);
        }

        [TestMethod]
        public void TestCaseMarkedAsSkip()
        {
            List<TestCase> testCases = Utilities.GetTestCases(_exampleExecutableFilePath);
            Assert.IsTrue(testCases.Count == 3);

            TestCase testCase = testCases[2];
            bool shouldBeSkipped = Utilities.GetTestCasePropertyValue<bool>(testCase, Constants.ShouldBeSkippedTestProperty);
            Assert.IsTrue(shouldBeSkipped);
        }

        [TestMethod]
        public void TestCaseNotMarkedAsSkip()
        {
            List<TestCase> testCases = Utilities.GetTestCases(_exampleExecutableFilePath);
            Assert.IsTrue(testCases.Count == 3);

            TestCase testCase = testCases[0];
            bool shouldBeSkipped = Utilities.GetTestCasePropertyValue<bool>(testCase, Constants.ShouldBeSkippedTestProperty);
            Assert.IsFalse(shouldBeSkipped);
        }

        [TestMethod]
        public void TestCases()
        {            
            List<string> sourceFiles = Utilities.GetSourceFiles(_exampleExecutableFilePath);
            Assert.IsTrue(sourceFiles.Count == 1);

            string sourceFile = sourceFiles[0];
            Assert.IsTrue(File.Exists(sourceFile));
            Assert.IsTrue(sourceFile.EndsWith("TestIsEvenUsingDoctestMain.h"));

            List<TestCase> testCases = Utilities.GetTestCases(_exampleExecutableFilePath);
            Assert.IsTrue(testCases.Count == 3);

            TestCase firstTestCase = testCases[0];
            TestCommon.AssertTestCase(firstTestCase, 
                _exampleExecutableFilePath,
                "TestUsingDoctestMain::Empty Class::[UsingDoctestMain] Testing IsEven Always Pass",
                "[UsingDoctestMain] Testing IsEven Always Pass",
                sourceFile,
                12);
        }
    }
}
