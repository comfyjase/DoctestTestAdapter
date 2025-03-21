// DoctestTestSuiteKeyword.cs
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

namespace DoctestTestAdapter.Shared.Keywords
{
    internal sealed class DoctestTestSuiteKeyword : Keyword
    {
        private List<string> _allTestSuiteNames = new List<string>();
        private string _currentTestSuiteName = string.Empty;

        protected override string Word => "TEST_SUITE";

        public DoctestTestSuiteKeyword(List<string> allTestSuiteNames)
        {
            _allTestSuiteNames = allTestSuiteNames;
        }

        protected override void OnEnterKeywordScope(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases)
        {
            string testSuiteName = _allTestSuiteNames.Find(s => line.Contains("\"" + s + "\""));
            _currentTestSuiteName = testSuiteName;
            if (string.IsNullOrEmpty(_currentTestSuiteName))
            {
                return;
            }

            if (string.IsNullOrEmpty(namespaceName))
            {
                namespaceName = _currentTestSuiteName;
            }
            else
            {
                namespaceName += (doubleColonSeparator + _currentTestSuiteName);
            }
        }

        protected override void OnExitKeywordScope(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases)
        {
            if (string.IsNullOrEmpty(_currentTestSuiteName))
            {
                return;
            }

            if (_currentTestSuiteName == namespaceName)
            {
                namespaceName = string.Empty;
            }
            else
            {
                int testSuiteSubstringIndex = namespaceName.LastIndexOf(_currentTestSuiteName);
                int separatorIndex = namespaceName.LastIndexOf(doubleColonSeparator, testSuiteSubstringIndex);

                namespaceName = namespaceName.Substring(0, separatorIndex);
            }
        }
    }
}
