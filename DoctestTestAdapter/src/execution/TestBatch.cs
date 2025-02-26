using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;

namespace DoctestTestAdapter.Execution
{
    internal class TestBatch
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

        public TestBatch() : this(null, null, -1)
        { }

        public TestBatch(List<TestCase> _tests, string _commandArguments, int _batchNumber)
        {
            Tests = _tests;
            CommandArguments = _commandArguments;
            BatchNumber = _batchNumber;
        }
    }
}
