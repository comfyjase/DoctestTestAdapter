using Microsoft.ServiceHub.Resources;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using VS.Common.DoctestTestAdapter;

namespace VS2022.DoctestTestAdapter
{
    public static class DoctestTestAdapterUtilities
    {
        public static TestCase CreateTestCase(string _testName, string _source, int _lineNumber)
        {
            Logger.Instance.WriteLine("Begin");

            TestCase testCase = new TestCase(_testName, DoctestTestAdapterConstants.ExecutorUri, _source);
            testCase.CodeFilePath = _source;
            testCase.LineNumber = _lineNumber;

            Logger.Instance.WriteLine("End");

            return testCase;
        }

        public static List<TestCase> GetTests(IEnumerable<string> _sources, IDiscoveryContext _discoveryContext, IMessageLogger _logger, ITestCaseDiscoverySink _discoverySink)
        {
            Logger.Instance.WriteLine("Begin");

            List<TestCase> tests = new List<TestCase>();

            string currentDirectory = Directory.GetCurrentDirectory();
            Logger.Instance.WriteLine("Searching current directory: " + currentDirectory);

            foreach (string source in _sources)
            {
                if (source.Contains(currentDirectory))
                {
                    Logger.Instance.WriteLine("Searching file: " + source, 1);

                    // Executable files
                    if (Path.GetExtension(source).Equals(DoctestTestAdapterConstants.ExeFileExtension, System.StringComparison.OrdinalIgnoreCase)
                        || Path.GetExtension(source).Equals(DoctestTestAdapterConstants.DLLFileExtension, System.StringComparison.OrdinalIgnoreCase))
                    {
                        //TODO_comfyjase_25/01/2025: Check if you need to store these executables in order to run the unit tests during RunTests...
                        // Might need to use them as for creating a new Process to actually run the tests?
                    }
                    // .h/.hpp files
                    else
                    {
                        // If the file contains the TEST_CASE macro, for now just consider it a valid test case.
                        string[] allLines = File.ReadAllLines(source);
                        int currentLineNumber = 1;
                        foreach (string line in allLines)
                        {
                            if (line.Contains("TEST_CASE(\""))
                            {
                                int startIndex = line.IndexOf("(\"") + 2;
                                int endIndex = line.LastIndexOf("\")");

                                Debug.Assert(startIndex > 0, "startIndex should be above 0");
                                Debug.Assert(endIndex > 0, "endIndex should be above 0");
                                Debug.Assert(endIndex > startIndex, "endIndex should be more than startIndex");

                                string testName = line.Substring(startIndex, endIndex - startIndex);

                                TestCase testCase = CreateTestCase(testName, source, currentLineNumber);
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
