// TestCaseComparerTest.cs
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
