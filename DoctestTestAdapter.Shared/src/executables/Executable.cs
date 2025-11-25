// Executable.cs
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
using System.Diagnostics;
using System.IO;

namespace DoctestTestAdapter.Shared.Executables
{
    internal class Executable
    {
        internal string FilePath { get; private set; } = null;
        internal string Output { get; private set; } = null;
        internal string Arguments { get; set; } = null;
        internal DoctestTestSettings Settings { get; private set; } = null;
        internal string RootDirectory { get; private set; } = null;
        internal IRunContext RunContext { get; private set; } = null;
        internal IMessageLogger Logger { get; private set; } = null;
        internal IFrameworkHandle FrameworkHandle { get; private set; } = null;

        private Process _process = null;

        internal Executable(string filePath, string rootDirectory, DoctestTestSettings settings, IRunContext runContext, IMessageLogger logger, IFrameworkHandle frameworkHandle = null) : this(filePath, rootDirectory, null, settings, runContext, logger, frameworkHandle)
        { }

        /// <summary>
        /// Executable Constructor.
        /// </summary>
        /// <param name="filePath">Full file path to the exe to run.</param>
        /// <param name="rootDirectory">Full file path to the root directory.</param>
        /// <param name="arguments">Command arguments to use.</param>
        /// <param name="settings">Doctest test adapter settings.</param>
        /// <param name="logger"></param>
        /// <exception cref="FileNotFoundException">Thrown if filePath doesn't exist.</exception>
        internal Executable(string filePath, string rootDirectory, string arguments, DoctestTestSettings settings, IRunContext runContext, IMessageLogger logger, IFrameworkHandle frameworkHandle)
        {
            if (!filePath.Equals(@"cmd.exe") && !File.Exists(filePath))
                throw new FileNotFoundException($"Could not find file {filePath}, abort!");

            FilePath = filePath;
            RootDirectory = rootDirectory;
            Arguments = arguments;
            Settings = settings;
            RunContext = runContext;
            Logger = logger;
            FrameworkHandle = frameworkHandle;
        }

        internal void Start(bool waitForExit = true, string exeOverridePath = null)
        {
            System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo();
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.FileName = string.Format("\"{0}\"", (string.IsNullOrEmpty(exeOverridePath) ? FilePath : exeOverridePath));
            processStartInfo.Arguments = Arguments;
            processStartInfo.WorkingDirectory = RootDirectory;

            if (waitForExit)
            {
                using (_process = new System.Diagnostics.Process())
                {
                    _process.StartInfo = processStartInfo;
                    _process.Start();

                    Output = _process.StandardOutput.ReadToEnd();
                    string errors = _process.StandardError.ReadToEnd();
                    if (!string.IsNullOrEmpty(errors))
                        Console.Error.WriteLine(Path.GetFileName(FilePath) + " errors: \n\t" + errors);

                    if (!string.IsNullOrEmpty(Output))
                    {
                        if (ShouldLogOutput() && Logger != null)
                        {
                            Logger.SendMessage(TestMessageLevel.Informational, Constants.InformationMessagePrefix + " " + Path.GetFileName(FilePath) + " output: \n" + Output);
                        }
                    }

                    _process.WaitForExit();
                }
            }
            else if (RunContext.IsBeingDebugged && FrameworkHandle != null)
            {
                int processId = FrameworkHandle.LaunchProcessWithDebuggerAttached(FilePath, RootDirectory, Arguments, null);
                _process = Process.GetProcessById(processId) ?? throw new NullReferenceException($"Failed to start process {FilePath} with debugger attached - _process is null, abort!");
                _process.EnableRaisingEvents = true;
                _process.Exited += ProcessExited;
            }
            else
            {
                _process = new System.Diagnostics.Process();
                _process.EnableRaisingEvents = true;
                _process.StartInfo = processStartInfo;

                _process.Exited += ProcessExited;

                _process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        if (ShouldLogOutput() && Logger != null)
                        {
                            Logger.SendMessage(TestMessageLevel.Informational, Constants.InformationMessagePrefix + " " + e.Data);
                        }
                    }
                });

                _process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Console.Error.WriteLine(e.Data);
                    }
                });

                _process.Start();

                _process.BeginOutputReadLine();
                _process.BeginErrorReadLine();
            }
        }

        protected virtual void OnProcessExited(object sender, EventArgs e)
        { }

        private void ProcessExited(object sender, EventArgs e)
        {
            if (_process != null)
            {
                _process.Exited -= ProcessExited;
                _process.Close();
                _process.Dispose();
                _process = null;

                OnProcessExited(sender, e);
            }
        }

        protected virtual bool ShouldLogOutput()
        {
            return (Settings != null && Settings.TryGetEnableDebugLogs(out bool enableDebugLogs) && enableDebugLogs);
        }
    }
}
