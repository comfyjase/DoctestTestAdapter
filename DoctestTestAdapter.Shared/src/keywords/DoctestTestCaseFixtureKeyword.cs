// DoctestTestCaseFixtureKeyword.cs
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
    internal class DoctestTestCaseFixtureKeyword : DoctestTestCaseKeyword
    {
        private string _currentTestCaseFixtureClassName = null;

        internal override string Word => "TEST_CASE_FIXTURE";

        internal DoctestTestCaseFixtureKeyword(List<string> allTestCaseNames) : base(allTestCaseNames)
        { }

        private string GetClassName(string line)
        {
            string className = null;

            int startIndex = line.IndexOf(@"(") + 1;
            int endIndex = line.IndexOf(@",", startIndex);
            className = line.Substring(startIndex, endIndex - startIndex);

            return className;
        }

        internal override void OnEnterKeywordScope(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases)
        {
            _currentTestCaseFixtureClassName = GetClassName(line);
            if (string.IsNullOrEmpty(_currentTestCaseFixtureClassName))
            {
                return;
            }

            if (string.IsNullOrEmpty(className))
            {
                className = _currentTestCaseFixtureClassName;
            }
            else
            {
                className += (doubleColonSeparator + _currentTestCaseFixtureClassName);
            }

            base.OnEnterKeywordScope(executableFilePath, sourceFilePath, ref namespaceName, ref className, line, lineNumber, ref allTestCases);
        }

        internal override void OnExitKeywordScope(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases)
        {
            if (string.IsNullOrEmpty(_currentTestCaseFixtureClassName))
            {
                return;
            }

            if (_currentTestCaseFixtureClassName == className)
            {
                className = string.Empty;
                _currentTestCaseFixtureClassName = string.Empty;
            }

            base.OnExitKeywordScope(executableFilePath, sourceFilePath, ref namespaceName, ref className, line, lineNumber, ref allTestCases);
        }
    }
}
