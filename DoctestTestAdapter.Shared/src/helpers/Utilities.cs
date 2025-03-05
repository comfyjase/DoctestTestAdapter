﻿using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using System.Reflection;
using DoctestTestAdapter.Settings;

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

            while (directory != null && !directory.EnumerateFiles("*.sln").Any())
                directory = directory.Parent;

            return directory?.FullName ?? throw new FileNotFoundException($"Could not find solution directory {directory}");
        }

        internal static string GetProjectDirectory(string projectFileType, string startingDirectoryPath = null)
        {
            if (startingDirectoryPath == null)
            {
                startingDirectoryPath = Environment.CurrentDirectory;
            }

            DirectoryInfo directory = new DirectoryInfo(startingDirectoryPath);

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
            return pdbFilePath ?? throw new FileNotFoundException($"Could not find pdb file for executable file {executableFilePath}");
        }

        internal static List<string> GetDependencies(string executableFilePath)
        {
            List<string> dependencies = new List<string>();
            string batFilePath = "\"C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\Common7\\Tools\\VsDevCmd.bat\"";

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

            return dependencies;
        }

        internal static List<string> GetSourceFiles(string executableFilePath, DoctestTestSettings settings = null)
        {
            List<string> sourceFiles = new List<string>();

            // Checking if executable has any dependencies.
            // These dependencies may also have test files to use.
            List<string> dependencies = GetDependencies(executableFilePath);
            string executableDirectory = Directory.GetParent(executableFilePath).FullName;
            foreach (string dependency in dependencies)
            {
                string dllFilePath = executableDirectory + "\\" + dependency;
                if (File.Exists(dllFilePath))
                {
                    // dll is a direct dependent for executableFilePath
                    // So make sure to include any test source files from the dll too so they can be executed as well.
                    sourceFiles.AddRange(GetSourceFiles(dllFilePath, settings));
                }
            }

            string pdbFilePath = GetPDBFilePath(executableFilePath);
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

            return sourceFiles.Distinct().ToList();
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

        private static void CheckForCurlyBrackets(string line, Stack<int> stack, Action onStackEmpty)
        {
            foreach (char letter in line)
            {
                switch (letter)
                {
                    case '{':
                    {
                        stack.Push(stack.Count + 1);
                        break;
                    }
                    case '}':
                    {
                        if (stack.Count > 0)
                            stack.Pop();

                        if (stack.Count == 0)
                            onStackEmpty();

                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }
        }

        private static void CheckForCommentBlockStart(string filePath, string line, Stack<int> stack, Action onStackEmpty, ref bool insideFlag)
        {
            string lineWithoutWhiteSpaceCharacters = whitespace.Replace(line, string.Empty);

            if (lineWithoutWhiteSpaceCharacters.StartsWith(@"/*"))
            {
                stack.Push(stack.Count + 1);

                if (!insideFlag)
                {
                    insideFlag = true;
                }
            }
        }

        private static void CheckForCommentBlockEnd(string filePath, string line, Stack<int> stack, Action onStackEmpty, ref bool insideFlag)
        {
            string lineWithoutWhiteSpaceCharacters = whitespace.Replace(line, string.Empty);

            if (lineWithoutWhiteSpaceCharacters.EndsWith(@"*/"))
            {
                if (stack.Count > 0)
                    stack.Pop();

                if (stack.Count == 0)
                    onStackEmpty();
            }
        }

        private static void CheckForPreprocessorIf0Block(string filePath, string line, Stack<int> stack, Action onStackEmpty, ref bool insideFlag)
        {
            string lineWithoutTabs = line.Replace("\t", string.Empty);

            if (lineWithoutTabs.StartsWith(@"#if 0"))
            {
                stack.Push(stack.Count + 1);
                if (!insideFlag)
                {
                    insideFlag = true;
                }
            }
            else if (lineWithoutTabs.EndsWith(@"#endif"))
            {
                if (stack.Count > 0)
                    stack.Pop();

                if (stack.Count == 0)
                    onStackEmpty();
            }
        }

        private static void CheckForPreprocessorIfFalseBlock(string filePath, string line, Stack<int> stack, Action onStackEmpty, ref bool insideFlag)
        {
            string lineWithoutTabs = line.Replace("\t", string.Empty);

            if (lineWithoutTabs.StartsWith(@"#if false"))
            {
                stack.Push(stack.Count + 1);
                if (!insideFlag)
                {
                    insideFlag = true;
                }
            }
            else if (lineWithoutTabs.EndsWith(@"#endif"))
            {
                if (stack.Count > 0)
                    stack.Pop();

                if (stack.Count == 0)
                    onStackEmpty();
            }
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
            List<string> sourceFiles = GetSourceFiles(executableFilePath, settings);
            List<TestCase> testCases = new List<TestCase>();

            foreach (string sourceFilePath in sourceFiles)
            {
                string[] allLines = System.IO.File.ReadAllLines(sourceFilePath);

                int currentLineNumber = 1;
                string testNamespace = Constants.EmptyNamespaceString;
                string testClassName = Constants.EmptyClassString;

                //TODO_comfyjase_03/03/2025: Refactor this into a generic class for pattern matching.
                // OR even better, get all of the relevant info from the pdb file somehow...
                // pdbdump.cpp seemed promising?
                bool insideNamespace = false;
                bool insideClass = false;
                bool insideOfCommentBlock = false;
                bool insideOfPreprocessorIf0Block = false;
                bool insideOfPreprocessorIfFalseBlock = false;
                Stack<int> namespaceBracketStack = new Stack<int>();
                Stack<int> classBracketStack = new Stack<int>();
                Stack<int> commentBlockStack = new Stack<int>();
                Stack<int> preprocessorIf0BlockStack = new Stack<int>();
                Stack<int> preprocessorIfFalseBlockStack = new Stack<int>();
                Action onNamespaceStackEmpty = () => { testNamespace = Constants.EmptyNamespaceString; insideNamespace = false; };
                Action onClassStackEmpty = () => { testClassName = Constants.EmptyClassString; insideClass = false; };
                Action onCommentBlockStackEmpty = () => { insideOfCommentBlock= false; };
                Action onPreprocessorIf0BlockStackEmpty = () => { insideOfPreprocessorIf0Block = false; };
                Action onPreprocessorIfFalseBlockStackEmpty = () => { insideOfPreprocessorIfFalseBlock = false; };

                foreach (string line in allLines)
                {
                    string lineWithoutWhiteSpaceCharacters = whitespace.Replace(line, string.Empty);
                    if (lineWithoutWhiteSpaceCharacters.StartsWith("//"))
                    {
                        currentLineNumber++;
                        continue;
                    }

                    // Ignore anything else since we are in a comment block.
                    // Have to wait until the end of the comment block.
                    CheckForCommentBlockStart(sourceFilePath, line, commentBlockStack, onCommentBlockStackEmpty, ref insideOfCommentBlock);
                    if (insideOfCommentBlock)
                    {
                        CheckForCommentBlockEnd(sourceFilePath, line, commentBlockStack, onCommentBlockStackEmpty, ref insideOfCommentBlock);
                        currentLineNumber++;
                        continue;
                    }

                    // Same as above, but for #if 0
                    CheckForPreprocessorIf0Block(sourceFilePath, line, preprocessorIf0BlockStack, onPreprocessorIf0BlockStackEmpty, ref insideOfPreprocessorIf0Block);
                    if (insideOfPreprocessorIf0Block)
                    {
                        currentLineNumber++;
                        continue;
                    }

                    // Again, but for #if false
                    CheckForPreprocessorIfFalseBlock(sourceFilePath, line, preprocessorIfFalseBlockStack, onPreprocessorIfFalseBlockStackEmpty, ref insideOfPreprocessorIfFalseBlock);
                    if (insideOfPreprocessorIfFalseBlock)
                    {
                        currentLineNumber++;
                        continue;
                    }

                    // If we are inside of a namespace.
                    if (insideNamespace)
                    {
                        // Check if we are still inside it after this line.
                        // If not, reset the testNamespace string and flag we are no longer inside the namespace.
                        CheckForCurlyBrackets(line, namespaceBracketStack, onNamespaceStackEmpty);
                    }

                    // Same as above but for classes.
                    if (insideClass)
                    {
                        CheckForCurlyBrackets(line, classBracketStack, onClassStackEmpty);
                    }

                    int numberOfSpacesInLine = line.Count(Char.IsWhiteSpace);

                    // Regex for some specific keywords.
                    string regexPattern = @"\bnamespace\b";
                    if (Regex.Match(line, regexPattern, RegexOptions.IgnoreCase).Success && numberOfSpacesInLine < 3)
                    {
                        testNamespace = GetNamespaceSubstring(line);
                        CheckForCurlyBrackets(line, namespaceBracketStack, onNamespaceStackEmpty);
                        insideNamespace = true;
                        currentLineNumber++;
                        continue;
                    }

                    regexPattern = @"\bclass\b";
                    if (Regex.Match(line, regexPattern, RegexOptions.IgnoreCase).Success && numberOfSpacesInLine < 5)
                    {
                        testClassName = GetClassNameSubstring(line);
                        CheckForCurlyBrackets(line, classBracketStack, onClassStackEmpty);
                        insideClass = true;
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
                        string testOwner = executableFilePath;
                        string testCaseName = GetTestCaseNameSubstring(line);

                        //TODO_comfyjase_30/01/2025: This assumes '* doctest::skip()' is on the same line as the name of the test...
                        // Would be nice to implement logic to be able to cope with '* doctest::skip()' being on a new line too
                        bool shouldSkipTest = Helpers.Constants.SkipTestKeywords.Any(s => line.Contains(s));

                        TestCase testCase = CreateTestCase(testOwner, 
                            testNamespace,
                            testClassName,
                            testCaseName,
                            sourceFilePath,
                            currentLineNumber,
                            shouldSkipTest);

                        testCases.Add(testCase);
                    }

                    CheckForCommentBlockEnd(sourceFilePath, line, commentBlockStack, onCommentBlockStackEmpty, ref insideOfCommentBlock);

                    currentLineNumber++;
                }
            }

            return testCases;
        }
    }
}
