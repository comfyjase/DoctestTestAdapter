// CustomTestCaseTemplateMacroKeywordTest.cs
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
    public class CustomTestCaseTemplateMacroKeywordTest
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
                    new CustomMacroKeyword(new List<Keyword>()
                    {
                        new DoctestTestCaseTemplateKeyword(_allTestCaseNames)
                    }, null)
                };

                TestCommon.AssertKeywords(TestCommon.UsingDoctestMainExecutableFilePath,
                    TestCommon.UsingDoctestMainTestHeaderFilePath,
                    keywords,
                    (int lineNumber, string testNamespace, List<TestCase> testCases) =>
                    {
                        if (lineNumber == 127)
                        {
                            TestCommon.AssertTestCase(testCases.Last(),
                                TestCommon.UsingDoctestMainExecutableFilePath,
                                "Empty Namespace::Empty Class::[UsingDoctestMain] Testing IsEven From Custom Test Case Template Macro In No Namespace Or Test Suite<int>",
                                "[UsingDoctestMain] Testing IsEven From Custom Test Case Template Macro In No Namespace Or Test Suite<int>",
                                TestCommon.UsingDoctestMainTestHeaderFilePath,
                                127);

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
                    new CustomMacroKeyword(new List<Keyword>()
                    {
                        new DoctestTestCaseTemplateKeyword(_allTestCaseNames)
                    }, null)
                };

                TestCommon.AssertKeywords(TestCommon.UsingDoctestMainExecutableFilePath,
                    TestCommon.UsingDoctestMainTestHeaderFilePath,
                    keywords,
                    (int lineNumber, string testNamespace, List<TestCase> testCases) =>
                    {
                        if (lineNumber == 218)
                        {
                            TestCommon.AssertTestCase(testCases.Last(),
                                TestCommon.UsingDoctestMainExecutableFilePath,
                                "[UsingDoctestMainTestSuite]::Empty Class::[UsingDoctestMain] Testing IsEven From Custom Test Case Template Macro In Test Suite<int>",
                                "[UsingDoctestMain] Testing IsEven From Custom Test Case Template Macro In Test Suite<int>",
                                TestCommon.UsingDoctestMainTestHeaderFilePath,
                                218);

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
                    new CustomMacroKeyword(new List<Keyword>()
                    {
                        new DoctestTestCaseTemplateKeyword(_allTestCaseNames)
                    }, null)
                };

                TestCommon.AssertKeywords(TestCommon.UsingDoctestMainExecutableFilePath,
                    TestCommon.UsingDoctestMainTestHeaderFilePath,
                    keywords,
                    (int lineNumber, string testNamespace, List<TestCase> testCases) =>
                    {
                        if (lineNumber == 402)
                        {
                            TestCommon.AssertTestCase(testCases.Last(),
                                TestCommon.UsingDoctestMainExecutableFilePath,
                                "UsingDoctestMainNamespace::Empty Class::[UsingDoctestMain] Testing IsEven From Custom Test Case Template Macro In Namespace<int>",
                                "[UsingDoctestMain] Testing IsEven From Custom Test Case Template Macro In Namespace<int>",
                                TestCommon.UsingDoctestMainTestHeaderFilePath,
                                402);

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
                    new CustomMacroKeyword (new List<Keyword>()
                    {
                        new DoctestTestCaseTemplateKeyword(_allTestCaseNames)
                    }, null)
                };

                TestCommon.AssertKeywords(TestCommon.UsingDoctestMainExecutableFilePath,
                    TestCommon.UsingDoctestMainTestHeaderFilePath,
                    keywords,
                    (int lineNumber, string testNamespace, List<TestCase> testCases) =>
                    {
                        if (lineNumber == 496)
                        {
                            TestCommon.AssertTestCase(testCases.Last(),
                                TestCommon.UsingDoctestMainExecutableFilePath,
                                "UsingDoctestMainNestedNamespaceOne::UsingDoctestMainNestedNamespaceTwo::Empty Class::[UsingDoctestMain] Testing IsEven From Custom Test Case Template Macro In Nested Namespace<int>",
                                "[UsingDoctestMain] Testing IsEven From Custom Test Case Template Macro In Nested Namespace<int>",
                                TestCommon.UsingDoctestMainTestHeaderFilePath,
                                496);

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
                    new CustomMacroKeyword(new List<Keyword>()
                    {
                        new DoctestTestCaseTemplateKeyword(_allTestCaseNames)
                    }, null)
                };

                TestCommon.AssertKeywords(TestCommon.UsingDoctestMainExecutableFilePath,
                    TestCommon.UsingDoctestMainTestHeaderFilePath,
                    keywords,
                    (int lineNumber, string testNamespace, List<TestCase> testCases) =>
                    {
                        if (lineNumber == 591)
                        {
                            TestCommon.AssertTestCase(testCases.Last(),
                                TestCommon.UsingDoctestMainExecutableFilePath,
                                "UsingDoctestMainNamespaceAndTestSuite_Namespace::[UsingDoctestMainNamespaceAndTestSuite_TestSuite]::Empty Class::[UsingDoctestMain] Testing IsEven From Custom Test Case Template Macro In Namespace And Test Suite<int>",
                                "[UsingDoctestMain] Testing IsEven From Custom Test Case Template Macro In Namespace And Test Suite<int>",
                                TestCommon.UsingDoctestMainTestHeaderFilePath,
                                591);

                            return true;
                        }

                        return false;
                    });
            });
        }
    }
}
