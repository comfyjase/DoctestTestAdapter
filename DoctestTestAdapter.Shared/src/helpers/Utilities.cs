using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using System.Reflection;
using System.Diagnostics;

namespace DoctestTestAdapter.Shared.Helpers
{
    internal static class Utilities
    {
        internal static string EmptyNamespaceString = "Empty Namespace";
        internal static string EmptyClassString = "Empty Class";

        internal static string GetSolutionDirectory()
        {
            DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory);

            while (directory != null && !directory.EnumerateFiles("*.sln").Any())
                directory = directory.Parent;

            return directory?.FullName ?? throw new FileNotFoundException($"Could not find solution directory {directory}");
        }

        internal static string GetProjectDirectory(string projectFileType)
        {
            DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory);

            while (directory != null && !directory.EnumerateFiles("*.sln").Any() && !directory.EnumerateFiles("*" + projectFileType).Any())
                directory = directory.Parent;

            return directory?.FullName ?? throw new FileNotFoundException($"Could not find project directory {directory}");
        }

        internal static string GetPDBFilePath(string executableFilePath)
        {
            string pdbFilePath = null;
            string batFilePath = "\"C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\Common7\\Tools\\VsDevCmd.bat\"";

            System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo();
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.FileName = @"cmd.exe";
            processStartInfo.Arguments = "/c call " + batFilePath + " & dumpbin /PDBPATH " + "\"" + executableFilePath + "\"";

            System.Diagnostics.Process dumpBinProcess = new System.Diagnostics.Process();
            dumpBinProcess.StartInfo = processStartInfo;
            dumpBinProcess.Start();

            string output = dumpBinProcess.StandardOutput.ReadToEnd();
            //TODO_comfyjase_25/02/2025: Wrap this in an option for the user to toggle on/off debug test output?
            //Console.WriteLine(output);
            dumpBinProcess.WaitForExit();

            string startStr = "PDB file found at \'";
            int startIndex = output.IndexOf(startStr) + startStr.Length;
            int endIndex = output.LastIndexOf("\'");
            pdbFilePath = output.Substring(startIndex, endIndex - startIndex);
            return pdbFilePath ?? throw new FileNotFoundException($"Could not find pdb file for exe {executableFilePath}");
        }

        internal static List<string> GetSourceFiles(string executableFilePath)
        {
            List<string> sourceFiles = new List<string>();

            string pdbFilePath = GetPDBFilePath(executableFilePath);
            string solutionDirectory = GetSolutionDirectory();
            string cvDumpFilePath = Directory.GetParent(Assembly.GetExecutingAssembly().CodeBase.Replace(@"file:///", string.Empty)) + "\\ThirdParty\\cvdump\\cvdump.exe";

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

            sourceFiles = stringTableStr
                .Split('\n')
                .Select(s => s.Replace("\n", string.Empty).Replace("\r", string.Empty).Substring(s.IndexOf(" ") + 1))
                .Where(s => (s.Contains(solutionDirectory) && !s.Contains("doctest.h") && s.EndsWith(".h") && File.Exists(s)))
                .ToList();

            return sourceFiles;
        }

        private static string GetNamespaceSubstring(string line)
        {
            string testFileNamespace = EmptyNamespaceString;

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
            string className = EmptyClassString;

            string classKeyword = "class ";
            int startIndex = line.IndexOf(classKeyword) + classKeyword.Length;
            int endIndex = (line.Contains(":") ? line.LastIndexOf(" :") : line.Length);

            string subString = line.Substring(startIndex, endIndex - startIndex);
            if (!string.IsNullOrEmpty(subString))
            {
                className = subString.Replace(" ", "");
            }

            return className;
        }

        private static string GetTestSuiteNameSubstring(string line)
        {
            string testSuiteName = EmptyNamespaceString;

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
            string[] parts = new string[] { testNamespace, testClassName, testCaseName };
            string fullyQualifiedName = string.Join("::", parts);

            TestCase testCase = new TestCase(fullyQualifiedName, Constants.ExecutorUri, testOwner);
            testCase.DisplayName = testCaseName;
            testCase.CodeFilePath = sourceCodeFilePath;
            testCase.LineNumber = lineNumber;

            //testCase.SetPropertyValue(VS.Common.DoctestTestAdapter.Constants.TestAdapter.ShouldBeSkippedTestProperty, shouldBeSkipped);

            return testCase;
        }

        internal static List<TestCase> GetTestCases(string executableFilePath)
        {
            List<string> sourceFiles = GetSourceFiles(executableFilePath);
            List<TestCase> testCases = new List<TestCase>();

            foreach (string sourceFilePath in sourceFiles)
            {
                string[] allLines = System.IO.File.ReadAllLines(sourceFilePath);

                int currentLineNumber = 1;
                string testNamespace = EmptyNamespaceString;
                string testClassName = EmptyClassString;

                foreach (string line in allLines)
                {
                    int numberOfSpacesInLine = line.Count(Char.IsWhiteSpace);

                    // Regex for some specific keywords.
                    string regexPattern = @"\bnamespace\b";
                    if (Regex.Match(line, regexPattern, RegexOptions.IgnoreCase).Success && numberOfSpacesInLine < 3)
                    {
                        testNamespace = GetNamespaceSubstring(line);
                        currentLineNumber++;
                        continue;
                    }

                    regexPattern = @"\bclass\b";
                    if (Regex.Match(line, regexPattern, RegexOptions.IgnoreCase).Success && numberOfSpacesInLine < 5)
                    {
                        testClassName = GetClassNameSubstring(line);
                        currentLineNumber++;
                        continue;
                    }

                    if (line.Contains("TEST_SUITE(\""))
                    {
                        testNamespace = GetTestSuiteNameSubstring(line);
                        currentLineNumber++;
                        continue;
                    }

                    if (line.Contains("TEST_CASE(\""))
                    {
                        //TODO_comfyjase_28/01/2025: Find out if there is a way to update the test owner to point to the project instead of the individual header files.
                        //string testOwner = GetTestProjectName(_discoveryContext.RunSettings, sourceFile);
                        string testOwner = executableFilePath;
                        string testCaseName = GetTestCaseNameSubstring(line);

                        //TODO_comfyjase_30/01/2025: This assumes '* doctest::skip()' is on the same line as the name of the test...
                        // Would be nice to implement logic to be able to cope with '* doctest::skip()' being on a new line too
                        //bool markedWithDoctestSkip = VS.Common.DoctestTestAdapter.Constants.TestAdapter.SkipTestKeywords.Any(s => line.Contains(s));
                        bool shouldSkip = false;

                        TestCase testCase = CreateTestCase(testOwner, 
                            testNamespace,
                            testClassName,
                            testCaseName,
                            sourceFilePath,
                            currentLineNumber,
                            shouldSkip);

                        testCases.Add(testCase);
                    }

                    currentLineNumber++;
                }
            }

            return testCases;
        }
    }
}
