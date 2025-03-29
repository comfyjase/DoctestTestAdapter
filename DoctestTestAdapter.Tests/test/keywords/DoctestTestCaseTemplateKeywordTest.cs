// DoctestTestCaseTemplateKeywordTest.cs
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

using DoctestTestAdapter.Shared.Executables;
using DoctestTestAdapter.Shared.Keywords;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace DoctestTestAdapter.Tests.Keywords
{
    [TestClass]
    public class DoctestTestCaseTemplateKeywordTest
    {
        private List<string> _allTestSuiteNames = new DoctestExecutable(TestCommon.UsingDoctestMainExecutableFilePath, TestCommon.ExamplesSolutionDirectory, null, null, null, null).GetTestSuiteNames();
        private List<string> _allTestCaseNames = new DoctestExecutable(TestCommon.UsingDoctestMainExecutableFilePath, TestCommon.ExamplesSolutionDirectory, null, null, null, null).GetTestCaseNames();

        [TestMethod]
        public void FindInNoNamespaceOrTestSuite()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                Assert.IsNotEmpty(_allTestCaseNames);
                Assert.HasCount(50, _allTestCaseNames);

                List<IKeyword> keywords = new List<IKeyword>()
                {
                    new DoctestTestCaseTemplateKeyword(_allTestCaseNames)
                };

                TestCommon.AssertKeywords(TestCommon.UsingDoctestMainExecutableFilePath,
                    TestCommon.UsingDoctestMainTestHeaderFilePath,
                    keywords,
                    (int lineNumber, string testNamespace, List<TestCase> testCases) =>
                    {
                        if (lineNumber == 118)
                        {
                            TestCommon.AssertTestCase(testCases.Last(),
                                TestCommon.UsingDoctestMainExecutableFilePath,
                                "Empty Namespace::Empty Class::[UsingDoctestMain] Testing IsEven Test Case Template In No Namespace Or Test Suite<int>",
                                "[UsingDoctestMain] Testing IsEven Test Case Template In No Namespace Or Test Suite<int>",
                                TestCommon.UsingDoctestMainTestHeaderFilePath,
                                118);

                            return true;
                        }

                        return false;
                    });
            });
        }

        [TestMethod]
        public void FindInTestSuite()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                Assert.IsNotEmpty(_allTestSuiteNames);
                Assert.HasCount(2, _allTestSuiteNames);
                Assert.IsNotEmpty(_allTestCaseNames);
                Assert.HasCount(50, _allTestCaseNames);

                List<IKeyword> keywords = new List<IKeyword>()
                {
                    new DoctestTestSuiteKeyword(_allTestSuiteNames),
                    new DoctestTestCaseTemplateKeyword(_allTestCaseNames)
                };

                TestCommon.AssertKeywords(TestCommon.UsingDoctestMainExecutableFilePath,
                    TestCommon.UsingDoctestMainTestHeaderFilePath,
                    keywords,
                    (int lineNumber, string testNamespace, List<TestCase> testCases) =>
                    {
                        if (lineNumber == 209)
                        {
                            TestCommon.AssertTestCase(testCases.Last(),
                                TestCommon.UsingDoctestMainExecutableFilePath,
                                "[UsingDoctestMainTestSuite]::Empty Class::[UsingDoctestMain] Testing IsEven Test Case Template In Test Suite<int>",
                                "[UsingDoctestMain] Testing IsEven Test Case Template In Test Suite<int>",
                                TestCommon.UsingDoctestMainTestHeaderFilePath,
                                209);

                            return true;
                        }

                        return false;
                    });
            });
        }

        [TestMethod]
        public void FindInNamespace()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                Assert.IsNotEmpty(_allTestCaseNames);
                Assert.HasCount(50, _allTestCaseNames);

                List<IKeyword> keywords = new List<IKeyword>()
                {
                    new NamespaceKeyword(),
                    new DoctestTestCaseTemplateKeyword(_allTestCaseNames)
                };

                TestCommon.AssertKeywords(TestCommon.UsingDoctestMainExecutableFilePath,
                    TestCommon.UsingDoctestMainTestHeaderFilePath,
                    keywords,
                    (int lineNumber, string testNamespace, List<TestCase> testCases) =>
                    {
                        if (lineNumber == 393)
                        {
                            TestCommon.AssertTestCase(testCases.Last(),
                                TestCommon.UsingDoctestMainExecutableFilePath,
                                "UsingDoctestMainNamespace::Empty Class::[UsingDoctestMain] Testing IsEven Test Case Template In Namespace<int>",
                                "[UsingDoctestMain] Testing IsEven Test Case Template In Namespace<int>",
                                TestCommon.UsingDoctestMainTestHeaderFilePath,
                                393);

                            return true;
                        }

                        return false;
                    });
            });
        }

        [TestMethod]
        public void FindInNestedNamespace()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                Assert.IsNotEmpty(_allTestCaseNames);
                Assert.HasCount(50, _allTestCaseNames);

                List<IKeyword> keywords = new List<IKeyword>()
                {
                    new NamespaceKeyword(),
                    new DoctestTestCaseTemplateKeyword(_allTestCaseNames)
                };

                TestCommon.AssertKeywords(TestCommon.UsingDoctestMainExecutableFilePath,
                    TestCommon.UsingDoctestMainTestHeaderFilePath,
                    keywords,
                    (int lineNumber, string testNamespace, List<TestCase> testCases) =>
                    {
                        if (lineNumber == 487)
                        {
                            TestCommon.AssertTestCase(testCases.Last(),
                                TestCommon.UsingDoctestMainExecutableFilePath,
                                "UsingDoctestMainNestedNamespaceOne::UsingDoctestMainNestedNamespaceTwo::Empty Class::[UsingDoctestMain] Testing IsEven Test Case Template In Nested Namespace<int>",
                                "[UsingDoctestMain] Testing IsEven Test Case Template In Nested Namespace<int>",
                                TestCommon.UsingDoctestMainTestHeaderFilePath,
                                487);

                            return true;
                        }

                        return false;
                    });
            });
        }

        [TestMethod]
        public void FindInNamespaceAndTestSuite()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                Assert.IsNotEmpty(_allTestSuiteNames);
                Assert.HasCount(2, _allTestSuiteNames);
                Assert.IsNotEmpty(_allTestCaseNames);
                Assert.HasCount(50, _allTestCaseNames);

                List<IKeyword> keywords = new List<IKeyword>()
                {
                    new NamespaceKeyword(),
                    new DoctestTestSuiteKeyword(_allTestSuiteNames),
                    new DoctestTestCaseTemplateKeyword(_allTestCaseNames)
                };

                TestCommon.AssertKeywords(TestCommon.UsingDoctestMainExecutableFilePath,
                    TestCommon.UsingDoctestMainTestHeaderFilePath,
                    keywords,
                    (int lineNumber, string testNamespace, List<TestCase> testCases) =>
                    {
                        if (lineNumber == 582)
                        {
                            TestCommon.AssertTestCase(testCases.Last(),
                                TestCommon.UsingDoctestMainExecutableFilePath,
                                "UsingDoctestMainNamespaceAndTestSuite_Namespace::[UsingDoctestMainNamespaceAndTestSuite_TestSuite]::Empty Class::[UsingDoctestMain] Testing IsEven Test Case Template In Namespace And Test Suite<int>",
                                "[UsingDoctestMain] Testing IsEven Test Case Template In Namespace And Test Suite<int>",
                                TestCommon.UsingDoctestMainTestHeaderFilePath,
                                582);

                            return true;
                        }

                        return false;
                    });
            });
        }
    }
}
