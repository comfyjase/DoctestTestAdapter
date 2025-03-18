using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;

namespace DoctestTestAdapter.Shared.Keywords
{
    internal sealed class ClassKeyword : Keyword
    {
        private string _currentClassName = string.Empty;

        protected override string Word => "class";

        private string GetClassNameSubstring(string line)
        {
            string testClassName = string.Empty;

            int startIndex = line.IndexOf(Word) + Word.Length;

            // E.g. if the line only contains the class keyword and nothing else...
            // Then just count this as an empty class.
            if (startIndex == line.Length)
            {
                return testClassName;
            }

            int endIndex = -1;

            // Need to get the string before the colon because this class inherits from something.
            if (line.Contains(@":"))
            {
                endIndex = line.IndexOf(@":");
            }
            // A class that doesn't inherit from anything.
            else
            {
                endIndex = (line.Contains(@"{") ? line.IndexOf(@"{") : line.Length);
            }

            testClassName = line.Substring(startIndex, endIndex - startIndex).Trim();

            return testClassName;
        }

        protected override void OnEnterKeywordScope(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases)
        {
            _currentClassName = GetClassNameSubstring(line);
            if (string.IsNullOrEmpty(_currentClassName))
            {
                return;
            }

            if (string.IsNullOrEmpty(className))
            {
                className = _currentClassName;
            }
            else
            {
                className += (doubleColonSeparator + _currentClassName);
            }
        }

        protected override void OnExitKeywordScope(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases)
        {
            if (string.IsNullOrEmpty(_currentClassName))
            {
                return;
            }

            // If this was the only class detected, just empty the string now we have left the keyword scope.
            if (_currentClassName == className)
            {
                className = string.Empty;
            }
            // Otherwise, we're in a nested class. So only remove the last occurance of this className from the full className string.
            else
            {
                int separatorIndex = className.LastIndexOf(doubleColonSeparator);
                className = className.Substring(0, separatorIndex);

                int nestedSeparatorIndex = className.LastIndexOf(doubleColonSeparator);
                if (nestedSeparatorIndex != -1)
                {
                    _currentClassName = className.Substring(nestedSeparatorIndex, className.Length);
                }
                else
                {
                    _currentClassName = className;
                }
            }
        }
    }
}
