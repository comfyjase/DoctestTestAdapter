using DoctestTestAdapter.Shared.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Constants = DoctestTestAdapter.Shared.Helpers.Constants;

namespace DoctestTestAdapter.Shared.Keywords
{
    internal sealed class DoctestTestCaseKeyword : Keyword
    {
        private List<string> _allTestCaseNames = new List<string>();

        protected override string Word => "TEST_CASE";

        public DoctestTestCaseKeyword(List<string> allTestCaseNames)
        {
            _allTestCaseNames = allTestCaseNames;
        }

        protected override void OnEnterKeywordScope(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases)
        {
            // Found the keyword, but need to check this test case is compiled into the executable.
            // Can do that by checking if it's in allTestCaseNames.
            string testCaseName = _allTestCaseNames.Find(s => Regex.Unescape(line).Contains("\"" + s + "\""));
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

            TestCase testCase = Utilities.CreateTestCase(testOwner,
                testNamespace,
                testClassName,
                testCaseName,
                sourceFilePath,
                lineNumber);

            allTestCases.Add(testCase);
        }

        protected override void OnExitKeywordScope(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases)
        {
            // Don't need to do anything for this keyword implementation...
        }
    }
}
