namespace VS.Common.DoctestTestAdapter
{
    public enum TestFileChangedReason
    {
        None,
        Added,
        Removed,
        Changed,
    }

    public class TestFileChangedEventArgs : System.EventArgs
    {
        public string File { get; private set; }
        public TestFileChangedReason ChangedReason { get; private set; }

        public TestFileChangedEventArgs(string _file, TestFileChangedReason _reason)
        {
            File = _file;
            ChangedReason = _reason;
        }
    }
}
