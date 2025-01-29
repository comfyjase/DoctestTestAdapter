using EnvDTE;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using VS.Common.DoctestTestAdapter;
using VS2022.DoctestTestAdapter.Settings;

namespace VS2022.DoctestTestAdapter
{
    [ExtensionUri(DoctestTestAdapterConstants.ExecutorUriString)]
    public class DoctestTestExecutor : ITestExecutor
    {
        private bool cancelled = false;

        List<System.Diagnostics.Process> processList = new List<System.Diagnostics.Process>();
        Dictionary<String, List<String>> mappedExecutableTestFiles = new Dictionary<String, List<String>>();
        Dictionary<String, List<String>> mappedTestOutputs = new Dictionary<String, List<String>>();
        Dictionary<String, EventHandler> mappedExitHandlers = new Dictionary<String, EventHandler>();

        public void RunTests(IEnumerable<TestCase> _tests, IRunContext _runContext, IFrameworkHandle _frameworkHandle)
        {
            Logger.Instance.WriteLine("Begin");

            cancelled = false;
            processList.Clear();

            DoctestSettingsProvider doctestSettings = _runContext.RunSettings.GetSettings(DoctestTestAdapterConstants.SettingsName) as DoctestSettingsProvider;
            foreach (TestCase test in _tests)
            {
                if (cancelled)
                {
                    return;
                }

                string executableFilePath = DoctestTestAdapterUtilities.GetTestFileExecutableFilePath(doctestSettings, test.CodeFilePath, out string commandArguments); ;

                //TODO_comfyjase_29/01/2025: Find a way to link dll -> related exe file
                if (Path.GetExtension(executableFilePath).Equals(DoctestTestAdapterConstants.DLLFileExtension))
                {
                    continue;
                }    

                if (mappedExecutableTestFiles.TryGetValue(executableFilePath, out List<String> testFiles))
                {
                    testFiles.Add(test.CodeFilePath);
                    mappedExecutableTestFiles[executableFilePath] = testFiles;
                }
                else
                {
                    List<String> newTestFiles = new List<String>();
                    newTestFiles.Add(test.CodeFilePath);
                    mappedExecutableTestFiles.Add(executableFilePath, newTestFiles);
                }
            }

            foreach (KeyValuePair<String, List<String>> testSetup in mappedExecutableTestFiles)
            {
                if (cancelled)
                {
                    return;
                }

                List<String> testFileNames = testSetup.Value.Select(s => Path.GetFileNameWithoutExtension(s)).ToList();

                System.Diagnostics.Process testExecutable = new System.Diagnostics.Process();
                testExecutable.EnableRaisingEvents = true;
                testExecutable.StartInfo.CreateNoWindow = true;
                testExecutable.StartInfo.RedirectStandardOutput = true;
                testExecutable.StartInfo.RedirectStandardError = true;
                testExecutable.StartInfo.UseShellExecute = false;

                testExecutable.StartInfo.FileName = testSetup.Key;

                // Should be something like: "TestFile, TestFile2"
                string commaSeparatedListOfTestFiles = string.Join(",", testFileNames);

                // Sorted into doctest specific argument formatting: "*TestFile1*,*TestFile2*"
                string doctestSourceFileArgument = string.Join(",", commaSeparatedListOfTestFiles.Split(',').Select(x => string.Format("*{0}*", x)).ToList());

                // Full doctest arguments: --source-file="*TestFile1*,*TestFile2*" --success=true
                string doctestArguments = "--source-file=" + doctestSourceFileArgument + " --success=true";

                testExecutable.StartInfo.Arguments = doctestArguments;

                processList.Add(testExecutable);

                // Start the executable now to run the doctests unit tests
                bool executableStartedSuccessfully = testExecutable.Start();
                Debug.Assert(executableStartedSuccessfully, "Failed to start " + testSetup.Key + " test executable");

                // Reset output strings for this test run
                mappedTestOutputs.Clear();

                mappedTestOutputs.Add(testSetup.Key, new List<String>());
                testExecutable.OutputDataReceived += (object _sender, DataReceivedEventArgs _e) =>
                {
                    if (_e.Data != null && _e.Data.Count() > 0)
                    {
                        Logger.Instance.WriteLine(_e.Data);
                        if (mappedTestOutputs.TryGetValue(testSetup.Key, out List<String> outputStrings))
                        {
                            outputStrings.Add(_e.Data + "\n");
                            mappedTestOutputs[testSetup.Key] = outputStrings;
                        }
                    }
                };
                testExecutable.BeginOutputReadLine();

                EventHandler testExecutableExitEventHandler = (_sender, _e) => OnTestExecutableFinished(_sender, _e, testExecutable);
                testExecutable.Exited += testExecutableExitEventHandler;
                mappedExitHandlers.Add(testSetup.Key, testExecutableExitEventHandler);
            }

            Logger.Instance.WriteLine("End");
        }

        private void OnTestExecutableFinished(object _sender, EventArgs _e, System.Diagnostics.Process _testExecutable)
        {
            if (_testExecutable != null)
            {
                int textExecutableExitCode = _testExecutable.ExitCode;

                if (mappedTestOutputs.TryGetValue(_testExecutable.StartInfo.FileName, out List<String> testOutput))
                {
                    //TODO_comfyjase_28/01/2025: read output and find any output relevant to the _tests?
                    // Create new test result and record it into the _frameworkHandle
                    foreach (String testOuputLine in testOutput)
                    {
                           
                    }
                    mappedTestOutputs.Remove(_testExecutable.StartInfo.FileName);
                }
                
                if (mappedExitHandlers.TryGetValue(_testExecutable.StartInfo.FileName, out EventHandler exitEventHandler))
                {
                    _testExecutable.Exited -= exitEventHandler;
                    mappedExitHandlers.Remove(_testExecutable.StartInfo.FileName);
                }

                mappedExecutableTestFiles.Remove(_testExecutable.StartInfo.FileName);

                processList.Remove(_testExecutable);
                _testExecutable.Close();
                _testExecutable = null;
            }
        }

        public void RunTests(IEnumerable<string> _sources, IRunContext _runContext, IFrameworkHandle _frameworkHandle)
        {
            Logger.Instance.WriteLine("Begin");

            IDiscoveryContext discoveryContext = _runContext;
            IMessageLogger logger = _frameworkHandle;
            ITestCaseDiscoverySink discoverySink = null;
            IEnumerable<TestCase> tests = DoctestTestAdapterUtilities.GetTests(_sources, discoveryContext, logger, discoverySink);
            RunTests(tests, _runContext, _frameworkHandle);

            Logger.Instance.WriteLine("End");
        }

        public void Cancel()
        {
            Logger.Instance.WriteLine("Begin");

            cancelled = true;
            processList.Clear();

            Logger.Instance.WriteLine("End");
        }
    }
}
