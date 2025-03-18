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
