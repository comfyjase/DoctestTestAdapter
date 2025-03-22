// DoctestTestCaseKeywordTest.cs
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

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DoctestTestAdapter.Shared.Keywords;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Linq;
using DoctestTestAdapter.Shared.Executables;

namespace DoctestTestAdapter.Tests.Keywords
{
    [TestClass]
	public class DoctestTestCaseKeywordTest
	{
        private List<string> _allTestSuiteNames = new DoctestExecutable(TestCommon.UsingDoctestMainExecutableFilePath, TestCommon.ExamplesSolutionDirectory, null, null, null, null).GetTestSuiteNames();
        private List<string> _allTestCaseNames = new DoctestExecutable(TestCommon.UsingDoctestMainExecutableFilePath, TestCommon.ExamplesSolutionDirectory, null, null, null, null).GetTestCaseNames();

        [TestMethod]
		public void FindInNoNamespaceOrTestSuite()
		{
            Assert.IsNotEmpty(_allTestCaseNames);
            Assert.HasCount(25, _allTestCaseNames);

			List<Keyword> keywords = new List<Keyword>()
			{
				new DoctestTestCaseKeyword(_allTestCaseNames)
			};

			TestCommon.AssertKeywords(TestCommon.UsingDoctestMainExecutableFilePath, 
				TestCommon.UsingDoctestMainTestHeaderFilePath,
				keywords, 
				(int lineNumber, string testNamespace, List<TestCase> testCases) =>
				{
					if (lineNumber == 19)
					{
						TestCommon.AssertTestCase(testCases.Last(),
							TestCommon.UsingDoctestMainExecutableFilePath,
					        "Empty Namespace::Empty Class::[UsingDoctestMain] Testing IsEven Always Pass In No Namespace Or Test Suite",
					        "[UsingDoctestMain] Testing IsEven Always Pass In No Namespace Or Test Suite",
							TestCommon.UsingDoctestMainTestHeaderFilePath,
							19);

						return true;
					}

					return false;
				});
        }

		[TestMethod]
		public void FindInTestSuite()
		{
            Assert.IsNotEmpty(_allTestSuiteNames);
            Assert.HasCount(2, _allTestSuiteNames);
            Assert.IsNotEmpty(_allTestCaseNames);
            Assert.HasCount(25, _allTestCaseNames);

            List<Keyword> keywords = new List<Keyword>()
            {
                new DoctestTestSuiteKeyword(_allTestSuiteNames),
                new DoctestTestCaseKeyword(_allTestCaseNames)
            };

            TestCommon.AssertKeywords(TestCommon.UsingDoctestMainExecutableFilePath,
                TestCommon.UsingDoctestMainTestHeaderFilePath,
                keywords,
                (int lineNumber, string testNamespace, List<TestCase> testCases) =>
                {
                    if (lineNumber == 94)
                    {
                        TestCommon.AssertTestCase(testCases.Last(),
                            TestCommon.UsingDoctestMainExecutableFilePath,
                            "[UsingDoctestMainTestSuite]::Empty Class::[UsingDoctestMain] Testing IsEven Always Pass In Test Suite",
                            "[UsingDoctestMain] Testing IsEven Always Pass In Test Suite",
                            TestCommon.UsingDoctestMainTestHeaderFilePath,
                            94);

                        return true;
                    }

                    return false;
                });
        }

        [TestMethod]
        public void FindInNamespace()
        {
            Assert.IsNotEmpty(_allTestCaseNames);
            Assert.HasCount(25, _allTestCaseNames);

            List<Keyword> keywords = new List<Keyword>()
            {
                new NamespaceKeyword(),
                new DoctestTestCaseKeyword(_allTestCaseNames)
            };

            TestCommon.AssertKeywords(TestCommon.UsingDoctestMainExecutableFilePath,
                TestCommon.UsingDoctestMainTestHeaderFilePath,
                keywords,
                (int lineNumber, string testNamespace, List<TestCase> testCases) =>
                {
                    if (lineNumber == 246)
                    {
                        TestCommon.AssertTestCase(testCases.Last(),
                            TestCommon.UsingDoctestMainExecutableFilePath,
                            "UsingDoctestMainNamespace::Empty Class::[UsingDoctestMain] Testing IsEven Always Pass In Namespace",
                            "[UsingDoctestMain] Testing IsEven Always Pass In Namespace",
                            TestCommon.UsingDoctestMainTestHeaderFilePath,
                            246);

                        return true;
                    }

                    return false;
                });
        }

        [TestMethod]
        public void FindInNestedNamespace()
        {
            Assert.IsNotEmpty(_allTestCaseNames);
            Assert.HasCount(25, _allTestCaseNames);

            List<Keyword> keywords = new List<Keyword>()
            {
                new NamespaceKeyword(),
                new DoctestTestCaseKeyword(_allTestCaseNames)
            };

            TestCommon.AssertKeywords(TestCommon.UsingDoctestMainExecutableFilePath,
                TestCommon.UsingDoctestMainTestHeaderFilePath,
                keywords,
                (int lineNumber, string testNamespace, List<TestCase> testCases) =>
                {
                    if (lineNumber == 324)
                    {
                        TestCommon.AssertTestCase(testCases.Last(),
                            TestCommon.UsingDoctestMainExecutableFilePath,
                            "UsingDoctestMainNestedNamespaceOne::UsingDoctestMainNestedNamespaceTwo::Empty Class::[UsingDoctestMain] Testing IsEven Always Pass In Nested Namespace",
                            "[UsingDoctestMain] Testing IsEven Always Pass In Nested Namespace",
                            TestCommon.UsingDoctestMainTestHeaderFilePath,
                            324);

                        return true;
                    }

                    return false;
                });
        }

        [TestMethod]
        public void FindInNamespaceAndTestSuite()
        {
            Assert.IsNotEmpty(_allTestSuiteNames);
            Assert.HasCount(2, _allTestSuiteNames);
            Assert.IsNotEmpty(_allTestCaseNames);
            Assert.HasCount(25, _allTestCaseNames);

            List<Keyword> keywords = new List<Keyword>()
            {
                new NamespaceKeyword(),
                new DoctestTestSuiteKeyword(_allTestSuiteNames),
                new DoctestTestCaseKeyword(_allTestCaseNames)
            };

            TestCommon.AssertKeywords(TestCommon.UsingDoctestMainExecutableFilePath,
                TestCommon.UsingDoctestMainTestHeaderFilePath,
                keywords,
                (int lineNumber, string testNamespace, List<TestCase> testCases) =>
                {
                    if (lineNumber == 403)
                    {
                        TestCommon.AssertTestCase(testCases.Last(),
                            TestCommon.UsingDoctestMainExecutableFilePath,
                            "UsingDoctestMainNamespaceAndTestSuite_Namespace::[UsingDoctestMainNamespaceAndTestSuite_TestSuite]::Empty Class::[UsingDoctestMain] Testing IsEven Always Pass In Namespace And Test Suite",
                            "[UsingDoctestMain] Testing IsEven Always Pass In Namespace And Test Suite",
                            TestCommon.UsingDoctestMainTestHeaderFilePath,
                            403);

                        return true;
                    }

                    return false;
                });
        }
	}
}
