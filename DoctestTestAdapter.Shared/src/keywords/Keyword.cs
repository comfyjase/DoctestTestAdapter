// Keyword.cs
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

using DoctestTestAdapter.Shared.IO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DoctestTestAdapter.Shared.Keywords
{
    internal abstract class Keyword
    {
        private BracketMatching _bracketMatching = new BracketMatching();
        private Stack<int> _relevantBracketIndexForKeywordScope = new Stack<int>();
        private bool _hasPairedBracketsForKeywordScope = false;
        private bool _isInKeywordScope = false;
        private Regex _regexSearchPattern = null;

        protected string colonSeparator = ":";
        protected string doubleColonSeparator = "::";
        protected string fullStopSeparator = ".";

        protected abstract string Word { get; }

        internal Keyword()
        {
            _regexSearchPattern = new Regex(@"(^|[\t])\b" + Word + @"\b");
            _bracketMatching.OnFoundCloseBracket += OnFoundCloseBracket;
            _bracketMatching.OnLeaveBracketScope += OnLeaveBracketScope;
        }

        private void OnLeaveBracketScope(object sender, BracketMatchingEventArgs e)
        {
            _isInKeywordScope = false;
        }

        private void OnFoundCloseBracket(object sender, BracketMatchingEventArgs e)
        {
            if (_relevantBracketIndexForKeywordScope.Count > 0)
            {
                if (_relevantBracketIndexForKeywordScope.Peek() == e.BracketNumber)
                {
                    _relevantBracketIndexForKeywordScope.Pop();
                    _hasPairedBracketsForKeywordScope = true;
                }
            }
        }

        protected abstract void OnEnterKeywordScope(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases);

        protected abstract void OnExitKeywordScope(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases);

        internal void Check(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases)
        {
            Match keywordRegexMatch = _regexSearchPattern.Match(line);
            bool validMatch = keywordRegexMatch.Success && !line.EndsWith(";");
            if (validMatch)
            {
                if (!_isInKeywordScope)
                {
                    _isInKeywordScope = true;
                }

                // Important that this is done before the _bracketMatching.Check(line).
                // Guaranteed to be entering some kind of keyword scope now.
                // So track the relevant index of what bracket we expect to open/close this keyword scope.
                _relevantBracketIndexForKeywordScope.Push(_bracketMatching.NumberOfBrackets);
                
                _bracketMatching.Check(line);
                OnEnterKeywordScope(executableFilePath, sourceFilePath, ref namespaceName, ref className, line, lineNumber, ref allTestCases);
            }
            // Even if the regex match fails, we might already be inside of a keyword scope - so check brackets to update inside state.
            else if (_isInKeywordScope)
            {
                _hasPairedBracketsForKeywordScope = false;

                int bracketNumber = _bracketMatching.Check(line);

                if (_hasPairedBracketsForKeywordScope)
                {
                    OnExitKeywordScope(executableFilePath, sourceFilePath, ref namespaceName, ref className, line, lineNumber, ref allTestCases);
                }
            }
        }
    }
}
