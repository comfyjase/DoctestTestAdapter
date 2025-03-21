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
            List<Keyword> keywords = new List<Keyword>()
            {
                new NamespaceKeyword()
            };

            TestCommon.AssertKeywords(TestCommon.UsingDoctestMainExecutableFilePath,
                TestCommon.UsingDoctestMainTestHeaderFilePath,
                keywords, 
                (int lineNumber, string testNamespace, List<TestCase> testCases) =>
                {
                    if (lineNumber == 244)
                    {
                        Assert.IsFalse(string.IsNullOrEmpty(testNamespace));
                        Assert.AreEqual("UsingDoctestMainNamespace", testNamespace);
                    }
                    else if (lineNumber == 310)
                    {
                        Assert.IsTrue(string.IsNullOrEmpty(testNamespace));
                        return true;
                    }

                    return false;
                });
        }

        [TestMethod]
        public void FindNested()
        {
            List<Keyword> keywords = new List<Keyword>()
            {
                new NamespaceKeyword()
            };

            TestCommon.AssertKeywords(TestCommon.UsingDoctestMainExecutableFilePath,
                TestCommon.UsingDoctestMainTestHeaderFilePath,
                keywords, 
                (int lineNumber, string testNamespace, List<TestCase> testCases) =>
                {
                    if (lineNumber == 320)
                    {
                        Assert.IsFalse(string.IsNullOrEmpty(testNamespace));
                        Assert.AreEqual("UsingDoctestMainNestedNamespaceOne", testNamespace);
                    }
                    else if (lineNumber == 322)
                    {
                        Assert.IsFalse(string.IsNullOrEmpty(testNamespace));
                        Assert.AreEqual("UsingDoctestMainNestedNamespaceOne::UsingDoctestMainNestedNamespaceTwo", testNamespace);
                    }
                    else if (lineNumber == 388)
                    {
                        Assert.IsFalse(string.IsNullOrEmpty(testNamespace));
                        Assert.AreEqual("UsingDoctestMainNestedNamespaceOne", testNamespace);
                    }
                    else if (lineNumber == 389)
                    {
                        Assert.IsTrue(string.IsNullOrEmpty(testNamespace));
                        return true;
                    }

                    return false;
                });
        }
	}
}
