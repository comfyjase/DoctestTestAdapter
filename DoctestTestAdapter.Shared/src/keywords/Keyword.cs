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

using DoctestTestAdapter.Shared.PatternSearcher;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DoctestTestAdapter.Shared.Keywords
{
    internal abstract class Keyword : IKeyword
    {
        private BracketSearcher _bracketSearcher = new BracketSearcher();
        private Stack<int> _relevantBracketIndexForKeywordScope = new Stack<int>();
        private bool _hasPairedBracketsForKeywordScope = false;
        private bool _isInKeywordScope = false;
        private Regex _regexSearchPattern = null;

        protected string colonSeparator = ":";
        protected string doubleColonSeparator = "::";
        protected string fullStopSeparator = ".";

        internal abstract string Word { get; }
        internal Regex SearchPattern 
        {
            get { return _regexSearchPattern; }
        }

        internal Keyword()
        {
            _regexSearchPattern = new Regex(@"(^|[\t])\b" + Word + @"\b");
            _bracketSearcher.OnFoundCloseBracket += OnFoundCloseBracket;
            _bracketSearcher.OnLeaveBracketScope += OnLeaveBracketScope;
        }

        private void OnLeaveBracketScope(object sender, BracketSearcherEventArgs e)
        {
            _isInKeywordScope = false;
        }

        private void OnFoundCloseBracket(object sender, BracketSearcherEventArgs e)
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

        internal abstract void OnEnterKeywordScope(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases);

        internal abstract void OnExitKeywordScope(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases);

        public void Check(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases, bool reachedEndOfFile)
        {
            Match keywordRegexMatch = _regexSearchPattern.Match(line);
            bool lineCheck = ((this is ClassKeyword || this is NamespaceKeyword) ? !line.Contains(";") : !line.EndsWith(";"));
            bool validMatch = keywordRegexMatch.Success && lineCheck;
            if (validMatch)
            {
                if (!_isInKeywordScope)
                {
                    _isInKeywordScope = true;
                }

                // Important that this is done before the _bracketMatching.Check(line).
                // Guaranteed to be entering some kind of keyword scope now.
                // So track the relevant index of what bracket we expect to open/close this keyword scope.
                _relevantBracketIndexForKeywordScope.Push(_bracketSearcher.NumberOfUnpairedBrackets);
                // And it's important to check for paired brackets for this scope within the one line too.
                _hasPairedBracketsForKeywordScope = false;
                _bracketSearcher.Check(line);
                OnEnterKeywordScope(executableFilePath, sourceFilePath, ref namespaceName, ref className, line, lineNumber, ref allTestCases);
                
                // In case a keyword was declared and scoped within the same line.
                // E.g. namespace Test {}
                if (_hasPairedBracketsForKeywordScope)
                {
                    OnExitKeywordScope(executableFilePath, sourceFilePath, ref namespaceName, ref className, line, lineNumber, ref allTestCases);
                }
            }
            // Even if the regex match fails, we might already be inside of a keyword scope - so check brackets to update inside state.
            else if (_isInKeywordScope)
            {
                _hasPairedBracketsForKeywordScope = false;
                _bracketSearcher.Check(line);

                // A fallback: If we have reached the end of the file and for some reason there are still unpaired brackets.
                // Which can happen if extra brackets are within #if blocks for example.
                // Then just manually clear data here for the next search.
                if (reachedEndOfFile && _bracketSearcher.NumberOfUnpairedBrackets > 0)
                {
                    namespaceName = "";
                    className = "";
                    OnExitKeywordScope(executableFilePath, sourceFilePath, ref namespaceName, ref className, line, lineNumber, ref allTestCases);
                    _bracketSearcher.Clear();
                }
                else
                {
                    if (_hasPairedBracketsForKeywordScope)
                    {
                        OnExitKeywordScope(executableFilePath, sourceFilePath, ref namespaceName, ref className, line, lineNumber, ref allTestCases);
                    }
                }
            }
        }
    }
}
