// TestCaseFactory.cs
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

using DoctestTestAdapter.Settings;
using DoctestTestAdapter.Shared.Helpers;
using DoctestTestAdapter.Shared.Keywords;
using DoctestTestAdapter.Shared.Profiling;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;
using System.IO;
using DoctestTestAdapter.Shared.Executables;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace DoctestTestAdapter.Shared.Factory
{
    internal class TestCaseFactory
    {
        private string _executableFilePath = null;
        private string _executableDirectory = null;
        private string _solutionDirectory = null;
        private DoctestTestSettings _settings = null;
        private IRunContext _runContext = null;
        private IMessageLogger _logger = null;

        internal TestCaseFactory(string executableFilePath, DoctestTestSettings settings, IRunContext runContext, IMessageLogger logger)
        {
            Utilities.CheckFilePath(executableFilePath, nameof(executableFilePath));

            _executableFilePath = executableFilePath;
            _executableDirectory = Directory.GetParent(_executableFilePath).FullName;
            _solutionDirectory = Utilities.GetSolutionDirectory(_executableDirectory);
            _settings = settings;
            _runContext = runContext;
            _logger = logger;
        }

        internal static TestCase CreateTestCase(string testOwner, string testNamespace, string testClassName, string testCaseName, string sourceCodeFilePath, int lineNumber)
        {
            // Here we escape any characters used by the test explorer.
            // This makes sure to display the test case names correctly in the test explorer window.
            // Note: Can't escape the '.' character, this is used as a separator for the fully qualified name.
            // Anything with a '.' in won't be valid - this is a VS thing.
            // However, we can escape '::' separator.
            // Apparently we only need to do this for the test case name. Namespace works fine as is.
            string[] parts = new string[]
            {
                testNamespace,
                testClassName,
                testCaseName.Replace(@"::", @"\:\:")
            };

            string fullyQualifiedName = string.Join(@"::", parts);

            TestCase testCase = new TestCase(fullyQualifiedName, Helpers.Constants.ExecutorUri, testOwner);
            testCase.DisplayName = testCaseName;
            testCase.CodeFilePath = sourceCodeFilePath;
            testCase.LineNumber = lineNumber;

            return testCase;
        }

        internal List<TestCase> CreateTestCases()
        {
            List<TestCase> testCases = new List<TestCase>();

            Profiler profiler = new Profiler();
            profiler.Start();
            {
                DoctestExecutable doctestExecutable = new DoctestExecutable(_executableFilePath, _solutionDirectory, _settings, _runContext, _logger, null);
                List<string> testSuiteNames = doctestExecutable.GetTestSuiteNames();
                List<string> testCaseNames = doctestExecutable.GetTestCaseNames();

                DumpBinExecutable dumpBinExecutable = new DumpBinExecutable(_executableFilePath, _solutionDirectory, _settings, _runContext, _logger);
                string pdbFilePath = dumpBinExecutable.GetPDBFilePath();

                CVDumpExecutable cvDumpExecutable = new CVDumpExecutable(pdbFilePath, _solutionDirectory, _settings, _runContext, _logger);
                List<string> dependencies = dumpBinExecutable.GetDependencies()
                    .Where(d => File.Exists(Path.Combine(_executableDirectory, d)))
                    .Select(d => Path.Combine(_executableDirectory, d))
                    .ToList();

                // Get all of the source files
                List<string> allSourceFilePaths = cvDumpExecutable.GetSourceFiles();
                foreach (string dependency in dependencies)
                {
                    dumpBinExecutable.SetDiscoveredExecutable(dependency);
                    pdbFilePath = dumpBinExecutable.GetPDBFilePath();

                    cvDumpExecutable.SetPdbFilePath(pdbFilePath);
                    allSourceFilePaths.AddRange(cvDumpExecutable.GetSourceFiles());
                }

                string testNamespace = string.Empty;
                string testClassName = string.Empty;

                List<Keyword> keywords = new List<Keyword>()
                {
                    new NamespaceKeyword(),
                    new ClassKeyword(),
                    new DoctestTestSuiteKeyword(testSuiteNames),
                    new DoctestTestCaseKeyword(testCaseNames),
                };

                // Loop over all of the source files and read them line by line
                foreach (string sourceFilePath in allSourceFilePaths)
                {
                    string[] allLines = File.ReadAllLines(sourceFilePath);
                    int currentLineNumber = 0;

                    foreach (string line in allLines)
                    {
                        ++currentLineNumber;
                        keywords.ForEach(k => k.Check(_executableFilePath, sourceFilePath, ref testNamespace, ref testClassName, line, currentLineNumber, ref testCases));
                    }
                }
            }
            profiler.End();

            return testCases;
        }
    }
}
