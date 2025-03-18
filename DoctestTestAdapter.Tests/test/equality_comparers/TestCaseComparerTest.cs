using DoctestTestAdapter.Shared.EqualityComparers;
using DoctestTestAdapter.Shared.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoctestTestAdapter.Tests.EqualityComparers
{
    [TestClass]
    public class TestCaseComparerTest
    {
        [TestMethod]
        public void CompareSameTestCases()
        {
            TestCase testCase = Utilities.CreateTestCase(TestCommon.UsingDoctestMainExecutableFilePath,
                    "TestUsingDoctestMain",
                    "Empty Class",
                    "[UsingDoctestMain] Testing IsEven Always Pass",
                    TestCommon.UsingDoctestMainTestHeaderFile,
                    50);
            TestCase copyOfTestCase = testCase;

            TestCaseComparer comparer = new TestCaseComparer();
            Assert.IsTrue(comparer.Equals(testCase, copyOfTestCase));
        }

        [TestMethod]
        public void CompareDifferentTestCases()
        {
            TestCase testCaseA = Utilities.CreateTestCase(TestCommon.UsingDoctestMainExecutableFilePath,
                    "TestUsingDoctestMain",
                    "Empty Class",
                    "[UsingDoctestMain] Testing IsEven Always Pass",
                    TestCommon.UsingDoctestMainTestHeaderFile,
                    50);
            TestCase testCaseB = Utilities.CreateTestCase(TestCommon.UsingDoctestMainExecutableFilePath,
                    "TestUsingDoctestMain",
                    "Empty Class",
                    "[UsingDoctestMain] Testing IsEven Always Fail",
                    TestCommon.UsingDoctestMainTestHeaderFile,
                    57);

            TestCaseComparer comparer = new TestCaseComparer();
            Assert.IsFalse(comparer.Equals(testCaseA, testCaseB));
        }
    }
}
