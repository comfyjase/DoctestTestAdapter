
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DoctestTestAdapter.Shared.Helpers;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Diagnostics;

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
        public void PDBFilePath()
        {
            string exampleExecutableFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Debug\\UsingDoctestMain\\UsingDoctestMain.exe";
            string pdbFilePath = Utilities.GetPDBFilePath(exampleExecutableFilePath);
            Assert.IsTrue(File.Exists(pdbFilePath));
            Assert.IsTrue(Path.GetExtension(pdbFilePath).Equals(".pdb"));
            Assert.IsTrue(pdbFilePath.EndsWith("UsingDoctestMain.pdb"));
        }

        [TestMethod]
        public void SourceFiles()
        {
            string exampleExecutableFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Debug\\UsingDoctestMain\\UsingDoctestMain.exe";
            List<string> sourceFiles = Utilities.GetSourceFiles(exampleExecutableFilePath);
            Assert.IsTrue(sourceFiles.Count == 1);
            Assert.IsTrue(File.Exists(sourceFiles[0]));
            Assert.IsTrue(sourceFiles[0].EndsWith("TestIsEvenUsingDoctestMain.h"));
        }

        [TestMethod]
        public void TestCases()
        {
            string exampleExecutableFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Debug\\UsingDoctestMain\\UsingDoctestMain.exe";
            
            List<string> sourceFiles = Utilities.GetSourceFiles(exampleExecutableFilePath);
            Assert.IsTrue(sourceFiles.Count == 1);
            Assert.IsTrue(File.Exists(sourceFiles[0]));
            Assert.IsTrue(sourceFiles[0].EndsWith("TestIsEvenUsingDoctestMain.h"));

            List<TestCase> testCases = Utilities.GetTestCases(exampleExecutableFilePath);
            Assert.IsTrue(testCases.Count == 3);

            TestCase firstTestCase = testCases[0];
            AssertTestCase(firstTestCase, 
                exampleExecutableFilePath,
                "TestUsingDoctestMain::Empty Class::[UsingDoctestMain] Testing IsEven Always Pass",
                "[UsingDoctestMain] Testing IsEven Always Pass",
                sourceFiles[0],
                12);
        }

        private void AssertTestCase(TestCase testCase, string source, string fullyQualifiedName, string displayName, string codeFilePath, int lineNumber)
        {
            Assert.IsNotNull(testCase);
            Assert.AreEqual(source, testCase.Source);
            Assert.AreEqual(fullyQualifiedName, testCase.FullyQualifiedName);
            Assert.AreEqual(displayName, testCase.DisplayName);
            Assert.AreEqual(codeFilePath, testCase.CodeFilePath);
            Assert.AreEqual(lineNumber, testCase.LineNumber);
        }
    }
}
