namespace VS.Common.DoctestTestAdapter.IO
{
    public interface IFile
    {
        void Write(string _text);
        string[] ReadAllLines();
    }
}
