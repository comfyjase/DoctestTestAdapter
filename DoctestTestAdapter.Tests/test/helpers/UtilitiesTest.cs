
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DoctestTestAdapter.Shared.Helpers;
using System.IO;
using System.Collections.Generic;

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
            Assert.IsTrue(sourceFiles.Count > 0);
            Assert.IsTrue(sourceFiles.Count == 1);
            Assert.IsTrue(File.Exists(sourceFiles[0]));
            Assert.IsTrue(sourceFiles[0].EndsWith("TestIsEvenUsingDoctestMain.h"));
        }
    }
}
