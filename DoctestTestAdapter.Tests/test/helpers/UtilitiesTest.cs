using DoctestTestAdapter.Shared.EqualityComparers;
using DoctestTestAdapter.Shared.Helpers;
using DoctestTestAdapter.Shared.Pdb;
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
            string pdbFilePath = Utilities.GetPDBFilePath(TestCommon.UsingDoctestMainExecutableFilePath);
            Assert.IsTrue(File.Exists(pdbFilePath));
            Assert.IsTrue(Path.GetExtension(pdbFilePath).Equals(".pdb"));
            Assert.AreEqual("UsingDoctestMain.pdb", Path.GetFileName(pdbFilePath));

            pdbFilePath = Utilities.GetPDBFilePath(TestCommon.ExecutableUsingDLLExecutableFilePath);
            Assert.IsTrue(File.Exists(pdbFilePath));
            Assert.IsTrue(Path.GetExtension(pdbFilePath).Equals(".pdb"));
            Assert.AreEqual("ExecutableUsingDLL.pdb", Path.GetFileName(pdbFilePath));

            pdbFilePath = Utilities.GetPDBFilePath(TestCommon.DLLExecutableFilePath);
            Assert.IsTrue(File.Exists(pdbFilePath));
            Assert.IsTrue(Path.GetExtension(pdbFilePath).Equals(".pdb"));
            Assert.AreEqual("DLL.pdb", Path.GetFileName(pdbFilePath));
        }

        [TestMethod]
        public void Dependencies()
        {
            List<string> dependencies = Utilities.GetDependencies(TestCommon.UsingDoctestMainExecutableFilePath);

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
            List<string> sourceFiles = Utilities.GetSourceFiles(TestCommon.UsingDoctestMainExecutableFilePath, TestCommon.UsingDoctestMainPdbFilePath);
            Assert.IsTrue(sourceFiles.Count == 1);

            string sourceFile = sourceFiles[0];
            Assert.IsTrue(File.Exists(sourceFile));
            Assert.IsTrue(sourceFile.EndsWith("TestIsEvenUsingDoctestMain.h"));
        }

        [TestMethod]
        public void ReadPdbFile()
        {
            List<PdbData> allPdbData = Utilities.ReadPdbFile(TestCommon.UsingDoctestMainExecutableFilePath);
            Assert.IsTrue(allPdbData.Count > 0);

            foreach (PdbData pdbData in allPdbData)
            {
                Assert.IsTrue(pdbData.LineData.Count > 0);

                pdbData.LineData.Sort(new PdbLineDataComparer());

                // Pdb information appears slightly differently between Debug/Release (if you do generate or optimize pdb for release).
                // This doesn't matter for the actual test adapter implementation itself.
                // However, it does matter for unit testing.
#if DEBUG
                Assert.AreEqual("TestUsingDoctestMain", pdbData.LineData[3].Namespace);
                Assert.AreEqual(50, pdbData.LineData[3].Number);
                Assert.AreEqual("TEST_CASE(\"[UsingDoctestMain] Testing IsEven Always Pass\")", pdbData.LineData[3].LineStr);
                Assert.IsTrue(string.IsNullOrEmpty(pdbData.LineData[3].ClassName));

                Assert.AreEqual("TestUsingDoctestMain", pdbData.LineData[9].Namespace);
                Assert.AreEqual(57, pdbData.LineData[9].Number);
                Assert.AreEqual("TEST_CASE(\"[UsingDoctestMain] Testing IsEven Always Fail\")", pdbData.LineData[9].LineStr);
                Assert.IsTrue(string.IsNullOrEmpty(pdbData.LineData[9].ClassName));

                Assert.AreEqual("TestUsingDoctestMain", pdbData.LineData[15].Namespace);
                Assert.AreEqual(64, pdbData.LineData[15].Number);
                Assert.AreEqual("TEST_CASE(\"[UsingDoctestMain] Testing IsEven Always Skipped\" * doctest::skip())", pdbData.LineData[15].LineStr);
                Assert.IsTrue(string.IsNullOrEmpty(pdbData.LineData[15].ClassName));

                Assert.AreEqual("TestUsingDoctestMain", pdbData.LineData[21].Namespace);
                Assert.AreEqual(71, pdbData.LineData[21].Number);
                Assert.AreEqual("TEST_CASE(\"[UsingDoctestMain] Testing IsEven, Always Pass, With Commas In Name\")", pdbData.LineData[21].LineStr);
                Assert.IsTrue(string.IsNullOrEmpty(pdbData.LineData[21].ClassName));
#else
                Assert.AreEqual("TestUsingDoctestMain", pdbData.LineData[0].Namespace);
                Assert.AreEqual(50, pdbData.LineData[0].Number);
                Assert.AreEqual("TEST_CASE(\"[UsingDoctestMain] Testing IsEven Always Pass\")", pdbData.LineData[0].LineStr);
                Assert.IsTrue(string.IsNullOrEmpty(pdbData.LineData[0].ClassName));

                Assert.AreEqual("TestUsingDoctestMain", pdbData.LineData[6].Namespace);
                Assert.AreEqual(57, pdbData.LineData[6].Number);
                Assert.AreEqual("TEST_CASE(\"[UsingDoctestMain] Testing IsEven Always Fail\")", pdbData.LineData[6].LineStr);
                Assert.IsTrue(string.IsNullOrEmpty(pdbData.LineData[6].ClassName));

                Assert.AreEqual("TestUsingDoctestMain", pdbData.LineData[12].Namespace);
                Assert.AreEqual(64, pdbData.LineData[12].Number);
                Assert.AreEqual("TEST_CASE(\"[UsingDoctestMain] Testing IsEven Always Skipped\" * doctest::skip())", pdbData.LineData[12].LineStr);
                Assert.IsTrue(string.IsNullOrEmpty(pdbData.LineData[12].ClassName));

                Assert.AreEqual("TestUsingDoctestMain", pdbData.LineData[18].Namespace);
                Assert.AreEqual(71, pdbData.LineData[18].Number);
                Assert.AreEqual("TEST_CASE(\"[UsingDoctestMain] Testing IsEven, Always Pass, With Commas In Name\")", pdbData.LineData[18].LineStr);
                Assert.IsTrue(string.IsNullOrEmpty(pdbData.LineData[18].ClassName));
#endif
            }
        }

        [TestMethod]
        public void TestCaseProperty()
        {
            TestCase testCase = Utilities.CreateTestCase(TestCommon.UsingDoctestMainExecutableFilePath,
                    "TestUsingDoctestMain",
                    "Empty Class",
                    "[UsingDoctestMain] Testing IsEven Always Pass",
                    TestCommon.UsingDoctestMainTestHeaderFile,
                    50,
                    false);

            object shouldBeSkippedObject = Utilities.GetTestCasePropertyValue<object>(testCase, Constants.ShouldBeSkippedTestProperty);
            Assert.IsTrue(shouldBeSkippedObject != null);
        }

        [TestMethod]
        public void TestCaseMarkedAsSkip()
        {
            TestCase testCase = Utilities.CreateTestCase(TestCommon.UsingDoctestMainExecutableFilePath,
                    "TestUsingDoctestMain",
                    "Empty Class",
                    "[UsingDoctestMain] Testing IsEven Always Skipped",
                    TestCommon.UsingDoctestMainTestHeaderFile,
                    64,
                    true);

            bool shouldBeSkipped = Utilities.GetTestCasePropertyValue<bool>(testCase, Constants.ShouldBeSkippedTestProperty);
            Assert.IsTrue(shouldBeSkipped);
        }

        [TestMethod]
        public void TestCaseNotMarkedAsSkip()
        {
            TestCase testCase = Utilities.CreateTestCase(TestCommon.UsingDoctestMainExecutableFilePath,
                   "TestUsingDoctestMain",
                   "Empty Class",
                   "[UsingDoctestMain] Testing IsEven Always Pass",
                   TestCommon.UsingDoctestMainTestHeaderFile,
                   50,
                   false);

            bool shouldBeSkipped = Utilities.GetTestCasePropertyValue<bool>(testCase, Constants.ShouldBeSkippedTestProperty);
            Assert.IsFalse(shouldBeSkipped);
        }

        [TestMethod]
        public void TestCases()
        {
            List<TestCase> testCases = Utilities.GetTestCases(TestCommon.UsingDoctestMainExecutableFilePath);
            Assert.IsTrue(testCases.Count == 4);

            TestCase firstTestCase = testCases[0];
            TestCommon.AssertTestCase(firstTestCase,
                TestCommon.UsingDoctestMainExecutableFilePath,
                "TestUsingDoctestMain::Empty Class::[UsingDoctestMain] Testing IsEven Always Pass",
                "[UsingDoctestMain] Testing IsEven Always Pass",
                TestCommon.UsingDoctestMainTestHeaderFile,
                50);
        }
    }
}
