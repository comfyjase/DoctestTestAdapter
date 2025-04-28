// CVDumpExecutable.cs
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
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DoctestTestAdapter.Shared.Executables
{
    internal class CVDumpExecutable : Executable
    {
        private static readonly string CVDumpFilePath = Directory.GetParent(Assembly.GetExecutingAssembly().CodeBase.Replace(@"file:///", string.Empty)) + "\\thirdparty\\cvdump\\cvdump.exe";
        private string _pdbFilePath = null;

        internal CVDumpExecutable(string pdbFilePath, string solutionDirectory, DoctestTestSettings settings, IRunContext runContext, IMessageLogger logger) : base(CVDumpFilePath, solutionDirectory, settings, runContext, logger)
        {
            SetPdbFilePath(pdbFilePath);
        }

        internal void SetPdbFilePath(string pdbFilePath)
        {
            _pdbFilePath = string.Format("\"{0}\"", pdbFilePath);
        }

        internal List<string> GetSourceFiles()
        {
            List<string> sourceFiles = new List<string>();

            Arguments = "-stringtable " + _pdbFilePath;

            Start();

            if (string.IsNullOrEmpty(Output))
                throw new NullReferenceException($"cvdump did not provide valid 'Output' for -stringtable {_pdbFilePath}, abort!");

            string startStr = "STRINGTABLE";
            int startIndex = Output.IndexOf(startStr, StringComparison.OrdinalIgnoreCase) + startStr.Length;
            int endIndex = Output.Length;
            string stringTableStr = Output.Substring(startIndex, endIndex - startIndex);

            // User has given specific search directories to use, so make sure we only return source files from those directories.
            if (Settings != null && Settings.TryGetSearchDirectories(out List<string> searchDirectories))
            {
                if (Settings.DiscoverySettings.AreSearchDirectoriesValid(SolutionDirectory, out string message))
                {
                    sourceFiles.AddRange
                    (
                        stringTableStr.Split('\n')
                            // Note: the .Substring is used because the string table adds some kind of virtual address information before the header file path.
                            // Getting the first index of the space will make sure the string starts from the beginning of the file path.
                            .Select(s => s.Trim('\r', '\n').Substring(s.IndexOf(" ") + 1))
                            .Where(s => (searchDirectories.Any(sd => (s.Contains(SolutionDirectory + "\\" + sd + "\\") || (s.Contains(SolutionDirectory) && s.Contains(sd + "\\")))) && !s.Contains("doctest.h") && s.EndsWith(".h") && !s.Contains(".gen.") && File.Exists(s)))
                            .ToList()
                    );
                }
                else
                {
                    if (Logger != null)
                        Logger.SendMessage(TestMessageLevel.Warning, message);

                    // Get all relevant source files under the solution directory since provided search directories were invalid.
                    // Means the test adapter can continue in some way without having to stop everything and remaining in a bad state.
                    sourceFiles.AddRange
                    (
                        stringTableStr.Split('\n')
                            .Select(s => s.Trim('\r', '\n').Substring(s.IndexOf(" ") + 1))
                            .Where(s => (s.Contains(SolutionDirectory) && !s.Contains("doctest.h") && s.EndsWith(".h") && !s.Contains(".gen.") && File.Exists(s)))
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
                        .Select(s => s.Trim('\r', '\n').Substring(s.IndexOf(" ") + 1))
                        .Where(s => (s.Contains(SolutionDirectory) && !s.Contains("doctest.h") && s.EndsWith(".h") && !s.Contains(".gen.") && File.Exists(s)))
                        .ToList()
                );
            }

            return sourceFiles;
        }
    }
}
