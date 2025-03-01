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

        public DoctestTestBatch() : this(null, null, -1)
        { }

        public DoctestTestBatch(List<TestCase> _tests, string _commandArguments, int _batchNumber)
        {
            Tests = _tests;
            CommandArguments = _commandArguments;
            BatchNumber = _batchNumber;
        }

        public override string ToString()
        {
            return "TestBatch: Number = " + BatchNumber + " Number of Tests = " + Tests.Count + " Command Argument Length = " + CommandArguments.Length;
        }
    }
}
