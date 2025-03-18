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
                    TestCommon.UsingDoctestMainTestHeaderFilePath,
                    50);
            Assert.IsNotNull(testCase);
            
            TestCase copyOfTestCase = testCase;
            Assert.IsNotNull(copyOfTestCase);

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
                    TestCommon.UsingDoctestMainTestHeaderFilePath,
                    50);
            Assert.IsNotNull(testCaseA);
            TestCase testCaseB = Utilities.CreateTestCase(TestCommon.UsingDoctestMainExecutableFilePath,
                    "TestUsingDoctestMain",
                    "Empty Class",
                    "[UsingDoctestMain] Testing IsEven Always Fail",
                    TestCommon.UsingDoctestMainTestHeaderFilePath,
                    57);
            Assert.IsNotNull(testCaseB);

            TestCaseComparer comparer = new TestCaseComparer();
            Assert.IsFalse(comparer.Equals(testCaseA, testCaseB));
        }

        [TestMethod]
        public void CompareTestCaseAgainstNull()
        {
            TestCase testCaseA = Utilities.CreateTestCase(TestCommon.UsingDoctestMainExecutableFilePath,
                    "TestUsingDoctestMain",
                    "Empty Class",
                    "[UsingDoctestMain] Testing IsEven Always Pass",
                    TestCommon.UsingDoctestMainTestHeaderFilePath,
                    50);
            Assert.IsNotNull(testCaseA);

            TestCase testCaseB = null;

            TestCaseComparer testCaseComparer = new TestCaseComparer();
            Assert.IsFalse(testCaseComparer.Equals(testCaseA, testCaseB));
        }
    }
}
