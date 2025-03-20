using DoctestTestAdapter.Shared.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;

namespace DoctestTestAdapter.Execution
{
    internal class DoctestTestBatch
    {
        public List<TestCase> Tests
        {
            get; private set;
        }

        public string CommandArguments
        {
            get; private set;
        }

        public int BatchNumber
        {
            get; private set;
        }

        public string TestReportFilePath
        {
            get; private set;
        }

        public DoctestTestBatch(List<TestCase> tests, string commandArguments, int batchNumber, string testReportFilePath)
        {
            Utilities.CheckEnumerable(tests, nameof(tests));
            Utilities.CheckString(commandArguments, nameof(commandArguments));
            Utilities.CheckString(testReportFilePath, nameof(testReportFilePath));

            Tests = tests;
            CommandArguments = commandArguments;
            BatchNumber = batchNumber;
            TestReportFilePath = testReportFilePath;
        }

        public override string ToString()
        {
            return "TestBatch: Number = " + BatchNumber + " Number of Tests = " + Tests.Count + " Command Argument Length = " + CommandArguments.Length + " Test Report File Path: " + TestReportFilePath;
        }
    }
}
