// DoctestTestCaseTemplateKeyword.cs
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

using DoctestTestAdapter.Shared.Factory;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace DoctestTestAdapter.Shared.Keywords
{
    internal class DoctestTestCaseTemplateKeyword : Keyword
    {
        private List<string> _allTestCaseNames = new List<string>();

        internal override string Word => "TEST_CASE_TEMPLATE";

        internal DoctestTestCaseTemplateKeyword(List<string> allTestCaseNames)
        {
            _allTestCaseNames = allTestCaseNames;
        }

        private string GetTestCaseName(string line)
        {
            string testCaseName = null;

            int startIndex = line.IndexOf("\"") + 1;
            int endIndex = line.IndexOf("\"", startIndex);
            testCaseName = line.Substring(startIndex, endIndex - startIndex);

            return testCaseName;
        }

        private List<string> GetTemplatedTestCaseNames(string line)
        {
            List<string> templateNames = new List<string>();

            string testCaseName = GetTestCaseName(line);
            if (testCaseName == null)
            {
                return templateNames;
            }

            int endOfTestCaseNameIndex = line.IndexOf(testCaseName) + testCaseName.Length;
            int commaBeforeTemplateParamaterNameIndex = line.IndexOf(@",", endOfTestCaseNameIndex) + 1;
            int startIndex = line.IndexOf(@",", commaBeforeTemplateParamaterNameIndex) + 1; // Skip past the template parameter name and +1 for the comma again.
            int endIndex = line.LastIndexOf(@")");
            List<string> templates = line.Substring(startIndex, endIndex - startIndex).Trim()
                .Split(',')
                .ToList();
            templates.ForEach(template => 
            {
                template = template.Trim();

                // Correct any std typdefs here since the underlying value is expected when using the template.
                if (Helpers.Constants.MappedTypedefs.TryGetValue(template, out string typeDefValue))
                {
                    template = typeDefValue;
                }

                templateNames.Add(testCaseName + @"<" + template + @">");
            });

            return templateNames;
        }

        internal override void OnEnterKeywordScope(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases)
        {
            string testOwner = executableFilePath;

            string testNamespace = namespaceName;
            if (string.IsNullOrEmpty(testNamespace))
                testNamespace = Helpers.Constants.EmptyNamespaceString;

            string testClassName = className;
            if (string.IsNullOrEmpty(testClassName))
                testClassName = Helpers.Constants.EmptyClassString;

            List<string> templatedTestCaseNames = GetTemplatedTestCaseNames(line);

            foreach (string templatedTestCaseName in templatedTestCaseNames)
            {
                bool foundTemplatedTestCaseName = _allTestCaseNames.Any(s => s.Equals(templatedTestCaseName));
                if (!foundTemplatedTestCaseName)
                {
                    continue;
                }

                TestCase testCase = TestCaseFactory.CreateTestCase(testOwner,
                    testNamespace,
                    testClassName,
                    templatedTestCaseName,
                    sourceFilePath,
                    lineNumber);

                allTestCases.Add(testCase);
            }
        }

        internal override void OnExitKeywordScope(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases)
        { }
    }
}
