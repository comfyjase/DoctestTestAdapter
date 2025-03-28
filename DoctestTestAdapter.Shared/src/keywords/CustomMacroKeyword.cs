// CustomMacroKeyword.cs
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
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DoctestTestAdapter.Shared.Keywords
{
    internal class CustomMacroData
    {
        private string _fullMacroDefinition = null;
        private string _doctestMacroLine = null;
        private Keyword _keyword = null;
        private List<string> _macroParameterNames = new List<string>();
        private Regex _macroRegex = null;
        private Regex _multipleSpaceRegex = new Regex(@"[ ]{2,}");

        internal Keyword Keyword
        {
            get { return _keyword; }
        }
        internal Regex MacroRegex
        {
            get { return _macroRegex; }
        }

        internal CustomMacroData(string macroName, string fullMacroDefinition, string doctestMacroLine, Keyword keyword)
        {
            _fullMacroDefinition = fullMacroDefinition;
            _doctestMacroLine = doctestMacroLine;
            _keyword = keyword;
            _macroRegex = new Regex(@"\b" + macroName + @"\b");
            
            int startIndex = _fullMacroDefinition.IndexOf(@"(") + 1;
            int endIndex = _fullMacroDefinition.LastIndexOf(@")");
            string macroParameters= _fullMacroDefinition.Substring(startIndex, endIndex - startIndex);
            _macroParameterNames = macroParameters.Split(',')
                .Select(s => s.Trim())
                .ToList();
        }

        internal string GetCustomLine(string line)
        {
            string customLine = _doctestMacroLine;

            int startIndex = line.IndexOf(@"(") + 1;
            int endIndex = line.LastIndexOf(@")");
            string macroParametersSubstring = line.Substring(startIndex, endIndex - startIndex);
            List<string> macroParameterValues = macroParametersSubstring.Split(',')
                .Select(s => s.Trim())
                .ToList();

            // Replaces the parameter names with the values.
            // E.g. TEST_CASE("[Test] " m_name) -> TEST_CASE("[Test] " "IsEven")
            for (int i = 0; i < _macroParameterNames.Count; ++i)
            {
                string parameterName = _macroParameterNames[i];
                string parameterValue = macroParameterValues[i];
                customLine = customLine.Replace(_macroParameterNames[i], macroParameterValues[i]);
            }

            startIndex = customLine.IndexOf(@"(") + 1;
            endIndex = customLine.LastIndexOf(@")");
            string customLineParameterSubstring = customLine.Substring(startIndex, endIndex - startIndex);
            List<string> customLineParameters = customLineParameterSubstring.Split(',')
                .Select(s => s.Trim())
                .ToList();

            // Checking if any string parameters need merging together.
            // E.g. TEST_CASE("[Test] " "IsEven") -> TEST_CASE("[Test] IsEven") 
            bool shouldUpdateCustomLine = false;
            for (int i = 0; i < customLineParameters.Count; ++i)
            {
                string customLineParameter = customLineParameters[i];

                // Any more than 2 double quotes per parameter in a string means we should fix up the string so it's all inside one set of double quotes.
                // Because in C++ macros will concatenate strings together, but here we read the strings directly as they are.
                // So need to fix them up and emulate what the preprocessor does since we're trying to support custom macros here.
                int numberOfDoubleQuotes = customLineParameter.Count(c => c == '"');
                if (numberOfDoubleQuotes > 2)
                {
                    customLineParameter = customLineParameter.Replace("\"", string.Empty);
                    customLineParameter = _multipleSpaceRegex.Replace(customLineParameter, " ");
                    customLineParameter = string.Format("\"{0}\"", customLineParameter);
                    customLineParameters[i] = customLineParameter;
                    shouldUpdateCustomLine = true;
                }
            }

            if (shouldUpdateCustomLine)
            {
                string newCustomLineParameters = string.Join(",", customLineParameters);
                customLine = customLine.Remove(startIndex, endIndex - startIndex).Insert(startIndex, newCustomLineParameters);
            }

            return customLine;
        }
    };

    internal class CustomMacroKeyword : IKeyword
    {
        private Regex _regexMacroDefineSearchPattern = null;
        private string _macroDefineKeyword = "#define";
        private string _currentMacroDefinition = null;
        private IMessageLogger _logger = null;

        // Done so custom macros can just reuse logic from existing keywords.
        // E.g. a macro that wraps around a TEST_CASE should just reuse the DoctestTestCaseKeyword class.
        private List<Keyword> _keywords = null;

        private List<CustomMacroData> _customMacros = new List<CustomMacroData>();

        internal CustomMacroKeyword(List<Keyword> keywords, IMessageLogger logger)
        {
            _regexMacroDefineSearchPattern = new Regex(@"^" + _macroDefineKeyword + @"\b");
            _keywords = keywords;
            _logger = logger;
        }

        private string GetMacroDefinition(string line)
        {
            string macroDefinition = null;

            // Currently only supporting macros that pass parameters through to doctest macros.
            // Doesn't seem like there would be a valid case for defining a macro and not passing any parameters through
            // E.g.
            //  #define CUSTOM_TEST_CASE_MACRO  \
            //      TEST_CASE("Some test")      \
            // ...
            // Doesn't really make much sense.
            if (!line.Contains(@"(") || !line.Contains(@")"))
            {
                return macroDefinition;
            }

            int startIndex = line.IndexOf(_macroDefineKeyword) + _macroDefineKeyword.Length + 1; // +1 for the space after #define
            int endIndex = line.LastIndexOf(@")") + 1; // +1 to include the bracket character, needed for parsing later.
            macroDefinition = line.Substring(startIndex, endIndex - startIndex);

            return macroDefinition;
        }

        private void MapMacro(string line, string macroName, Keyword keyword)
        {
            int startIndex = line.IndexOf(keyword.Word);
            int endIndex = line.LastIndexOf(@")") + 1;
            string doctestMacroLine = line.Substring(startIndex, endIndex - startIndex);

            CustomMacroData customMacroData = new CustomMacroData(macroName, _currentMacroDefinition, doctestMacroLine, keyword);
            _customMacros.Add(customMacroData);
        }

        private void CheckForKeyword(string line)
        {
            Keyword keyword = _keywords.Find(k => (k.SearchPattern.Match(line).Success || Regex.Match(line, @"\b" + k.Word + @"\b", RegexOptions.None).Success));
            if (keyword != null)
            {
                string macroName = _currentMacroDefinition.Substring(0, _currentMacroDefinition.IndexOf("("));

                if (_logger != null)
                {
                    _logger.SendMessage(TestMessageLevel.Informational, Shared.Helpers.Constants.InformationMessagePrefix + " - Found macro " + macroName + " mapped to " + keyword.Word);
                }

                // Add to the dictionary of custom macro definition -> doctest macros
                MapMacro(line, macroName, keyword);
            }

            // This is the last line of the macro definition.
            if (!line.EndsWith("\\"))
            {
                // Reset this now it's been parsed/stored.
                _currentMacroDefinition = null;
            }
        }

        private void CheckForMacroDefinition(string line)
        {
            // Found a #define to parse.
            Match keywordRegexMatch = _regexMacroDefineSearchPattern.Match(line);
            bool macroDefinitionMatch = keywordRegexMatch.Success;
            if (macroDefinitionMatch)
            {
                _currentMacroDefinition = GetMacroDefinition(line);
                CheckForKeyword(line);
            }
            // Otherwise, this isn't a match but we are looking through a macro definition now.
            else if (!string.IsNullOrEmpty(_currentMacroDefinition))
            {
                CheckForKeyword(line);
            }
        }

        private void CheckForMacroImplementation(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases)
        {
            // Should only be checking for implementation calls outside of macro definitions.
            if (string.IsNullOrEmpty(_currentMacroDefinition))
            {
                foreach (CustomMacroData customMacro in _customMacros)
                {
                    if (customMacro.MacroRegex.Match(line).Success)
                    {
                        string customLine = customMacro.GetCustomLine(line);

                        customMacro.Keyword.OnEnterKeywordScope(executableFilePath,
                            sourceFilePath,
                            ref namespaceName,
                            ref className,
                            customLine,
                            lineNumber,
                            ref allTestCases);

                        // These custom macros will only be only line, so exit straight away which is why the OnExitKeywordScope is called here.
                        customMacro.Keyword.OnExitKeywordScope(executableFilePath,
                            sourceFilePath,
                            ref namespaceName,
                            ref className,
                            customLine,
                            lineNumber,
                            ref allTestCases);
                    }
                }
            }
        }

        public void Check(string executableFilePath, string sourceFilePath, ref string namespaceName, ref string className, string line, int lineNumber, ref List<TestCase> allTestCases)
        {
            CheckForMacroDefinition(line);
            CheckForMacroImplementation(executableFilePath, sourceFilePath, ref namespaceName, ref className, line, lineNumber, ref allTestCases);
        }
    }
}
