﻿using DoctestTestAdapter.Shared.Keywords;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DoctestTestAdapter.Shared.Helpers;

namespace DoctestTestAdapter.Tests.Keywords
{
	[TestClass]
	public class DoctestTestSuiteKeywordTest
	{
        private List<string> _allTestSuiteNames = Utilities.GetAllTestSuiteNames(TestCommon.UsingDoctestMainExecutableFilePath);

        [TestMethod]
		public void FindSingle()
		{
            Assert.IsNotEmpty(_allTestSuiteNames);
            Assert.HasCount(2, _allTestSuiteNames);

            List<Keyword> keywords = new List<Keyword>()
            {
                new DoctestTestSuiteKeyword(_allTestSuiteNames)
            };

            TestCommon.AssertKeywords(TestCommon.UsingDoctestMainExecutableFilePath, 
                TestCommon.UsingDoctestMainTestHeaderFilePath,
                keywords, (int lineNumber, string testNamespace, List<TestCase> testCases) =>
                {
                    if (lineNumber == 92)
                    {
                        Assert.IsFalse(string.IsNullOrEmpty(testNamespace));
                        Assert.AreEqual("[UsingDoctestMainTestSuite]", testNamespace);
                    }
                    else if (lineNumber == 158)
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

            List<Keyword> keywords = new List<Keyword>()
            {
                new NamespaceKeyword(),
                new DoctestTestSuiteKeyword(_allTestSuiteNames)
            };

            TestCommon.AssertKeywords(TestCommon.UsingDoctestMainExecutableFilePath,
                TestCommon.UsingDoctestMainTestHeaderFilePath,
                keywords, (int lineNumber, string testNamespace, List<TestCase> testCases) =>
                {
                    if (lineNumber == 399)
                    {
                        Assert.IsFalse(string.IsNullOrEmpty(testNamespace));
                        Assert.AreEqual("UsingDoctestMainNamespaceAndTestSuite_Namespace", testNamespace);
                    }
                    else if (lineNumber == 401)
                    {
                        Assert.IsFalse(string.IsNullOrEmpty(testNamespace));
                        Assert.AreEqual("UsingDoctestMainNamespaceAndTestSuite_Namespace::[UsingDoctestMainNamespaceAndTestSuite_TestSuite]", testNamespace);
                    }
                    else if (lineNumber == 467)
                    {
                        Assert.IsFalse(string.IsNullOrEmpty(testNamespace));
                        Assert.AreEqual("UsingDoctestMainNamespaceAndTestSuite_Namespace", testNamespace);
                    }
                    else if (lineNumber == 468)
                    {
                        Assert.IsTrue(string.IsNullOrEmpty(testNamespace));
                        return true;
                    }

                    return false;
                });
        }
	}
}
