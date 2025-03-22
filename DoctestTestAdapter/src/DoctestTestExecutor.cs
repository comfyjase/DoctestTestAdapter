// DoctestTestExecutor.cs
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
using DoctestTestAdapter.Shared.Executables;
using DoctestTestAdapter.Shared.Factory;
using DoctestTestAdapter.Shared.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;

using Helpers = DoctestTestAdapter.Shared.Helpers;

namespace DoctestTestAdapter
{
    [ExtensionUri(Helpers.Constants.ExecutorUriString)]
    public sealed class DoctestTestExecutor : ITestExecutor
    {
        private string _solutionDirectory = null;
        private bool _cancelled = false; 
        private bool _waitingForTestResults = false;
        private int _currentNumberOfTestBatches = 0;
        private List<DoctestExecutable> _testExecutables = new List<DoctestExecutable>();

        private void OnTestExecutableFinished(object sender, System.EventArgs e)
        {
            _currentNumberOfTestBatches--;

            // No more test executables to run.
            if (_currentNumberOfTestBatches == 0)
            {
                OnAllTestExecutablesFinished();
            }
        }

        private void OnAllTestExecutablesFinished()
        {
            _testExecutables.Clear();
            _waitingForTestResults = false;
        }

        public void Cancel()
        {
            _cancelled = true;
            _waitingForTestResults = false;
            _currentNumberOfTestBatches = 0;
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            Utilities.CheckEnumerable(tests, nameof(tests));
            Utilities.CheckNull(runContext, nameof(runContext));
            Utilities.CheckNull(frameworkHandle, nameof(frameworkHandle));

            _waitingForTestResults = true;
            _currentNumberOfTestBatches = 0;
            _testExecutables.Clear();

            try
            {
                DoctestTestSettings settings = DoctestTestSettingsProvider.LoadSettings(runContext);

                foreach (TestCase test in tests)
                {
                    if (string.IsNullOrEmpty(_solutionDirectory))
                    {
                        _solutionDirectory = Utilities.GetSolutionDirectory(test.Source);
                    }

                    DoctestExecutable existingTestExecutable = _testExecutables.Find(t => (t.FilePath.Equals(test.Source)));
                    if (existingTestExecutable != null)
                    {
                        existingTestExecutable.TrackTestCase(test);
                    }
                    else
                    {
                        DoctestExecutable newTestExecutable = new DoctestExecutable(test.Source, _solutionDirectory, settings, runContext, frameworkHandle, frameworkHandle);
                        newTestExecutable.TrackTestCase(test);
                        _testExecutables.Add(newTestExecutable);
                    }
                }

                foreach (DoctestExecutable testExecutable in _testExecutables)
                {
                    testExecutable.SetupBatches(tests);
                    _currentNumberOfTestBatches += testExecutable.NumberOfTestBatches;

                    testExecutable.Finished += OnTestExecutableFinished;
                    testExecutable.RunUnitTests();
                }

                //TODO_comfyjase_03/02/2025: Check if you still need this.
                while (_waitingForTestResults)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
            catch(Exception ex)
            {
                frameworkHandle.SendMessage(TestMessageLevel.Error, Helpers.Constants.ErrorMessagePrefix + $"[Test Executing]: {ex}");
            }
            finally
            {
                _waitingForTestResults = false;
                _currentNumberOfTestBatches = 0;
                _testExecutables.Clear();
            }
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            Utilities.CheckEnumerable(sources, nameof(sources));
            Utilities.CheckNull(runContext, nameof(runContext));
            Utilities.CheckNull(frameworkHandle, nameof(frameworkHandle));

            IDiscoveryContext discoveryContext = runContext;

            DoctestTestSettings settings = DoctestTestSettingsProvider.LoadSettings(runContext);

            List<TestCase> tests = new List<TestCase>();
            foreach (string source in sources)
            {
                if (_cancelled)
                {
                    return;
                }

                if (string.IsNullOrEmpty(_solutionDirectory))
                {
                    _solutionDirectory = Utilities.GetSolutionDirectory(source);
                }

                TestCaseFactory testCaseFactory = new TestCaseFactory(source, settings, runContext, frameworkHandle);
                List<TestCase> sourceTestCases = testCaseFactory.CreateTestCases();
                if (sourceTestCases == null || sourceTestCases.Count == 0)
                    continue;

                tests.AddRange(sourceTestCases);
            }

            RunTests(tests, runContext, frameworkHandle);
        }
    }
}
