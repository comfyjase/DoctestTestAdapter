// DumpBinExecutable.cs
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
using DoctestTestAdapter.Shared.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DoctestTestAdapter.Shared.Executables
{
    internal class DumpBinExecutable : Executable
    {
        private string _batFilePath = null;
        private string _discoveredExecutableFilePath = null;

        /// <summary>
        /// DumpBinExecutable Constructor
        /// Note, not passing arguments straight away since this should provide an easier setup for calling dumpbin first.
        /// </summary>
        /// <param name="discoveredExecutableFilePath">Full file path to the discovered executable.</param>
        /// <param name="settings">Doctest test adapter settings.</param>
        /// <exception cref="FileNotFoundException">Thrown if VsDevCmd.bat could not be found.</exception>
        internal DumpBinExecutable(string discoveredExecutableFilePath, string solutionDirectory, DoctestTestSettings settings, IRunContext runContext, IMessageLogger logger) : base(@"cmd.exe", solutionDirectory, settings, runContext, logger)
        {
            _batFilePath = Utilities.GetVSInstallDirectory() + "\\Common7\\Tools\\VsDevCmd.bat";
            if (!File.Exists(_batFilePath))
                throw new FileNotFoundException($"Could not find file {_batFilePath}, abort!");
            _batFilePath = string.Format("\"{0}\"", _batFilePath);
            SetDiscoveredExecutable(discoveredExecutableFilePath);
        }

        internal void SetDiscoveredExecutable(string executableFilePath)
        {
            _discoveredExecutableFilePath = string.Format("\"{0}\"", executableFilePath);
        }

        internal List<string> GetDependencies()
        {
            List<string> dependencies = new List<string>();

            Arguments = "/c call " + _batFilePath + " & dumpbin /dependents " + _discoveredExecutableFilePath;
            
            Start();

            if (string.IsNullOrEmpty(Output))
                throw new NullReferenceException($"dumpbin did not provide valid 'Output' for /dependents {_discoveredExecutableFilePath}, abort!");

            string startIndexString = "Image has the following dependencies:";
            string endIndexString = "Summary";
            int startIndex = Output.IndexOf(startIndexString, StringComparison.OrdinalIgnoreCase) + startIndexString.Length;
            int endIndex = Output.IndexOf(endIndexString, StringComparison.OrdinalIgnoreCase);
            string outputSubstring = Output.Substring(startIndex, endIndex - startIndex);

            dependencies = outputSubstring.Split('\n')
                .Where(s => s.Contains(".dll"))
                .Select(s => s.Trim())
                .ToList();
            
            return dependencies;
        }

        internal string GetPDBFilePath()
        {
            string pdbFilePath = null;
            
            Arguments = "/c call " + _batFilePath + " & dumpbin /PDBPATH " + _discoveredExecutableFilePath;

            Start();
            
            if (string.IsNullOrEmpty(Output))
                throw new NullReferenceException($"dumpbin did not provide valid 'Output' for /PDBPATH {_discoveredExecutableFilePath}, abort!");

            string startStr = "PDB file found at \'";
            int startIndex = Output.IndexOf(startStr, StringComparison.OrdinalIgnoreCase) + startStr.Length;
            int endIndex = Output.LastIndexOf("\'", StringComparison.OrdinalIgnoreCase);
            pdbFilePath = Output.Substring(startIndex, endIndex - startIndex);

            if (!File.Exists(pdbFilePath))
                throw new FileNotFoundException($"dumpbin could not find pdb file {pdbFilePath} for {_discoveredExecutableFilePath}, abort!");

            return pdbFilePath;
        }
    }
}
