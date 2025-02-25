using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DoctestTestAdapter.Shared.Helpers
{
    internal static class Utilities
    {
        internal static string GetSolutionDirectory()
        {
            //TODO_comfyjase_25/02/2025: Check if AppContext.BaseDirectory works when running VS Exp for custom test adapter.
            DirectoryInfo directory = new DirectoryInfo(AppContext.BaseDirectory);

            while (directory != null && !directory.EnumerateFiles("*.sln").Any())
                directory = directory.Parent;

            return directory?.FullName ?? throw new FileNotFoundException($"Could not find solution directory {directory}");
        }

        internal static string GetProjectDirectory(string projectFileType)
        {
            DirectoryInfo directory = new DirectoryInfo(AppContext.BaseDirectory);

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
            string cvDumpFilePath = solutionDirectory + "\\ThirdParty\\cvdump\\cvdump.exe";

            System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo();
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.FileName = @"cmd.exe";
            processStartInfo.Arguments = "/c " + cvDumpFilePath + " -stringtable " + "\"" + pdbFilePath + "\"";

            System.Diagnostics.Process cvDumpProcess = new System.Diagnostics.Process();
            cvDumpProcess.StartInfo = processStartInfo;
            cvDumpProcess.Start();

            string output = cvDumpProcess.StandardOutput.ReadToEnd();
            //TODO_comfyjase_25/02/2025: Wrap this in an option for the user to toggle on/off debug test output?
            //Console.WriteLine(output);
            cvDumpProcess.WaitForExit();

            string startStr = "STRINGTABLE";
            int startIndex = output.IndexOf(startStr) + startStr.Length;
            int endIndex = output.Length;
            string stringTableStr = output.Substring(startIndex, endIndex - startIndex);

            sourceFiles = stringTableStr
                .Split('\n')
                .Select(s => s.Replace("\n", string.Empty).Replace("\r", string.Empty).Substring(s.IndexOf(" ") + 1))
                .Where(s => (s.Contains(solutionDirectory) && !s.Contains("doctest.h") && s.EndsWith(".h")))
                .ToList();

            return sourceFiles;
        }
    }
}
