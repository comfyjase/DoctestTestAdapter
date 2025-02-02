namespace VS.Common.DoctestTestAdapter.IO
{
    public interface IFile
    {
        void WriteLine(string _text);
        string[] ReadAllLines();
    }
}
