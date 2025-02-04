using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using VS.Common.DoctestTestAdapter;
using VS.Common.DoctestTestAdapter.IO;

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

        //TODO_comfyjase_03/02/2025: Nice to have, way to write in the .runsettings file which exe the dll tests use.
        public static string GetTestFileExecutableFilePath(/*DoctestSettingsProvider _doctestSettings, */string _filePath)
        {
            string testExecutableFilePath = string.Empty;

            Dictionary<string, List<string>> allMappedExecutableDependencies = new Dictionary<string, List<string>>();
            XmlFile discoveredExecutablesInformationFile = new XmlFile(DoctestTestAdapterConstants.DiscoveredExecutablesInformationFilePath);
            XmlDocument discoveredExecutablesXmlDocument = discoveredExecutablesInformationFile.XmlDocument;
            XmlNodeList node = discoveredExecutablesXmlDocument.SelectNodes("//DiscoveredExecutables/ExecutableFile");

            // ExecutableFile namespace
            // E.g.
            // <ExecutableFile>
            //      ...
            // </ExecutableFile>
            foreach (XmlNode childNode in node)
            {
                XmlAttribute filePathAttribute = childNode.Attributes["FilePath"];
                if (filePathAttribute != null && !string.IsNullOrWhiteSpace(filePathAttribute.Value))
                {
                    string discoveredExecutableFilePath = filePathAttribute.Value;

                    // Dependencies namespace
                    // E.g.
                    // <Dependencies>
                    //      ...
                    // </Dependencies>
                    XmlNodeList dependencyNodes = childNode["Dependencies"].ChildNodes;
                    Debug.Assert(dependencyNodes.Count > 0);

                    foreach (XmlNode dependencyNode in dependencyNodes)
                    {
                        XmlAttribute fileNameAttribute = dependencyNode.Attributes["FileName"];
                        if (fileNameAttribute != null && !string.IsNullOrWhiteSpace(fileNameAttribute.Value))
                        {
                            string discoveredDependencyFileName = fileNameAttribute.Value;
                            Logger.Instance.WriteLine("Executable: " + Path.GetFileName(discoveredExecutableFilePath) + " depends on " + discoveredDependencyFileName);

                            if (allMappedExecutableDependencies.TryGetValue(discoveredExecutableFilePath, out List<string> dependencies))
                            {
                                dependencies.Add(discoveredDependencyFileName);
                                allMappedExecutableDependencies[discoveredExecutableFilePath] = dependencies;
                            }
                            else
                            {
                                List<string> newDependenciesList = new List<string>() { discoveredDependencyFileName };
                                allMappedExecutableDependencies.Add(discoveredExecutableFilePath, newDependenciesList);
                            }
                        }
                    }
                }
            }

            Debug.Assert(allMappedExecutableDependencies.Count > 0);

            // Now check which executable file path to use for the test run...
            foreach (KeyValuePair<string, List<string>> mappedExecutableDependencies in allMappedExecutableDependencies)
            {
                //TODO_comfyjase_03/02/2025: This will just match whichever the first executable shows up.
                // Might be nice to have the option of choosing which configuration unit tests would run in?
                // E.g. Debug by default, but user could choose Release or a combination (flags) via custom DoctestTestAdapter .runsettings or something?
                string regexPattern = @"\b" + Regex.Escape(Path.GetFileNameWithoutExtension(mappedExecutableDependencies.Key)) + @"\b";
                if (Regex.Match(_filePath, regexPattern, RegexOptions.IgnoreCase).Success)
                {
                    Logger.Instance.WriteLine("Associating test file " + Path.GetFileName(_filePath) + " with executable " + Path.GetFileName(mappedExecutableDependencies.Key));
                    testExecutableFilePath = mappedExecutableDependencies.Key;

                    // If it's a DLL, get the first executable which depends on it.
                    if (Path.GetExtension(testExecutableFilePath).Equals(DoctestTestAdapterConstants.DLLFileExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        //Debug.Assert(false);

                        Logger.Instance.WriteLine("DLL: " + Path.GetFileName(testExecutableFilePath) + " need to find out which .exe to run tests from this project.");

                        foreach (KeyValuePair<string, List<string>> mappedDependencies in allMappedExecutableDependencies)
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

        private static List<string> GetExecutableDependencies(string _sourceFile)
        {
            // Find out what dependencies this exe has...
            System.Diagnostics.Process dumpBinProcess = new System.Diagnostics.Process();
            dumpBinProcess.EnableRaisingEvents = true;
            dumpBinProcess.StartInfo.CreateNoWindow = true;
            dumpBinProcess.StartInfo.UseShellExecute = false;
            dumpBinProcess.StartInfo.RedirectStandardOutput = true;
            dumpBinProcess.StartInfo.FileName = @"dumpbin.exe";
            dumpBinProcess.StartInfo.Arguments = "/dependents " + _sourceFile;

            Logger.Instance.WriteLine("About to start dumpbin process and check dependencies for: " + Path.GetFileName(_sourceFile));
            Debug.Assert(dumpBinProcess.Start());

            string output = dumpBinProcess.StandardOutput.ReadToEnd();
            Console.WriteLine(output);

            dumpBinProcess.WaitForExit();

            string startIndexString = "Image has the following dependencies:";
            string endIndexString = "Summary";
            int startIndex = output.IndexOf(startIndexString) + startIndexString.Length;
            int endIndex = output.IndexOf(endIndexString);
            string outputSubstring = output.Substring(startIndex, endIndex - startIndex); // 

            Logger.Instance.WriteLine("dumpbin process finished checking dependences for: " + Path.GetFileName(_sourceFile));

            List<string> dependencies = outputSubstring.Split('\n').Where(s => s.Contains(DoctestTestAdapterConstants.DLLFileExtension)).Select(s => s.Trim().Replace(" ", "")).ToList();

            Logger.Instance.WriteLine(Path.GetFileName(_sourceFile) + " dependencies: " + "\n" + string.Join("\n", dependencies));

            return dependencies;
        }

        public static List<TestCase> GetTests(IEnumerable<string> _sources, IDiscoveryContext _discoveryContext, IMessageLogger _logger, ITestCaseDiscoverySink _discoverySink)
        {
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
                        VS.Common.DoctestTestAdapter.IO.XmlFile discoveredExecutableInformationFile = new VS.Common.DoctestTestAdapter.IO.XmlFile(DoctestTestAdapterConstants.DiscoveredExecutablesInformationFilePath);

                        string[] existingExecuteableInformation = discoveredExecutableInformationFile.ReadAllLines();
                        bool executableInformationIsAlreadyInFile = existingExecuteableInformation.Any(s => s.Contains(sourceFile));
                        if (executableInformationIsAlreadyInFile)
                        {
                            // TODO: Log here and test godot...
                            // Should be picking up the console exe still
                            continue;
                        }

                        List<string> dependences = GetExecutableDependencies(sourceFile);

                        string textToWrite = 
                        (
                            "\n\t<ExecutableFile FilePath=\"" + sourceFile + "\">\n"
                            + "\t\t<Dependencies>" + "\n"
                        );

                        foreach (string dependency in dependences)
                        {
                            textToWrite +=
                            (
                                "\t\t\t<Dependency FileName=\"" + dependency + "\"/>" + "\n"
                            );
                        }

                        textToWrite +=
                        (
                            "\t\t</Dependencies>" + "\n"
                            + "\t</ExecutableFile>"
                        );

                        Logger.Instance.WriteLine("About to write executable " + Path.GetFileName(sourceFile) + " information to " + Path.GetFileName(DoctestTestAdapterConstants.DiscoveredExecutablesInformationFilePath));

                        discoveredExecutableInformationFile.BatchWrite(textToWrite);
                    }
                    // .h/.hpp files
                    else
                    {
                        // If the file contains the TEST_CASE macro, for now just consider it a valid test case.
                        string[] allLines = System.IO.File.ReadAllLines(sourceFile);
                        int currentLineNumber = 1;
                        string testFileNamespace = EmptyNamespaceString;
                        string testClassName = EmptyClassString;

                        foreach (string line in allLines)
                        {
                            //TODO_comfyjase_04/02/2025: Update these checks to use Regex?
                            // Same as you do for the test exectuable regex pattern...
                            
                            int numberOfSpacesInLine = line.Count(Char.IsWhiteSpace);

                            // Regex for some specific keywords.
                            string regexPattern = @"\bnamespace\b";
                            if (Regex.Match(line, regexPattern, RegexOptions.IgnoreCase).Success && numberOfSpacesInLine < 3)
                            {
                                testFileNamespace = GetNamespaceSubstring(line);
                            }

                            regexPattern = @"\bclass\b";
                            if (Regex.Match(line, regexPattern, RegexOptions.IgnoreCase).Success && numberOfSpacesInLine < 6)
                            {
                                testClassName = GetClassNameSubstring(line);
                            }

                            // Contains is good enough for these I think...
                            if (line.Contains("TEST_SUITE(\""))
                            {
                                testFileNamespace = GetTestSuiteNameSubstring(line);
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
