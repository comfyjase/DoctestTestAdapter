using VS.Common.DoctestTestAdapter.Executables;

namespace VS.Common.DoctestTestAdapter.EventArgs
{
    public class TestExecutableFinishedEventArgs : System.EventArgs
    {
        public TestExecutable Executable
        {
            get;
            private set;
        }

        public TestExecutableFinishedEventArgs(TestExecutable _executable)
        {
            Executable = _executable;
        }
    }
}
