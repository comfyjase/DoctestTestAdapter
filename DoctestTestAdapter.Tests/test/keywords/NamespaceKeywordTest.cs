// NamespaceKeywordTest.cs
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
using DoctestTestAdapter.Shared.Executables;
using DoctestTestAdapter.Shared.Keywords;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoctestTestAdapter.Tests.Keywords
{
	[TestClass]
	public class NamespaceKeywordTest
	{
        [TestMethod]
        public void FindSingle()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                List<IKeyword> keywords = new List<IKeyword>()
                {
                    new NamespaceKeyword()
                };

                TestCommon.AssertKeywords(TestCommon.UsingDoctestMainExecutableFilePath,
                    TestCommon.UsingDoctestMainTestHeaderFilePath,
                    keywords,
                    (int lineNumber, string testNamespace, string testClassName, List <TestCase> testCases) =>
                    {
                        if (lineNumber == 321)
                        {
                            Assert.IsFalse(string.IsNullOrEmpty(testNamespace));
                            Assert.AreEqual("UsingDoctestMainNamespace", testNamespace);
                        }
                        else if (lineNumber == 403)
                        {
                            Assert.IsTrue(string.IsNullOrEmpty(testNamespace));
                            return true;
                        }

                        return false;
                    });
            });
        }

        [TestMethod]
        public void FindNested()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                List<IKeyword> keywords = new List<IKeyword>()
                {
                    new NamespaceKeyword()
                };

                TestCommon.AssertKeywords(TestCommon.UsingDoctestMainExecutableFilePath,
                    TestCommon.UsingDoctestMainTestHeaderFilePath,
                    keywords,
                    (int lineNumber, string testNamespace, string testClassName, List<TestCase> testCases) =>
                    {
                        if (lineNumber == 413)
                        {
                            Assert.IsFalse(string.IsNullOrEmpty(testNamespace));
                            Assert.AreEqual("UsingDoctestMainNestedNamespaceOne", testNamespace);
                        }
                        else if (lineNumber == 415)
                        {
                            Assert.IsFalse(string.IsNullOrEmpty(testNamespace));
                            Assert.AreEqual("UsingDoctestMainNestedNamespaceOne::UsingDoctestMainNestedNamespaceTwo", testNamespace);
                        }
                        else if (lineNumber == 497)
                        {
                            Assert.IsFalse(string.IsNullOrEmpty(testNamespace));
                            Assert.AreEqual("UsingDoctestMainNestedNamespaceOne", testNamespace);
                        }
                        else if (lineNumber == 498)
                        {
                            Assert.IsTrue(string.IsNullOrEmpty(testNamespace));
                            return true;
                        }

                        return false;
                    });
            });
        }

        [TestMethod]
        public void FindWithUsingNamespace()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                List<IKeyword> keywords = new List<IKeyword>()
                {
                    new NamespaceKeyword()
                };

                TestCommon.AssertKeywords(TestCommon.NamespaceKeywordsExecutableFilePath,
                    TestCommon.NamespaceKeywordsHeaderFilePath,
                    keywords,
                    (int lineNumber, string testNamespace, string testClassName, List<TestCase> testCases) =>
                    {
                        if (lineNumber == 6 || lineNumber == 8)
                        {
                            Assert.IsFalse(string.IsNullOrEmpty(testNamespace));
                            Assert.AreEqual("Test", testNamespace);
                        }
                        else if (lineNumber == 14)
                        {
                            Assert.IsTrue(string.IsNullOrEmpty(testNamespace));
                            return true;
                        }

                        return false;
                    });
            });
        }

        [TestMethod]
        public void FindWithMultipleNamespacesWithTheSameNameInTheSameFile()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                List<string> allTestSuiteNames = new DoctestExecutable(TestCommon.NamespaceKeywordsExecutableFilePath, TestCommon.ExamplesSolutionDirectory, null, null, null, null).GetTestSuiteNames();

                List<IKeyword> keywords = new List<IKeyword>()
                {
                    new NamespaceKeyword(),
                    new DoctestTestSuiteKeyword(allTestSuiteNames)
                };

                TestCommon.AssertKeywords(TestCommon.NamespaceKeywordsExecutableFilePath,
                    TestCommon.NamespaceKeywordsHeaderFilePath,
                    keywords,
                    (int lineNumber, string testNamespace, string testClassName, List<TestCase> testCases) =>
                    {
                        if (lineNumber == 6 || lineNumber == 8)
                        {
                            Assert.IsFalse(string.IsNullOrEmpty(testNamespace));
                            Assert.AreEqual("Test", testNamespace);
                        }
                        else if (lineNumber == 14)
                        {
                            Assert.IsTrue(string.IsNullOrEmpty(testNamespace));
                        }
                        else if (lineNumber == 16)
                        {
                            Assert.IsFalse(string.IsNullOrEmpty(testNamespace));
                            Assert.AreEqual("std", testNamespace);
                        }
                        else if (lineNumber == 22)
                        {
                            Assert.IsTrue(string.IsNullOrEmpty(testNamespace));
                        }
                        else if (lineNumber == 24 || lineNumber == 26)
                        {
                            Assert.IsTrue(string.IsNullOrEmpty(testNamespace));
                        }
                        else if (lineNumber == 30)
                        {
                            Assert.IsTrue(string.IsNullOrEmpty(testNamespace));
                        }
                        else if (lineNumber == 32 || lineNumber == 34)
                        {
                            Assert.IsFalse(string.IsNullOrEmpty(testNamespace));
                            Assert.AreEqual("Test", testNamespace);
                        }
                        else if (lineNumber == 40)
                        {
                            Assert.IsTrue(string.IsNullOrEmpty(testNamespace));
                        }
                        else if (lineNumber == 42)
                        {
                            Assert.IsFalse(string.IsNullOrEmpty(testNamespace));
                            Assert.AreEqual("Test", testNamespace);
                        }
                        else if (lineNumber == 44)
                        {
                            Assert.IsFalse(string.IsNullOrEmpty(testNamespace));
                            Assert.AreEqual("Test::[Multiple Namespaces With The Same Name Suite]", testNamespace);
                        }
                        else if (lineNumber == 46)
                        {
                            Assert.IsFalse(string.IsNullOrEmpty(testNamespace));
                            Assert.AreEqual("Test::[Multiple Namespaces With The Same Name Suite]", testNamespace);
                        }
                        else if (lineNumber == 52)
                        {
                            Assert.IsFalse(string.IsNullOrEmpty(testNamespace));
                            Assert.AreEqual("Test", testNamespace);
                        }
                        else if (lineNumber == 53)
                        {
                            Assert.IsTrue(string.IsNullOrEmpty(testNamespace));
                            return true;
                        }
                        
                        return false;
                    });
            });
        }
    }
}
