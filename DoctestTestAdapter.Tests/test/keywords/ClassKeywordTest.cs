// ClassKeywordTest.cs
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DoctestTestAdapter.Tests.Keywords
{
    [TestClass]
    public class ClassKeywordTest
    {
        [TestMethod]
        public void SearchNoTests()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                List<IKeyword> keywords = new List<IKeyword>()
                {
                    new ClassKeyword()
                };

                TestCommon.AssertKeywords(TestCommon.ClassKeywordsExecutableFilePath,
                    TestCommon.ClassKeywordsHeaderFilePath,
                    keywords,
                    (int lineNumber, string testNamespace, string testClassName, List<TestCase> testCases) =>
                    {
                        if (lineNumber == 3)
                        {
                            Assert.IsFalse(string.IsNullOrEmpty(testClassName));
                            Assert.AreEqual("BaseClass", testClassName);
                        }
                        else if (lineNumber == 11)
                        {
                            Assert.IsTrue(string.IsNullOrEmpty(testClassName));
                        }
                        else if (lineNumber == 13)
                        {
                            Assert.IsFalse(string.IsNullOrEmpty(testClassName));
                            Assert.AreEqual("BaseClassWithBracketOnSameLine", testClassName);
                        }
                        else if (lineNumber == 21)
                        {
                            Assert.IsTrue(string.IsNullOrEmpty(testClassName));
                        }
                        else if (lineNumber == 23)
                        {
                            Assert.IsFalse(string.IsNullOrEmpty(testClassName));
                            Assert.AreEqual("InheritingClassWithBracketOnSameLine", testClassName);
                        }
                        else if (lineNumber == 29)
                        {
                            Assert.IsTrue(string.IsNullOrEmpty(testClassName));
                        }
                        else if (lineNumber == 31)
                        {
                            Assert.IsFalse(string.IsNullOrEmpty(testClassName));
                            Assert.AreEqual("InheritingClass", testClassName);
                        }
                        else if (lineNumber == 39)
                        {
                            Assert.IsTrue(string.IsNullOrEmpty(testClassName));
                            return true;
                        }

                        return false;
                    });
            });
        }
    }
}
