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
