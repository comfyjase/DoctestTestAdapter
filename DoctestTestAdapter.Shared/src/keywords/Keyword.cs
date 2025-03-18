using DoctestTestAdapter.Shared.IO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DoctestTestAdapter.Shared.Keywords
{
    internal abstract class Keyword
    {
        private Stack<BracketMatching> _bracketMatching = new Stack<BracketMatching>();
        private bool _isInKeywordScope = false;
        private Regex _regexSearchPattern = null;

        protected string colonSeparator = ":";
        protected string doubleColonSeparator = "::";
        protected string fullStopSeparator = ".";

        protected abstract string Word { get; }

        public Keyword()
        {
            _regexSearchPattern = new Regex(@"(^|[\t])\b" + Word + @"\b");
        }

        protected virtual bool HasLeftKeywordScope()
        {
            return true;
        }

        protected abstract void OnEnterKeywordScope(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases);

        protected abstract void OnExitKeywordScope(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases);

        public void Check(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases)
        {
            Match keywordRegexMatch = _regexSearchPattern.Match(line);
            bool validMatch = keywordRegexMatch.Success && !line.EndsWith(";");
            if (validMatch)
            {
                // This is the very first occurance of this keyword.
                // Takes into account nested keywords.
                // E.g.
                //  namespace A
                //  {
                //      namespace B
                //      ...
                //  }
                if (!_isInKeywordScope)
                {
                    _isInKeywordScope = true;
                    _bracketMatching.Push(new BracketMatching(() => { _isInKeywordScope = false; }));
                }
                // Otherwise, we are already inside the keyword scope - so this has become a nested keyword
                // Don't want to exit the keyword scope when all of the brackets have been matched inside of this scope.
                // Should only exit once the most outer keyword has finished matching all brackets.
                else
                {
                    _bracketMatching.Push(new BracketMatching(null));
                }

                _bracketMatching.Peek().Check(line);
                OnEnterKeywordScope(executableFilePath, sourceFilePath, ref namespaceName, ref className, line, lineNumber, ref allTestCases);
            }
            // Even if the regex match fails, we might already be inside of a keyword scope - so check brackets to update inside state.
            else if (_isInKeywordScope)
            {
                BracketMatching currentBracketMatcher = _bracketMatching.Peek();
                currentBracketMatcher.Check(line);
                if (!currentBracketMatcher.IsInside)
                {
                    _bracketMatching.Pop();
                    OnExitKeywordScope(executableFilePath, sourceFilePath, ref namespaceName, ref className, line, lineNumber, ref allTestCases);
                }
            }
        }
    }
}
