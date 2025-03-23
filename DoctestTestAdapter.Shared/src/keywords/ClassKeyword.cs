// ClassKeyword.cs
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
using System;
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

            int firstSpaceAfterClassKeyword = line.IndexOf(" ");
            int lastSpaceAfterClassKeyword = line.IndexOf(" ", firstSpaceAfterClassKeyword + 1);
            int endIndex = -1;

            // Means it must be declared like so: "class Example"
            if (firstSpaceAfterClassKeyword == lastSpaceAfterClassKeyword)
            {
                endIndex = line.Length;
            }
            // Means it is declared like so: "class Example "
            else
            {
                endIndex = lastSpaceAfterClassKeyword;
            }

            if (endIndex == -1)
            {
                throw new InvalidOperationException($"ClassKeyword from {line} 'endIndex' is still '-1' which is unexpected at this point, abort!");
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
