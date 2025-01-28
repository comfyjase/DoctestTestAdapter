﻿using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using VS.Common.DoctestTestAdapter;
using VS2022.DoctestTestAdapter.Settings;

namespace VS2022.DoctestTestAdapter
{
    public static class DoctestTestAdapterUtilities
    {
        private static readonly String EmptyNamespaceString = "Empty Namespace";
        private static readonly String EmptyClassString = "Empty Class";

        public static TestCase CreateTestCase(string _testOwner, string _namespace, string _className, string _testName, string _sourceFilePath, int _lineNumber)
        {
            Logger.Instance.WriteLine("Begin");

            string[] parts = new string[] { _namespace, _className, _testName };
            string fullyQualifiedName = String.Join("::", parts);

            TestCase testCase = new TestCase(fullyQualifiedName, DoctestTestAdapterConstants.ExecutorUri, _testOwner);
            testCase.DisplayName = _testName;
            testCase.CodeFilePath = _sourceFilePath;
            testCase.LineNumber = _lineNumber;

            Logger.Instance.WriteLine("End");

            return testCase;
        }

        private static string GetSubstring(string _line, int startIndex, int endIndex)
        {
            Debug.Assert(startIndex > 0, "startIndex should be above 0");
            Debug.Assert(endIndex > 0, "endIndex should be above 0");
            Debug.Assert(endIndex > startIndex, "endIndex should be more than startIndex");
            Debug.Assert(endIndex <= _line.Length, "endIndex should be less than or equal to _line.Length");

            string substring = "";

            substring = _line.Substring(startIndex, endIndex - startIndex);

            return substring;
        }

        private static string GetNamespaceSubstring(string _line)
        {
            string testFileNamespace = EmptyNamespaceString;

            string namespaceKeyword = "namespace";
            int startIndex = _line.IndexOf(namespaceKeyword) + namespaceKeyword.Length;
            
            // E.g. if the line only contains the namespace keyword and nothing else...
            // Then just count this as an empty namespace.
            if (startIndex == _line.Length)
            {
                return testFileNamespace;
            }

            int endIndex = (_line.Contains("{") ? _line.IndexOf("{") : _line.Length);
            
            string subString = GetSubstring(_line, startIndex, endIndex).Replace(" ", "");
            if (!string.IsNullOrEmpty(subString))
            {
                testFileNamespace = subString.Replace(" ", "");
            }

            return testFileNamespace;
        }

        private static string GetTestSuiteNameSubstring(string _line)
        {
            string testSuiteName = EmptyNamespaceString;

            string openBracketStartString = "(\"";
            int startIndex = _line.IndexOf(openBracketStartString) + openBracketStartString.Length;
            int endIndex = _line.LastIndexOf("\")");

            string subString = GetSubstring(_line, startIndex, endIndex);
            if (!string.IsNullOrEmpty(subString))
            {
                testSuiteName = subString;
            }

            return testSuiteName;
        }

        private static string GetClassNameSubstring(string _line)
        {
            string className = EmptyClassString;

            string classKeyword = "class ";
            int startIndex = _line.IndexOf(classKeyword) + classKeyword.Length;
            int endIndex = (_line.Contains(":") ? _line.LastIndexOf(" :") : _line.Length);

            string subString = GetSubstring(_line, startIndex, endIndex);
            if (!string.IsNullOrEmpty(subString))
            {
                className = subString.Replace(" ", "");
            }

            return className;
        }

        private static string GetTestNameSubstring(string _line)
        {
            string testName = string.Empty;

            string openBracketStartString = "(\"";
            int startIndex = _line.IndexOf(openBracketStartString) + openBracketStartString.Length;
            int endIndex = _line.LastIndexOf("\")");

            string subString = GetSubstring(_line, startIndex, endIndex);
            if (!string.IsNullOrEmpty(subString))
            {
                testName = subString;
            }

            return testName;
        }

        public static string GetTestProjectName(IRunSettings _runSettings, string _filePath)
        {
            // Updating the source of tests to use the correct output files.
            // In these cases, exe/dll files.
            //IRunSettings runSettings = _discoveryContext.RunSettings;

            // Just default to the source filepath if nothing else was found.
            string testSource = _filePath;
            
            if (_runSettings != null)
            {
                Logger.Instance.WriteLine("1) Found run settings");

                DoctestSettingsProvider doctestSettingsProvider = _runSettings.GetSettings(DoctestTestAdapterConstants.DoctestTestAdapterSettingsName) as DoctestSettingsProvider;

                if (doctestSettingsProvider != null)
                {
                    Logger.Instance.WriteLine("2) Found doctest run settings");

                    Debug.Assert(doctestSettingsProvider.OutputFileData.Count > 0, "Run settings file should have at least one OutputFile entry.");

                    foreach (DoctestSettingsOutputFileData outputFileData in doctestSettingsProvider.OutputFileData)
                    {
                        string searchString = outputFileData.ProjectFilePath;
                        //string searchString = outputFileData.FilePath;

                        string regexPattern = @"\b" + Regex.Escape(Path.GetFileNameWithoutExtension(searchString)) + @"\b";
                        if (Regex.Match(_filePath, regexPattern, RegexOptions.IgnoreCase).Success)
                        {
                            // Associate this test with this output filepath.
                            Logger.Instance.WriteLine("3) Associating test file " + Path.GetFileName(_filePath) + " with file " + Path.GetFileName(searchString));
                            return (testSource = searchString);
                        }
                        else
                        {
                            Logger.Instance.WriteLine("3) Test file " + _filePath + " didn't match regexPattern " + regexPattern + " for file " + Regex.Escape(Path.GetFileNameWithoutExtension(searchString)));
                        }
                    }
                }
            }

            return testSource;
        }

        public static List<TestCase> GetTests(IEnumerable<string> _sources, IDiscoveryContext _discoveryContext, IMessageLogger _logger, ITestCaseDiscoverySink _discoverySink)
        {
            Logger.Instance.WriteLine("Begin");

            List<TestCase> tests = new List<TestCase>();

            string currentDirectory = Directory.GetCurrentDirectory();
            Logger.Instance.WriteLine("Searching current directory: " + currentDirectory);

            foreach (string sourceFile in _sources)
            {
                if (sourceFile.Contains(currentDirectory))
                {
                    Logger.Instance.WriteLine("Searching file: " + sourceFile, 1);

                    // Executable files
                    if (Path.GetExtension(sourceFile).Equals(DoctestTestAdapterConstants.ExeFileExtension, System.StringComparison.OrdinalIgnoreCase)
                        || Path.GetExtension(sourceFile).Equals(DoctestTestAdapterConstants.DLLFileExtension, System.StringComparison.OrdinalIgnoreCase))
                    {
                        //TODO_comfyjase_25/01/2025: Check if you need to store these executables in order to run the unit tests during RunTests...
                        // Might need to use them as for creating a new Process to actually run the tests?
                    }
                    // .h/.hpp files
                    else
                    {
                        // If the file contains the TEST_CASE macro, for now just consider it a valid test case.
                        string[] allLines = File.ReadAllLines(sourceFile);
                        int currentLineNumber = 1;
                        string testFileNamespace = EmptyNamespaceString;
                        string testClassName = EmptyClassString;

                        foreach (string line in allLines)
                        {
                            if (line.Contains("namespace"))
                            {
                                testFileNamespace = GetNamespaceSubstring(line);
                            }
                            else if (line.Contains("TEST_SUITE"))
                            {
                                testFileNamespace = GetTestSuiteNameSubstring(line);
                            }
                            else if (line.Contains("class"))
                            {
                                testClassName = GetClassNameSubstring(line);
                            }
                            else if (line.Contains("TEST_CASE(\""))
                            {
                                //TODO_comfyjase_28/01/2025: Find out if there is a way to update the test owner to point to the project instead of the individual header files.
                                //string testOwner = GetTestProjectName(_discoveryContext.RunSettings, sourceFile);
                                string testOwner = sourceFile;
                                string testName = GetTestNameSubstring(line);
                                TestCase testCase = CreateTestCase(testOwner,
                                    testFileNamespace, 
                                    testClassName, 
                                    testName, 
                                    sourceFile, 
                                    currentLineNumber);
                                tests.Add(testCase);
                            }

                            ++currentLineNumber;
                        }
                    }                    
                }
            }

            

            Logger.Instance.WriteLine("Found " + tests.Count + " TestCases");
            Logger.Instance.WriteLine("End");

            return tests;
        }
    }
}
