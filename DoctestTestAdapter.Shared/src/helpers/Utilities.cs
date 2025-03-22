// Utilities.cs
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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using Microsoft.Win32;
using System.Globalization;
using DoctestTestAdapter.Shared.Profiling;
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
            string path = startingDirectoryPath;

            // No starting path provided.
            if (path == null)
            {
                path = Environment.CurrentDirectory;
            }
            // A file path was provided instead of a directory path.
            else if (!Directory.Exists(path))
            {
                path = Directory.GetParent(startingDirectoryPath).FullName;
            }

            DirectoryInfo directory = new DirectoryInfo(path);

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
        internal static string GetVSInstallDirectory()
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
