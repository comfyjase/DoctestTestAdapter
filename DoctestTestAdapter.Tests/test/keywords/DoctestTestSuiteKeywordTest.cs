// DoctestTestSuiteKeywordTest.cs
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

using DoctestTestAdapter.Shared.Keywords;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DoctestTestAdapter.Shared.Executables;

namespace DoctestTestAdapter.Tests.Keywords
{
	[TestClass]
	public class DoctestTestSuiteKeywordTest
	{
        private List<string> _allTestSuiteNames = new DoctestExecutable(TestCommon.UsingDoctestMainExecutableFilePath, TestCommon.ExamplesSolutionDirectory, null, null, null, null).GetTestSuiteNames();

        [TestMethod]
		public void FindSingle()
		{
            Assert.IsNotEmpty(_allTestSuiteNames);
            Assert.HasCount(2, _allTestSuiteNames);

            List<IKeyword> keywords = new List<IKeyword>()
            {
                new DoctestTestSuiteKeyword(_allTestSuiteNames)
            };

            TestCommon.AssertKeywords(TestCommon.UsingDoctestMainExecutableFilePath, 
                TestCommon.UsingDoctestMainTestHeaderFilePath,
                keywords, (int lineNumber, string testNamespace, List<TestCase> testCases) =>
                {
                    if (lineNumber == 137)
                    {
                        Assert.IsFalse(string.IsNullOrEmpty(testNamespace));
                        Assert.AreEqual("[UsingDoctestMainTestSuite]", testNamespace);
                    }
                    else if (lineNumber == 219)
                    {
                        Assert.IsTrue(string.IsNullOrEmpty(testNamespace));
                        return true;
                    }

                    return false;
                });
        }

        // Note, this means nested inside of a namespace. Can't nest TEST_SUITE inside of another TEST_SUITE afaik.
		[TestMethod]
		public void FindNested()
		{
            Assert.IsNotEmpty(_allTestSuiteNames);
            Assert.HasCount(2, _allTestSuiteNames);

            List<IKeyword> keywords = new List<IKeyword>()
            {
                new NamespaceKeyword(),
                new DoctestTestSuiteKeyword(_allTestSuiteNames)
            };

            TestCommon.AssertKeywords(TestCommon.UsingDoctestMainExecutableFilePath,
                TestCommon.UsingDoctestMainTestHeaderFilePath,
                keywords, (int lineNumber, string testNamespace, List<TestCase> testCases) =>
                {
                    if (lineNumber == 508)
                    {
                        Assert.IsFalse(string.IsNullOrEmpty(testNamespace));
                        Assert.AreEqual("UsingDoctestMainNamespaceAndTestSuite_Namespace", testNamespace);
                    }
                    else if (lineNumber == 510)
                    {
                        Assert.IsFalse(string.IsNullOrEmpty(testNamespace));
                        Assert.AreEqual("UsingDoctestMainNamespaceAndTestSuite_Namespace::[UsingDoctestMainNamespaceAndTestSuite_TestSuite]", testNamespace);
                    }
                    else if (lineNumber == 592)
                    {
                        Assert.IsFalse(string.IsNullOrEmpty(testNamespace));
                        Assert.AreEqual("UsingDoctestMainNamespaceAndTestSuite_Namespace", testNamespace);
                    }
                    else if (lineNumber == 593)
                    {
                        Assert.IsTrue(string.IsNullOrEmpty(testNamespace));
                        return true;
                    }

                    return false;
                });
        }
	}
}
