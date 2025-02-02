using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using VS.Common.DoctestTestAdapter;
using VS2022.DoctestTestAdapter.Settings;

namespace VS2022.DoctestTestAdapter
{
    public static class DoctestTestAdapterUtilities
    {
        private static readonly string EmptyNamespaceString = "Empty Namespace";
        private static readonly string EmptyClassString = "Empty Class";

        public static TestCase CreateTestCase(string _testOwner, string _namespace, string _className, string _testName, string _sourceFilePath, int _lineNumber, bool _shouldBeSkipped)
        {
            string[] parts = new string[] { _namespace, _className, _testName };
            string fullyQualifiedName = string.Join("::", parts);

            TestCase testCase = new TestCase(fullyQualifiedName, DoctestTestAdapterConstants.ExecutorUri, _testOwner);
            testCase.DisplayName = _testName;
            testCase.CodeFilePath = _sourceFilePath;
            testCase.LineNumber = _lineNumber;

            testCase.SetPropertyValue(DoctestTestAdapterConstants.ShouldBeSkippedTestProperty, _shouldBeSkipped);
            
            if (_shouldBeSkipped)
            {
                Logger.Instance.WriteLine("Test " + _testName + " should be skipped.");
            }

            return testCase;
        }

        public static T GetTestPropertyValue<T>(TestCase _test, TestProperty _testProperty)
        {
            Debug.Assert(_test != null);
            Debug.Assert(_testProperty != null);
            object testPropertyObject = _test.GetPropertyValue(_testProperty);
            Debug.Assert(testPropertyObject != null);
            return (T)testPropertyObject;
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
            int endIndex = _line.LastIndexOf("\"");

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
            int endIndex = _line.LastIndexOf("\"");

            string subString = GetSubstring(_line, startIndex, endIndex);
            if (!string.IsNullOrEmpty(subString))
            {
                testName = subString;
            }

            return testName;
        }

        //TODO_comfyjase_02/02/2025: Update this function to use the new txt file to query executable information.
        // For DLLs just use the first executable that dumpbin provides.
        // Otherwise, if the user wants to use a specific executable that isn't the first dependency, they will have to provide a filepath in the settings per config?
        public static string GetTestFileExecutableFilePath(DoctestSettingsProvider _doctestSettings, string _filePath)
        {
            string testExecutableFilePath = string.Empty;

            string[] allDiscoveredExecutablesFilePaths = File.ReadAllLines(DoctestTestAdapterConstants.DiscoveredExecutablesInformationFilePath);

            Dictionary<string, List<string>> executableDependencies = new Dictionary<string, List<string>>();

            // Work out what dependencies are needed first.
            foreach (string executableFilePath in allDiscoveredExecutablesFilePaths)
            {
                if (Path.GetExtension(executableFilePath).Equals(DoctestTestAdapterConstants.ExeFileExtension, StringComparison.OrdinalIgnoreCase))
                {
                    // Find out what dependencies this exe has...
                    Process dumpBinProcess = new Process();
                    dumpBinProcess.EnableRaisingEvents = true;
                    dumpBinProcess.StartInfo.CreateNoWindow = true;
                    dumpBinProcess.StartInfo.UseShellExecute = false;
                    dumpBinProcess.StartInfo.RedirectStandardOutput = true;
                    dumpBinProcess.StartInfo.FileName = @"dumpbin.exe";
                    dumpBinProcess.StartInfo.Arguments = "/dependents " + executableFilePath;

                    List<string> dumpBinProcessOutput = new List<string>();
                    dumpBinProcess.OutputDataReceived += (object _sender, DataReceivedEventArgs _e) =>
                    {
                        if (_e.Data != null && _e.Data.Count() > 0)
                        {
                            Logger.Instance.WriteLine(_e.Data);
                            dumpBinProcessOutput.Add(_e.Data);
                        }
                    };

                    Logger.Instance.WriteLine("About to start dumpbin process and check dependencies for: " + Path.GetFileName(executableFilePath));
                    Debug.Assert(dumpBinProcess.Start());

                    dumpBinProcess.WaitForExit();

                    Logger.Instance.WriteLine("dumpbin process finished checking dependences for: " + Path.GetFileName(executableFilePath));

                    List<string> dependencies = allDiscoveredExecutablesFilePaths.Where(s => dumpBinProcessOutput.Contains(Path.GetFileName(s))).ToList();
                    Debug.Assert(dependencies.Count > 0);

                    Logger.Instance.WriteLine(Path.GetFileName(executableFilePath) + " dependencies: " + "\n" + dependencies.ToString());

                    executableDependencies.Add(executableFilePath, dependencies);
                }
            }

            // Now check which executable file path to use for the test run...
            foreach (string discoveredExecutableFilePath in allDiscoveredExecutablesFilePaths)
            {
                string regexPattern = @"\b" + Regex.Escape(Path.GetFileNameWithoutExtension(discoveredExecutableFilePath)) + @"\b";
                if (Regex.Match(_filePath, regexPattern, RegexOptions.IgnoreCase).Success)
                {
                    Logger.Instance.WriteLine("Associating test file " + Path.GetFileName(_filePath) + " with executable " + Path.GetFileName(discoveredExecutableFilePath));
                    testExecutableFilePath = discoveredExecutableFilePath;

                    // If it's a DLL, get the first executable which depends on it.
                    if (Path.GetExtension(testExecutableFilePath).Equals(DoctestTestAdapterConstants.DLLFileExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.Assert(false);

                        Logger.Instance.WriteLine("DLL: " + Path.GetFileName(testExecutableFilePath) + " need to find out which .exe to run tests from this project.");

                        foreach (KeyValuePair<string, List<string>> mappedDependencies in executableDependencies)
                        {
                            string executableFilePath = mappedDependencies.Key;
                            List<string> dependencies = mappedDependencies.Value;

                            // Searching dependencies for this .dll file we have.
                            bool doesExecutableDependOnThisDLL = dependencies.Any(s => s.Contains(Path.GetFileName(testExecutableFilePath)));
                            if (doesExecutableDependOnThisDLL)
                            {
                                Logger.Instance.WriteLine("Executable: " + Path.GetFileName(executableFilePath) + " depends on DLL: " + Path.GetFileName(testExecutableFilePath) + " so using that for any test runs for file: " + Path.GetFileName(_filePath));

                                // Swap from using a .dll -> .exe file so the test has something to run on
                                testExecutableFilePath = mappedDependencies.Key;
                                break;
                            }
                        }
                    }

                    break;
                }
            }

            Debug.Assert(!Path.GetExtension(testExecutableFilePath).Equals(DoctestTestAdapterConstants.DLLFileExtension, StringComparison.OrdinalIgnoreCase));
            return testExecutableFilePath;
        }

        public static List<TestCase> GetTests(IEnumerable<string> _sources, IDiscoveryContext _discoveryContext, IMessageLogger _logger, ITestCaseDiscoverySink _discoverySink)
        {
            List<TestCase> tests = new List<TestCase>();

            string currentDirectory = Directory.GetCurrentDirectory();
            Logger.Instance.WriteLine("Searching current directory: " + currentDirectory);

            VS.Common.DoctestTestAdapter.IO.File discoveredExecutableInformationFile = new VS.Common.DoctestTestAdapter.IO.File(DoctestTestAdapterConstants.DiscoveredExecutablesInformationFilePath);

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

                        string[] existingExecuteableInformation = discoveredExecutableInformationFile.ReadAllLines();
                        bool executableInformationIsAlreadyInFile = existingExecuteableInformation.Any(s => s.Equals(sourceFile, StringComparison.OrdinalIgnoreCase));
                        if (!executableInformationIsAlreadyInFile)
                        {
                            discoveredExecutableInformationFile.WriteLine(sourceFile);
                        }
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
                                
                                //TODO_comfyjase_30/01/2025: This assumes '* doctest::skip()' is on the same line as the name of the test...
                                // Would be nice to implement logic to be able to cope with '* doctest::skip()' being on a new line too
                                bool markedWithDoctestSkip = DoctestTestAdapterConstants.SkipTestKeywords.Any(s => line.Contains(s));
                                
                                TestCase testCase = CreateTestCase(testOwner,
                                    testFileNamespace, 
                                    testClassName, 
                                    testName, 
                                    sourceFile, 
                                    currentLineNumber,
                                    markedWithDoctestSkip);
                                tests.Add(testCase);
                            }

                            ++currentLineNumber;
                        }
                    }                    
                }
            }

            Logger.Instance.WriteLine("Found " + tests.Count + " TestCases");

            return tests;
        }
    }
}
