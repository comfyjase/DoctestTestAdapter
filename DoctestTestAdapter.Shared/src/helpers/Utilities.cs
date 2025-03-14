using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using System.Reflection;
using DoctestTestAdapter.Settings;
using Microsoft.Win32;
using System.Globalization;
using DoctestTestAdapter.Shared.Pdb;
using DoctestTestAdapter.Shared.Profiling;

namespace DoctestTestAdapter.Shared.Helpers
{
    internal static class Utilities
    {
        private static readonly Regex whitespace = new Regex(@"\s+");

        internal static string GetSolutionDirectory(string startingDirectoryPath = null)
        {
            if (startingDirectoryPath == null)
            {
                startingDirectoryPath = Environment.CurrentDirectory;
            }

            DirectoryInfo directory = new DirectoryInfo(startingDirectoryPath);

            Profiler profiler = new Profiler();
            profiler.Start();
            {
                while (directory != null && !directory.EnumerateFiles("*.sln").Any())
                    directory = directory.Parent;
            }
            profiler.End();

            return directory?.FullName ?? throw new FileNotFoundException($"Could not find solution directory {directory}");
        }

        internal static string GetProjectDirectory(string projectFileType, string startingDirectoryPath = null)
        {
            if (startingDirectoryPath == null)
            {
                startingDirectoryPath = Environment.CurrentDirectory;
            }

            DirectoryInfo directory = new DirectoryInfo(startingDirectoryPath);
            
            Profiler profiler = new Profiler();
            profiler.Start();
            {
                while (directory != null && !directory.EnumerateFiles("*.sln").Any() && !directory.EnumerateFiles("*" + projectFileType).Any())
                    directory = directory.Parent;
            }
            profiler.End();

            return directory?.FullName ?? throw new FileNotFoundException($"Could not find project directory {directory}");
        }
        
        /// <summary>
        /// Gets the general install directory for any of the supported and installed visual studio versions.
        /// </summary>
        /// <returns>string - Full directory path for the vs install directory.</returns>
        public static string GetVSInstallDirectory()
        {
            string vsInstallDirectory = string.Empty;

            Profiler profiler = new Profiler();
            profiler.Start();
            {
                List<string> foundInstances = Directory.GetDirectories("C:\\ProgramData\\Microsoft\\VisualStudio\\Packages\\_Instances\\").ToList();

                foreach (string instance in foundInstances)
                {
                    string directoryName = Path.GetFileName(instance);

                    string subKeyName = string.Format(
                        CultureInfo.InvariantCulture,
                        @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{0}\",
                        directoryName);

                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(subKeyName))
                    {
                        if (key != null)
                        {
                            string displayName = (string)key.GetValue("DisplayName");

                            bool foundInstalledVersion = Helpers.Constants.SupportedVisualStudioVersionNames.Any(s => displayName.Equals(s));
                            if (!foundInstalledVersion)
                                continue;

                            // This will find whatever the first valid VS install location is and then base the directory from that.
                            vsInstallDirectory = (string)key.GetValue("InstallLocation");
                            break;
                        }
                    }
                }
            }
            profiler.End();

            return vsInstallDirectory;
        }

        internal static string GetPDBFilePath(string executableFilePath)
        {
            string pdbFilePath = null;

            Profiler profiler = new Profiler();
            profiler.Start();
            {
                string batFilePath = "\"" + GetVSInstallDirectory() + "\\Common7\\Tools\\VsDevCmd.bat\"";

                System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo();
                processStartInfo.CreateNoWindow = true;
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.FileName = @"cmd.exe";
                processStartInfo.Arguments = "/c call " + batFilePath + " & dumpbin /PDBPATH " + "\"" + executableFilePath + "\"";

                System.Diagnostics.Process dumpBinProcess = new System.Diagnostics.Process();
                dumpBinProcess.StartInfo = processStartInfo;
                dumpBinProcess.Start();

                //TODO_comfyjase_25/02/2025: Wrap this in an option for the user to toggle on/off debug test output?
                string output = dumpBinProcess.StandardOutput.ReadToEnd();
                //if (!string.IsNullOrEmpty(output))
                //    Console.WriteLine("dumpbin output: \n" + output);
                string errors = dumpBinProcess.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(errors))
                    Console.WriteLine("dumpbin errors: \n\t" + errors);
                dumpBinProcess.WaitForExit();

                string startStr = "PDB file found at \'";
                int startIndex = output.IndexOf(startStr) + startStr.Length;
                int endIndex = output.LastIndexOf("\'");

                pdbFilePath = output.Substring(startIndex, endIndex - startIndex);
            }
            profiler.End();

            return pdbFilePath ?? throw new FileNotFoundException($"Could not find pdb file for executable file {executableFilePath}");
        }

        internal static List<string> GetDependencies(string executableFilePath)
        {
            List<string> dependencies = new List<string>();

            Profiler profiler = new Profiler();
            profiler.Start();
            {
                string batFilePath = "\"" + GetVSInstallDirectory() + "\\Common7\\Tools\\VsDevCmd.bat\"";

                System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo();
                processStartInfo.CreateNoWindow = true;
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.FileName = @"cmd.exe";
                processStartInfo.Arguments = "/c call " + batFilePath + " & dumpbin /dependents " + "\"" + executableFilePath + "\"";

                System.Diagnostics.Process dumpBinProcess = new System.Diagnostics.Process();
                dumpBinProcess.StartInfo = processStartInfo;
                dumpBinProcess.Start();

                //TODO_comfyjase_25/02/2025: Wrap this in an option for the user to toggle on/off debug test output?
                string output = dumpBinProcess.StandardOutput.ReadToEnd();
                //if (!string.IsNullOrEmpty(output))
                //    Console.WriteLine("dumpbin output: \n" + output);
                string errors = dumpBinProcess.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(errors))
                    Console.WriteLine("dumpbin errors: \n\t" + errors);
                dumpBinProcess.WaitForExit();

                string startIndexString = "Image has the following dependencies:";
                string endIndexString = "Summary";
                int startIndex = output.IndexOf(startIndexString) + startIndexString.Length;
                int endIndex = output.IndexOf(endIndexString);
                string outputSubstring = output.Substring(startIndex, endIndex - startIndex);

                dependencies = outputSubstring.Split('\n')
                    .Where(s => s.Contains(".dll"))
                    .Select(s => s.Trim().Replace(" ", ""))
                    .ToList();
            }
            profiler.End();

            return dependencies;
        }

        internal static List<string> GetSourceFiles(string executableFilePath, string pdbFilePath, DoctestTestSettings settings = null)
        {
            List<string> sourceFiles = new List<string>();

            Profiler profiler = new Profiler();
            profiler.Start();
            {
                string solutionDirectory = GetSolutionDirectory(Directory.GetParent(executableFilePath).FullName);
                string cvDumpFilePath = Directory.GetParent(Assembly.GetExecutingAssembly().CodeBase.Replace(@"file:///", string.Empty)) + "\\thirdparty\\cvdump\\cvdump.exe";

                System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo();
                processStartInfo.CreateNoWindow = true;
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.FileName = string.Format(@"""{0}""", cvDumpFilePath);
                processStartInfo.Arguments = "-stringtable " + string.Format(@"""{0}""", pdbFilePath);

                System.Diagnostics.Process cvDumpProcess = new System.Diagnostics.Process();
                cvDumpProcess.StartInfo = processStartInfo;
                cvDumpProcess.Start();

                //TODO_comfyjase_25/02/2025: Wrap this in an option for the user to toggle on/off debug test output?
                string output = cvDumpProcess.StandardOutput.ReadToEnd();
                //if (!string.IsNullOrEmpty(output))
                //    Console.WriteLine("cvdumpbin output: \n" + output);
                string errors = cvDumpProcess.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(errors))
                    Console.WriteLine("cvdumpbin errors: \n\t" + errors);

                cvDumpProcess.WaitForExit();

                string startStr = "STRINGTABLE";
                int startIndex = output.IndexOf(startStr) + startStr.Length;
                int endIndex = output.Length;
                string stringTableStr = output.Substring(startIndex, endIndex - startIndex);

                // User has given specific search directories to use, so make sure we only return source files from those directories.
                if (settings != null && settings.DiscoverySettings != null && settings.DiscoverySettings.SearchDirectories.Count > 0)
                {
                    sourceFiles.AddRange
                    (
                        stringTableStr.Split('\n')
                            .Select(s => s.Replace("\n", string.Empty).Replace("\r", string.Empty).Substring(s.IndexOf(" ") + 1))
                            .Where(s => (settings.DiscoverySettings.SearchDirectories.Any(sd => s.Contains(solutionDirectory + "\\" + sd + "\\")) && !s.Contains("doctest.h") && s.EndsWith(".h") && File.Exists(s)))
                            .ToList()
                    );
                }
                // Otherwise, just grab any relevant source file under the solution directory.
                else
                {
                    sourceFiles.AddRange
                    (
                        stringTableStr.Split('\n')
                            .Select(s => s.Replace("\n", string.Empty).Replace("\r", string.Empty).Substring(s.IndexOf(" ") + 1))
                            .Where(s => (s.Contains(solutionDirectory) && !s.Contains("doctest.h") && s.EndsWith(".h") && File.Exists(s)))
                            .ToList()
                    );
                }
            }
            profiler.End();

            return sourceFiles.Distinct().ToList();
        }

        internal static string GetSymbolLineForFunction(string symbolOutput, string functionAddress, int startSearchIndex = 0)
        {
            Profiler profiler = new Profiler();
            profiler.Start();
            {
                string searchString = ":" + functionAddress + "]";
                int searchIndex = symbolOutput.IndexOf(searchString, startSearchIndex);

                if (searchIndex != -1)
                {
                    int symbolDataStartIndex = searchIndex + searchString.Length;
                    int symbolLineStartIndex = symbolOutput.LastIndexOf("(", symbolDataStartIndex);
                    int symbolDataEndIndex = symbolOutput.IndexOf('\n', symbolDataStartIndex);

                    string symbolLine = symbolOutput.Substring(symbolLineStartIndex, symbolDataEndIndex - symbolLineStartIndex);

                    //TODO_comfyjase_14/03/2025: Do we need to add DOCTEST_ANON_ as part of the symbol line as well to ensure it's only doctest stuff?
                    //E.g. if ((symbolLine.Contains("S_LPROC32") || symbolLine.Contains("S_GPROC32")) && symbolLine.Contains("DOCTEST_ANON_"))
                    if (symbolLine.Contains("S_LPROC32") || symbolLine.Contains("S_GPROC32"))
                    {
                        profiler.End();

                        return symbolLine;
                    }

                    // If we have reached the final result in this string, we haven't been able to find a function symbol for this address.
                    if (startSearchIndex == symbolLineStartIndex)
                    {
                        profiler.End();

                        return null;
                    }

                    // Recursively search until we do find S_LPROC32 or S_GPROC32 with the given address.
                    return GetSymbolLineForFunction(symbolOutput, functionAddress, symbolDataEndIndex);
                }
            }
            profiler.End();

            return null;
        }

        internal static List<PdbData> ReadPdbFile(string executableFilePath, List<string> dependencies = null, DoctestTestSettings settings = null)
        {
            List<PdbData> pdbData = new List<PdbData>();

            Profiler profiler = new Profiler();
            profiler.Start();
            {
                if (dependencies != null)
                {
                    foreach (string dependency in dependencies)
                    {
                        pdbData.AddRange(ReadPdbFile(dependency, null, settings));
                    }
                }

                string pdbFilePath = GetPDBFilePath(executableFilePath);
                string solutionDirectory = GetSolutionDirectory(Directory.GetParent(executableFilePath).FullName);
                string cvDumpFilePath = Directory.GetParent(Assembly.GetExecutingAssembly().CodeBase.Replace(@"file:///", string.Empty)) + "\\thirdparty\\cvdump\\cvdump.exe";

                List<string> sourceFiles = GetSourceFiles(executableFilePath, pdbFilePath, settings);

                System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo();
                processStartInfo.CreateNoWindow = true;
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.FileName = string.Format(@"""{0}""", cvDumpFilePath);
                processStartInfo.Arguments = "-l -s " + string.Format(@"""{0}""", pdbFilePath);

                System.Diagnostics.Process cvDumpProcess = new System.Diagnostics.Process();
                cvDumpProcess.StartInfo = processStartInfo;
                cvDumpProcess.Start();

                //TODO_comfyjase_25/02/2025: Wrap this in an option for the user to toggle on/off debug test output?
                string output = cvDumpProcess.StandardOutput.ReadToEnd();
                //if (!string.IsNullOrEmpty(output))
                //    Console.WriteLine("cvdumpbin output: \n" + output);
                string errors = cvDumpProcess.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(errors))
                    Console.WriteLine("cvdumpbin errors: \n\t" + errors);

                cvDumpProcess.WaitForExit();

                // Module data output
                string startStr = "*** SYMBOLS";
                int startIndex = output.IndexOf(startStr) + startStr.Length;
                int endIndex = output.IndexOf("*** LINES");
                string symbolOutput = output.Substring(startIndex, endIndex - startIndex);

                // Line data output
                startStr = "*** LINES";
                startIndex = output.IndexOf(startStr) + startStr.Length;
                endIndex = output.Length;
                string lineDataOutput = output.Substring(startIndex, endIndex - startIndex);
                string[] lineDataOutputArr = lineDataOutput
                    .Trim()
                    .Split('\n')
                    .Where(s => (!s.Equals("\r") && !string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s)))
                    .ToArray();

                PdbData currentPdbData = null;
                for (int i = 0; i < lineDataOutputArr.Length; i++)
                {
                    string lineData = lineDataOutputArr[i];

                    if (!sourceFiles.Any(s => lineData.Contains(s)))
                    {
                        continue;
                    }

                    string sourceFile = sourceFiles.Single(s => lineData.Contains(s));

                    currentPdbData = pdbData.Find(p => p.CodeFilePath.Equals(sourceFile));
                    if (currentPdbData == null)
                    {
                        currentPdbData = new PdbData(sourceFile);
                        pdbData.Add(currentPdbData);
                    }

                    string lineAddrPairsStr = "line/addr pairs = ";
                    startIndex = lineData.IndexOf(lineAddrPairsStr) + lineAddrPairsStr.Length;
                    endIndex = lineData.Length;
                    string subString = lineData.Substring(startIndex, endIndex - startIndex);

                    if (int.TryParse(subString, out int lineAddrPairs))
                    {
                        int numberOfLinesAddressPairsAreOn = (int)Math.Ceiling((decimal)lineAddrPairs / 4);
                        for (int j = 1; j < (numberOfLinesAddressPairsAreOn + 1); ++j)
                        {
                            string[] lineNumberAndAddress = lineDataOutputArr[i + j]
                                .Trim()
                                .Split(' ')
                                .Where(s => (!string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s)))
                                .ToArray();

                            for (int k = 0; k < lineNumberAndAddress.Count(); k += 2)
                            {
                                string lineNumberStr = lineNumberAndAddress[k];
                                string lineAddressStr = lineNumberAndAddress[k + 1];

                                if (int.TryParse(lineNumberStr, out int lineNumber))
                                {
                                    PdbLineData currentLineData = currentPdbData.AddLineData(lineNumber, lineAddressStr);
                                    string symbolLine = GetSymbolLineForFunction(symbolOutput, lineAddressStr);

                                    if (!string.IsNullOrEmpty(symbolLine))
                                    {
                                        Match regexMatch = Regex.Match(symbolLine, "Type.*?::");
                                        if (regexMatch.Success)
                                        {
                                            string matchStr = regexMatch.Value;
                                            string matchSearchString = ", ";
                                            startIndex = matchStr.IndexOf(matchSearchString) + matchSearchString.Length;
                                            endIndex = matchStr.LastIndexOf("::");
                                            string fullyQualifiedName = matchStr.Substring(startIndex, endIndex - startIndex);
                                            currentLineData.Namespace = fullyQualifiedName;

                                            //TODO_comfyjase_14/03/2025: Check for class names here too?
                                            // Namespace::NestedNamespace::Class::DOC_TEST_THINGY
                                            //currentLineData.ClassName = fullyQualifiedName.SomethingHere();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            profiler.End();

            return pdbData;
        }

        internal static T GetTestCasePropertyValue<T>(TestCase test, TestProperty testProperty)
        {
            object testPropertyObject = test.GetPropertyValue(testProperty);
            return (T)testPropertyObject;
        }

        internal static string GetCommandArguments(string executableFilePath, int batchNumber, DoctestTestSettings settings, IEnumerable<TestCase> tests)
        {
            List<string> testCaseNames = tests.Select(t => string.Format("*\"{0}\"*", t.DisplayName.Replace(",", @"\,"))).ToList();

            // Sorted into doctest specific argument formatting: *"[TestDecorator] Test 1"*,*"[TestDecorator] Test 2"*
            string doctestTestCaseCommandArgument = "--test-case=" + string.Join(",", testCaseNames);

            // Report so we know what tests passed, failed, skipped.
            string testReportFilePath = Directory.GetParent(executableFilePath).FullName + "\\" + Path.GetFileNameWithoutExtension(executableFilePath) + "_TestReport_" + batchNumber.ToString() + ".xml";
            string doctestReporterCommandArgument = "--reporters=xml --out=" + testReportFilePath;

            // Full doctest arguments: --test-case=*"[TestDecorator] Test 1"*,*"[TestDecorator] Test 2"* --reporters=xml --out=AppTestReport.xml
            string doctestArguments = doctestTestCaseCommandArgument + " " + doctestReporterCommandArgument;

            string fullCommandArguments = string.Empty;

            // User defined command arguments: --test
            if (settings != null && settings.ExecutorSettings != null && !string.IsNullOrEmpty(settings.ExecutorSettings.CommandArguments))
            {
                fullCommandArguments = settings.ExecutorSettings.CommandArguments + " " + doctestArguments;
            }
            // Otherwise, just use regular doctest arguments
            else
            {
                fullCommandArguments = doctestArguments;
            }
            
            return fullCommandArguments;
        }

        private static string GetNamespaceSubstring(string line)
        {
            string testFileNamespace = Constants.EmptyNamespaceString;

            string namespaceKeyword = "namespace";
            int startIndex = line.IndexOf(namespaceKeyword) + namespaceKeyword.Length;

            // E.g. if the line only contains the namespace keyword and nothing else...
            // Then just count this as an empty namespace.
            if (startIndex == line.Length)
            {
                return testFileNamespace;
            }

            int endIndex = (line.Contains("{") ? line.IndexOf("{") : line.Length);

            string subString = line.Substring(startIndex, endIndex - startIndex);
            if (!string.IsNullOrEmpty(subString))
            {
                testFileNamespace = subString.Replace(" ", string.Empty);
            }

            return testFileNamespace;
        }

        private static string GetClassNameSubstring(string line)
        {
            string className = Constants.EmptyClassString;

            string classKeyword = "class ";
            int startIndex = line.IndexOf(classKeyword) + classKeyword.Length;
            int endIndex = (Regex.Match(line, @"\b\s:\s\b").Success ? line.LastIndexOf(" :") : line.Length - 1);

            string subString = line.Substring(startIndex, endIndex - startIndex);
            if (!string.IsNullOrEmpty(subString))
            {
                className = subString.Replace(" ", string.Empty);
            }

            return className;
        }

        private static string GetTestSuiteNameSubstring(string line)
        {
            string testSuiteName = Constants.EmptyNamespaceString;

            string openBracketStartString = "(\"";
            int startIndex = line.IndexOf(openBracketStartString) + openBracketStartString.Length;
            int endIndex = line.LastIndexOf("\"");

            string subString = line.Substring(startIndex, endIndex - startIndex);
            if (!string.IsNullOrEmpty(subString))
            {
                testSuiteName = subString;
            }

            return testSuiteName;
        }

        private static string GetTestCaseNameSubstring(string line)
        {
            string testName = string.Empty;

            string openBracketStartString = "(\"";
            int startIndex = line.IndexOf(openBracketStartString) + openBracketStartString.Length;
            int endIndex = line.LastIndexOf("\"");

            string subString = line.Substring(startIndex, endIndex - startIndex);
            if (!string.IsNullOrEmpty(subString))
            {
                testName = subString;
            }

            return testName;
        }

        public static TestCase CreateTestCase(string testOwner, string testNamespace, string testClassName, string testCaseName, string sourceCodeFilePath, int lineNumber, bool shouldBeSkipped)
        {
            string[] parts = new string[] { testNamespace, testClassName, testCaseName.Replace(@":", @"\:") };
            string fullyQualifiedName = string.Join(@"::", parts);

            TestCase testCase = new TestCase(fullyQualifiedName, Constants.ExecutorUri, testOwner);
            testCase.DisplayName = testCaseName;
            testCase.CodeFilePath = sourceCodeFilePath;
            testCase.LineNumber = lineNumber;

            testCase.SetPropertyValue(Helpers.Constants.ShouldBeSkippedTestProperty, shouldBeSkipped);

            return testCase;
        }

        internal static List<TestCase> GetTestCases(string executableFilePath, DoctestTestSettings settings = null)
        {
            List<TestCase> testCases = new List<TestCase>();

            Profiler profiler = new Profiler();
            profiler.Start();
            {
                List<string> dependencyFilePaths = new List<string>();
                List<string> dependencies = GetDependencies(executableFilePath);
                string executableDirectory = Directory.GetParent(executableFilePath).FullName;
                foreach (string dependency in dependencies)
                {
                    string dllFilePath = executableDirectory + "\\" + dependency;
                    if (File.Exists(dllFilePath))
                    {
                        // dll is a direct dependent for executableFilePath
                        // So make sure to include any test source files from the dll too so they can be executed as well.
                        dependencyFilePaths.Add(dllFilePath);
                    }
                }

                List<PdbData> pdbData = Utilities.ReadPdbFile(executableFilePath, dependencyFilePaths, settings);

                foreach (PdbData data in pdbData)
                {
                    foreach (PdbLineData lineData in data.LineData)
                    {
                        // TODO: TEST_SUITE

                        if (lineData.LineStr.Contains("TEST_CASE(\""))
                        {
                            string testOwner = executableFilePath;
                            string testCaseName = GetTestCaseNameSubstring(lineData.LineStr);
                            string testNamespace = string.IsNullOrEmpty(lineData.Namespace) ? "Empty Namespace" : lineData.Namespace;
                            string testClassName = string.IsNullOrEmpty(lineData.ClassName) ? "Empty Class" : lineData.ClassName;

                            //TODO_comfyjase_30/01/2025: This assumes '* doctest::skip()' is on the same line as the name of the test...
                            // Would be nice to implement logic to be able to cope with '* doctest::skip()' being on a new line too
                            bool shouldSkipTest = Helpers.Constants.SkipTestKeywords.Any(s => lineData.LineStr.Contains(s));

                            TestCase testCase = CreateTestCase(testOwner,
                                testNamespace,
                                testClassName,
                                testCaseName,
                                data.CodeFilePath,
                                lineData.Number,
                                shouldSkipTest);

                            testCases.Add(testCase);
                        }
                    }
                }
            }
            profiler.End();

            return testCases;
        }
    }
}
