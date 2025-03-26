// DoctestTestCaseKeyword.cs
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

using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DoctestTestAdapter.Shared.Factory;
using Constants = DoctestTestAdapter.Shared.Helpers.Constants;

namespace DoctestTestAdapter.Shared.Keywords
{
    internal class DoctestTestCaseKeyword : Keyword
    {
        private List<string> _allTestCaseNames = new List<string>();

        internal override string Word => "TEST_CASE";

        internal DoctestTestCaseKeyword(List<string> allTestCaseNames)
        {
            _allTestCaseNames = allTestCaseNames;
        }

        internal override void OnEnterKeywordScope(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases)
        {
            // Found the keyword, but need to check this test case is compiled into the executable.
            // Can do that by checking if it's in allTestCaseNames.
            string lineUnescaped = Regex.Unescape(line);
            string testCaseName = _allTestCaseNames.Find(s => lineUnescaped.Contains("\"" + s + "\""));
            if (string.IsNullOrEmpty(testCaseName))
            {
                return;
            }

            string testOwner = executableFilePath;

            string testNamespace = namespaceName;
            if (string.IsNullOrEmpty(testNamespace))
                testNamespace = Constants.EmptyNamespaceString;

            string testClassName = className;
            if (string.IsNullOrEmpty(testClassName))
                testClassName = Constants.EmptyClassString;

            TestCase testCase = TestCaseFactory.CreateTestCase(testOwner,
                testNamespace,
                testClassName,
                testCaseName,
                sourceFilePath,
                lineNumber);

            allTestCases.Add(testCase);
        }

        internal override void OnExitKeywordScope(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases)
        {
            // Don't need to do anything for this keyword implementation...
        }
    }
}
