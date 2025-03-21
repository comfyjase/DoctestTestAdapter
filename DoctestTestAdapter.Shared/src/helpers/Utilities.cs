using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Reflection;
using DoctestTestAdapter.Settings;
using Microsoft.Win32;
using System.Globalization;
using DoctestTestAdapter.Shared.Profiling;
using System.Diagnostics;
using DoctestTestAdapter.Shared.Keywords;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Runtime.CompilerServices;

namespace DoctestTestAdapter.Shared.Helpers
{
    internal static class Utilities
    {
        /// <summary>
        /// GetSolutionDirectory.
        /// </summary>
        /// <param name="startingDirectoryPath">Starting path to work out where the parent solution directory is. Uses Environment.CurrentDirectory if nothing is provided.</param>
        /// <returns>string - Full path to the solution directory.</returns>
        /// <exception cref="FileNotFoundException">Thrown if no solution file is found.</exception>
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

            return directory?.FullName ?? throw new FileNotFoundException($"Could not find solution file in {directory}, abort!");
        }

        /// <summary>
        /// Gets the general install directory for any of the supported and installed visual studio versions.
        /// </summary>
        /// <returns>string - Full directory path for the vs install directory.</returns>
        /// <exception cref="DirectoryNotFoundException">Thrown if foundInstances is empty or if no vsInstallDirectory is found.</exception>
        public static string GetVSInstallDirectory()
        {
            string vsInstallDirectory = string.Empty;

            Profiler profiler = new Profiler();
            profiler.Start();
            {
                List<string> foundInstances = Directory.GetDirectories("C:\\ProgramData\\Microsoft\\VisualStudio\\Packages\\_Instances\\").ToList();
                if (foundInstances.Count == 0)
                    throw new DirectoryNotFoundException("Could not find VS instance directories under \"C:\\ProgramData\\Microsoft\\VisualStudio\\Packages\\_Instances\\\", abort!");

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

                            bool foundInstalledVersion = Helpers.Constants.SupportedVisualStudioNames.Any(s => displayName.Contains(s));
                            if (!foundInstalledVersion)
                                continue;

                            // This will find whatever the first valid VS install location is and then base the directory from that.
                            vsInstallDirectory = (string)key.GetValue("InstallLocation");
                            if (!string.IsNullOrEmpty(vsInstallDirectory))
                            {
                                break;
                            }
                        }
                    }
                }
            }
            profiler.End();

            if (!Directory.Exists(vsInstallDirectory))
                throw new DirectoryNotFoundException("Could not find any valid VS install locations... (somehow?!), abort!");

            return vsInstallDirectory;
        }

        /// <summary>
        /// GetPDBFilePath
        /// </summary>
        /// <param name="executableFilePath">Full file path to the discovered executable.</param>
        /// <param name="settings">Doctest test adapter settings.</param>
        /// <returns>string - Full file path to the pdb file for executableFilePath</returns>
        /// <exception cref="FileNotFoundException">Thrown if unable to find VsDevCmd.bat or if the found pdb file path doesn't exist.</exception>
        internal static string GetPDBFilePath(string executableFilePath, DoctestTestSettings settings = null)
        {
            string pdbFilePath = null;

            Profiler profiler = new Profiler();
            profiler.Start();
            {
                string batFilePath = GetVSInstallDirectory() + "\\Common7\\Tools\\VsDevCmd.bat";
                if (!File.Exists(batFilePath))
                    throw new FileNotFoundException($"Could not find file {batFilePath}, abort!");

                System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo();
                processStartInfo.CreateNoWindow = true;
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.FileName = @"cmd.exe";
                processStartInfo.Arguments = "/c call " + string.Format("\"{0}\"", batFilePath) + " & dumpbin /PDBPATH " + "\"" + executableFilePath + "\"";

                System.Diagnostics.Process dumpBinProcess = new System.Diagnostics.Process();
                dumpBinProcess.StartInfo = processStartInfo;
                dumpBinProcess.Start();

                string output = dumpBinProcess.StandardOutput.ReadToEnd();
                if (settings != null && settings.GeneralSettings != null && settings.GeneralSettings.PrintStandardOutput)
                    Console.WriteLine("dumpbin output: \n" + output);
                string errors = dumpBinProcess.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(errors))
                    Console.WriteLine("dumpbin errors: \n\t" + errors);
                dumpBinProcess.WaitForExit();

                string startStr = "PDB file found at \'";
                int startIndex = output.IndexOf(startStr, StringComparison.OrdinalIgnoreCase) + startStr.Length;
                int endIndex = output.LastIndexOf("\'", StringComparison.OrdinalIgnoreCase);

                pdbFilePath = output.Substring(startIndex, endIndex - startIndex);
            }
            profiler.End();

            if (!File.Exists(pdbFilePath))
                throw new FileNotFoundException($"Could not find pdb file for executable file {executableFilePath}, abort!");

            return pdbFilePath;
        }

        /// <summary>
        /// GetDependencies - Returns any dependencies that executableFilePath relies on (.dlls).
        /// </summary>
        /// <param name="executableFilePath">Full path to the discovered executable.</param>
        /// <param name="settings">Doctest test adapter settings.</param>
        /// <returns>List<string> - List of file names for all dependencies.</returns>
        /// <exception cref="FileNotFoundException">Thrown if unable to find VsDevCmd.bat.</exception>
        internal static List<string> GetDependencies(string executableFilePath, DoctestTestSettings settings = null)
        {
            List<string> dependencies = new List<string>();

            Profiler profiler = new Profiler();
            profiler.Start();
            {
                string batFilePath = GetVSInstallDirectory() + "\\Common7\\Tools\\VsDevCmd.bat";
                if (!File.Exists(batFilePath))
                    throw new FileNotFoundException($"Could not find file {batFilePath}, abort!");

                System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo();
                processStartInfo.CreateNoWindow = true;
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.FileName = @"cmd.exe";
                processStartInfo.Arguments = "/c call " + string.Format("\"{0}\"", batFilePath) + " & dumpbin /dependents " + "\"" + executableFilePath + "\"";

                System.Diagnostics.Process dumpBinProcess = new System.Diagnostics.Process();
                dumpBinProcess.StartInfo = processStartInfo;
                dumpBinProcess.Start();

                string output = dumpBinProcess.StandardOutput.ReadToEnd();
                if (settings != null && settings.GeneralSettings != null && settings.GeneralSettings.PrintStandardOutput)
                    Console.WriteLine("dumpbin output: \n" + output);
                string errors = dumpBinProcess.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(errors))
                    Console.WriteLine("dumpbin errors: \n\t" + errors);
                dumpBinProcess.WaitForExit();

                string startIndexString = "Image has the following dependencies:";
                string endIndexString = "Summary";
                int startIndex = output.IndexOf(startIndexString, StringComparison.OrdinalIgnoreCase) + startIndexString.Length;
                int endIndex = output.IndexOf(endIndexString, StringComparison.OrdinalIgnoreCase);
                string outputSubstring = output.Substring(startIndex, endIndex - startIndex);

                dependencies = outputSubstring.Split('\n')
                    .Where(s => s.Contains(".dll"))
                    .Select(s => s.Trim().Replace(" ", ""))
                    .ToList();
            }
            profiler.End();

            return dependencies;
        }

        /// <summary>
        /// GetSourceFiles - Returns a list of full file paths to relevant source files from executableFilePath.
        /// </summary>
        /// <param name="executableFilePath">The full file path the discovered executable.</param>
        /// <param name="pdbFilePath">The full file path to the discovered executable's pdb file.</param>
        /// <param name="logger">Logger for sending test messages.</param>
        /// <param name="settings">Doctest test adapter settings.</param>
        /// <returns>List<string> - List of full file paths of source files.</returns>
        /// <exception cref="FileNotFoundException">Thrown if unable to find the cvdump.exe file.</exception>
        internal static List<string> GetSourceFiles(string executableFilePath, string pdbFilePath, IMessageLogger logger, DoctestTestSettings settings = null)
        {
            List<string> sourceFiles = new List<string>();

            Profiler profiler = new Profiler();
            profiler.Start();
            {
                string solutionDirectory = GetSolutionDirectory(Directory.GetParent(executableFilePath).FullName);
                string cvDumpFilePath = Directory.GetParent(Assembly.GetExecutingAssembly().CodeBase.Replace(@"file:///", string.Empty)) + "\\thirdparty\\cvdump\\cvdump.exe";
                if (!File.Exists(cvDumpFilePath))
                    throw new FileNotFoundException($"Could not find file {cvDumpFilePath}, abort!");

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

                string output = cvDumpProcess.StandardOutput.ReadToEnd();
                if (settings != null && settings.GeneralSettings != null && settings.GeneralSettings.PrintStandardOutput)
                    Console.WriteLine("cvdumpbin output: \n" + output);
                string errors = cvDumpProcess.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(errors))
                    Console.WriteLine("cvdumpbin errors: \n\t" + errors);

                cvDumpProcess.WaitForExit();

                string startStr = "STRINGTABLE";
                int startIndex = output.IndexOf(startStr, StringComparison.OrdinalIgnoreCase) + startStr.Length;
                int endIndex = output.Length;
                string stringTableStr = output.Substring(startIndex, endIndex - startIndex);

                // User has given specific search directories to use, so make sure we only return source files from those directories.
                if (settings != null && settings.DiscoverySettings != null && settings.DiscoverySettings.SearchDirectories.Count > 0)
                {
                    if (settings.DiscoverySettings.AreSearchDirectoriesValid(solutionDirectory, out string message))
                    {
                        sourceFiles.AddRange
                        (
                            stringTableStr.Split('\n')
                                .Select(s => s.Replace("\n", string.Empty).Replace("\r", string.Empty).Substring(s.IndexOf(" ") + 1))
                                .Where(s => (settings.DiscoverySettings.SearchDirectories.Any(sd => (s.Contains(solutionDirectory + "\\" + sd + "\\") || s.Contains(sd + "\\"))) && !s.Contains("doctest.h") && s.EndsWith(".h") && File.Exists(s)))
                                .ToList()
                        );
                    }
                    else
                    {
                        logger.SendMessage(TestMessageLevel.Warning, message);

                        // Get all relevant source files under the solution directory since provided search directories were invalid.
                        // Means the test adapter can continue in some way without having to stop everything and remaining in a bad state.
                        sourceFiles.AddRange
                        (
                            stringTableStr.Split('\n')
                                .Select(s => s.Replace("\n", string.Empty).Replace("\r", string.Empty).Substring(s.IndexOf(" ") + 1))
                                .Where(s => (s.Contains(solutionDirectory) && !s.Contains("doctest.h") && s.EndsWith(".h") && File.Exists(s)))
                                .ToList()
                        );
                    }
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

        internal static T GetTestCasePropertyValue<T>(TestCase test, TestProperty testProperty)
        {
            object testPropertyObject = test.GetPropertyValue(testProperty);
            return (T)testPropertyObject;
        }

        internal static string GetCommandArguments(string executableFilePath, int batchNumber, DoctestTestSettings settings, IEnumerable<TestCase> tests)
        {
            List<string> testCaseNames = tests.Select(t => string.Format("*\"{0}\"*", t.DisplayName.Replace(@"\", @"\\").Replace(",", @"\,"))).ToList();

            // Sorted into doctest specific argument formatting: *"[TestDecorator] Test 1"*,*"[TestDecorator] Test 2"*
            string doctestTestCaseCommandArgument = "--test-case=" + string.Join(",", testCaseNames);

            // Report so we know what tests passed, failed, skipped.
            string testReportFilePath = Directory.GetParent(executableFilePath).FullName + "\\" + Path.GetFileNameWithoutExtension(executableFilePath) + "_TestReport_" + batchNumber.ToString() + ".xml";
            string doctestReporterCommandArgument = "--duration=true --reporters=xml --out=" + testReportFilePath;

            // Full doctest arguments: --test-case=*"[TestDecorator] Test 1"*,*"[TestDecorator] Test 2"* --duration=true --reporters=xml --out=AppTestReport.xml
            string doctestArguments = doctestTestCaseCommandArgument + " " + doctestReporterCommandArgument;

            string fullCommandArguments = string.Empty;

            // User defined command arguments: --test
            if (settings != null && settings.GeneralSettings != null && !string.IsNullOrEmpty(settings.GeneralSettings.CommandArguments))
            {
                fullCommandArguments = settings.GeneralSettings.CommandArguments + " " + doctestArguments;
            }
            // Otherwise, just use regular doctest arguments
            else
            {
                fullCommandArguments = doctestArguments;
            }
            
            return fullCommandArguments;
        }

        public static TestCase CreateTestCase(string testOwner, string testNamespace, string testClassName, string testCaseName, string sourceCodeFilePath, int lineNumber)
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

            TestCase testCase = new TestCase(fullyQualifiedName, Constants.ExecutorUri, testOwner);
            testCase.DisplayName = testCaseName;
            testCase.CodeFilePath = sourceCodeFilePath;
            testCase.LineNumber = lineNumber;

            return testCase;
        }

        internal static List<string> GetAllTestSuiteNames(string executableFilePath, DoctestTestSettings settings = null)
        {
            List<string> testSuiteNames = new List<string>();

            string solutionDirectory = GetSolutionDirectory(Directory.GetParent(executableFilePath).FullName);

            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.CreateNoWindow = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.WorkingDirectory = solutionDirectory;
            processStartInfo.FileName = string.Format("\"{0}\"", executableFilePath);
            if (settings != null && settings.GeneralSettings != null && !string.IsNullOrEmpty(settings.GeneralSettings.CommandArguments))
            {
                processStartInfo.Arguments = settings.GeneralSettings.CommandArguments + " --no-intro=true --no-version=true --list-test-suites";
            }
            else
            {
                processStartInfo.Arguments = "--no-intro=true --no-version=true --list-test-suites";
            }
            
            System.Diagnostics.Process exeProcess = new System.Diagnostics.Process();
            exeProcess.StartInfo = processStartInfo;
            exeProcess.Start();

            string output = exeProcess.StandardOutput.ReadToEnd();
            if (settings != null && settings.GeneralSettings != null && settings.GeneralSettings.PrintStandardOutput)
                Console.WriteLine(Path.GetFileName(executableFilePath) + " output: \n" + output);
            string errors = exeProcess.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(errors))
                Console.WriteLine(Path.GetFileName(executableFilePath) + " errors: \n\t" + errors);

            exeProcess.WaitForExit();

            string startSearchString = "===============================================================================\r\n";
            string endSearchString = "\r\n===============================================================================";
            int startOfDoctestListIndex = output.IndexOf(startSearchString) + startSearchString.Length;
            int endOfDoctestListIndex = output.LastIndexOf(endSearchString);
            string subString = output.Substring(startOfDoctestListIndex, endOfDoctestListIndex - startOfDoctestListIndex); 

            if (!string.IsNullOrEmpty(subString))
            {
                testSuiteNames = subString
                    .Split('\n')
                    .Select(s => s.Trim())
                    .ToList();
            }

            return testSuiteNames;
        }

        internal static List<string> GetAllTestCaseNames(string executableFilePath, DoctestTestSettings settings = null)
        {
            List<string> testCaseNames = new List<string>();

            string solutionDirectory = GetSolutionDirectory(Directory.GetParent(executableFilePath).FullName);

            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.CreateNoWindow = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.WorkingDirectory = solutionDirectory;
            processStartInfo.FileName = string.Format("\"{0}\"", executableFilePath);
            if (settings != null && settings.GeneralSettings != null && !string.IsNullOrEmpty(settings.GeneralSettings.CommandArguments))
            {
                processStartInfo.Arguments = settings.GeneralSettings.CommandArguments + " --no-intro=true --no-version=true --list-test-cases";
            }
            else
            {
                processStartInfo.Arguments = "--no-intro=true --no-version=true --list-test-cases";
            }

            System.Diagnostics.Process exeProcess = new System.Diagnostics.Process();
            exeProcess.StartInfo = processStartInfo;
            exeProcess.Start();

            string output = exeProcess.StandardOutput.ReadToEnd();
            if (settings != null && settings.GeneralSettings != null && settings.GeneralSettings.PrintStandardOutput)
                Console.WriteLine(Path.GetFileName(executableFilePath) + " output: \n" + output);
            string errors = exeProcess.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(errors))
                Console.WriteLine(Path.GetFileName(executableFilePath) + " errors: \n\t" + errors);

            exeProcess.WaitForExit();

            string startSearchString = "===============================================================================\r\n";
            string endSearchString = "\r\n===============================================================================";
            int startOfDoctestListIndex = output.IndexOf(startSearchString) + startSearchString.Length;
            int endOfDoctestListIndex = output.LastIndexOf(endSearchString);
            string subString = output.Substring(startOfDoctestListIndex, endOfDoctestListIndex - startOfDoctestListIndex);

            if (!string.IsNullOrEmpty(subString))
            {
                testCaseNames = subString
                    .Trim()
                    .Split('\n')
                    .Select(s => s.Trim())
                    .ToList();
            }

            return testCaseNames;
        }

        internal static List<TestCase> GetTestCases(string executableFilePath, IMessageLogger logger, DoctestTestSettings settings = null)
        {
            Utilities.CheckFilePath(executableFilePath, nameof(executableFilePath));

            List<TestCase> testCases = new List<TestCase>();

            Profiler profiler = new Profiler();
            profiler.Start();
            {
                List<string> dependencyFilePaths = new List<string>();
                List<string> dependencies = GetDependencies(executableFilePath, settings);
                string executableDirectory = Directory.GetParent(executableFilePath).FullName;
                foreach (string dependency in dependencies)
                {
                    string dllFilePath = executableDirectory + "\\" + dependency;
                    if (File.Exists(dllFilePath))
                    {
                        // dll is a direct dependent for executableFilePath
                        // So make sure to include any test source files from the dll too so they can be executed as well.
                        // This only goes one level deep atm for dependencies.
                        // If we need to recursively check for dependencies, this function would probably need more arguments.
                        // E.g. testCases.AddRange(GetTestCases(dllFilePath, settings, ...));
                        dependencyFilePaths.Add(dllFilePath);
                    }
                }

                List<string> allTestSuiteNames = GetAllTestSuiteNames(executableFilePath, settings);
                List<string> allTestCaseNames = GetAllTestCaseNames(executableFilePath, settings);

                // Get all of the source files
                string pdbFilePath = GetPDBFilePath(executableFilePath, settings);
                List<string> allSourceFilePaths = GetSourceFiles(executableFilePath, pdbFilePath, logger, settings);
                foreach (string dependencyFilePath in dependencyFilePaths)
                {
                    pdbFilePath = GetPDBFilePath(dependencyFilePath, settings);
                    allSourceFilePaths.AddRange(GetSourceFiles(dependencyFilePath, pdbFilePath, logger, settings));
                }
                
                string testNamespace = string.Empty;
                string testClassName = string.Empty;

                List<Keyword> keywords = new List<Keyword>()
                {
                    new NamespaceKeyword(),
                    new ClassKeyword(),
                    new DoctestTestSuiteKeyword(allTestSuiteNames),
                    new DoctestTestCaseKeyword(allTestCaseNames),
                };

                // Loop over all of the source files and read them line by line
                foreach (string sourceFilePath in allSourceFilePaths)
                {
                    string[] allLines = File.ReadAllLines(sourceFilePath);
                    int currentLineNumber = 0;

                    foreach (string line in allLines)
                    {
                        ++currentLineNumber;
                        keywords.ForEach(k => k.Check(executableFilePath, sourceFilePath, ref testNamespace, ref testClassName, line, currentLineNumber, ref testCases));
                    }
                }
            }
            profiler.End();

            return testCases;
        }

        /// <summary>
        /// CheckNull - checks if arg == null.
        /// </summary>
        /// <param name="arg">Argument to check.</param>
        /// <param name="nameOfArg">Name of the argument to check from the calling function.</param>
        /// <param name="memberName">Name of the function that called this.</param>
        /// <exception cref="ArgumentNullException">Thrown if arg is null.</exception>
        internal static void CheckNull<T>(T arg, string nameOfArg, [CallerMemberName] string memberName = "")
        {
            if (arg == null)
                throw new ArgumentNullException(nameOfArg, Constants.ErrorMessagePrefix + " " + memberName + ": Argument '" + nameOfArg + "' is null, abort!");
        }

        /// <summary>
        /// CheckString - Checks if arg == null and if arg == string.Empty.
        /// </summary>
        /// <param name="arg">String argument to check.</param>
        /// <param name="nameOfArg">Name of the string argument from the calling class.</param>
        /// <param name="memberName">Name of the function that called this validate function.</param>
        /// <exception cref="ArgumentNullException">Thrown if arg is null.</exception>
        /// <exception cref="ArgumentException">Thrown if arg is empty.</exception>
        internal static void CheckString(string arg, string nameOfArg, [CallerMemberName] string memberName = "")
        {
            CheckNull(arg, nameOfArg, memberName);
            if (arg == string.Empty)
                throw new ArgumentException(Constants.ErrorMessagePrefix + " " + memberName + ": Argument '" + nameOfArg + "' cannot be empty, abort!", nameOfArg);
        }

        /// <summary>
        /// CheckFilePath - Checks if filePathArg == null and if filePathArg == string.Empty and if File.Exists(filePathArg)
        /// </summary>
        /// <param name="filePathArg"></param>
        /// <param name="nameOfFilePathArg"></param>
        /// <param name="memberName"></param>
        /// <exception cref="ArgumentNullException">Thrown if filePathArg is null.</exception>
        /// <exception cref="ArgumentException">Thrown if filePathArg is empty.</exception>
        /// <exception cref="FileNotFoundException"></exception>
        internal static void CheckFilePath(string filePathArg, string nameOfFilePathArg, [CallerMemberName] string memberName = "")
        {
            CheckString(filePathArg, nameOfFilePathArg, memberName);
            if (!File.Exists(filePathArg))
                throw new FileNotFoundException(Constants.ErrorMessagePrefix + " " + memberName + ": Argument '" + nameOfFilePathArg + "' file does not exist, abort!", filePathArg);
        }

        /// <summary>
        /// CheckEnumerable - Checks if arg == null and if arg.Count() == 0.
        /// </summary>
        /// <typeparam name="T">Type of enumerable used.</typeparam>
        /// <param name="arg">Enumerable argument to check.</param>
        /// <param name="nameOfArg">Name of the enumerable argument from the calling class.</param>
        /// <param name="memberName">Name of the function that called this validate function.</param>
        /// <exception cref="ArgumentNullException">Thrown if arg is null.</exception>
        /// <exception cref="ArgumentException">Thrown if arg is empty.</exception>
        internal static void CheckEnumerable<T>(IEnumerable<T> arg, string nameOfArg, [CallerMemberName] string memberName = "")
        {
            CheckNull(arg, nameOfArg, memberName); 
            if (arg.Count() == 0)
                throw new ArgumentException(Constants.ErrorMessagePrefix + " " + memberName + ": Argument '" + nameOfArg + "' cannot be empty, abort!", nameOfArg);
        }
    }
}
