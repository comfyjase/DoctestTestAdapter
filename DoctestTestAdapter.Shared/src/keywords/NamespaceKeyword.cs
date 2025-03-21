// NamespaceKeyword.cs
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
    internal sealed class NamespaceKeyword : Keyword
    {
        private string _currentNamespace = string.Empty;

        protected override string Word => "namespace";

        private string GetNamespaceSubstring(string line)
        {
            string testFileNamespace = string.Empty;

            int startIndex = line.IndexOf(Word) + Word.Length;

            // E.g. if the line only contains the namespace keyword and nothing else...
            // Then just count this as an empty namespace.
            if (startIndex == line.Length)
            {
                return testFileNamespace;
            }

            int endIndex = (line.Contains("{") ? line.IndexOf("{") : line.Length);

            testFileNamespace = line.Substring(startIndex, endIndex - startIndex).Trim();

            return testFileNamespace;
        }

        protected override void OnEnterKeywordScope(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases)
        {
            _currentNamespace = GetNamespaceSubstring(line);
            if (string.IsNullOrEmpty(_currentNamespace))
            {
                return;
            }

            if (string.IsNullOrEmpty(namespaceName))
            {
                namespaceName = _currentNamespace;
            }
            else
            {
                namespaceName += (doubleColonSeparator + _currentNamespace);
            }
        }

        protected override void OnExitKeywordScope(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases)
        {
            if (string.IsNullOrEmpty(_currentNamespace))
            {
                return;
            }

            // If this was the only namespace detected, just empty the string now we have left the keyword scope.
            if (_currentNamespace == namespaceName)
            {
                namespaceName = string.Empty;
            }
            // Otherwise, we're in a nested namespace. So only remove the last occurance of this namespace from the full namespace string.
            else
            {
                int separatorIndex = namespaceName.LastIndexOf(doubleColonSeparator);
                namespaceName = namespaceName.Substring(0, separatorIndex);

                int nestedSeparatorIndex = namespaceName.LastIndexOf(doubleColonSeparator);
                if (nestedSeparatorIndex != -1)
                {
                    _currentNamespace = namespaceName.Substring(nestedSeparatorIndex, namespaceName.Length);
                }
                else
                {
                    _currentNamespace = namespaceName;
                }
            }
        }
    }
}
