using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace VS.Common.DoctestTestAdapter.IO
{
    public class File : IFile
    {
        private string fullPath = string.Empty;
        public string FullPath
        {
            get { return fullPath; }
        }
        public string FileName
        {
            get { return Path.GetFileName(fullPath); }
        }
        public string FileExtension
        {
            get { return Path.GetExtension(fullPath); }
        }

        private ReaderWriterLockSlim readWriteLock = new ReaderWriterLockSlim();

        public File(string _fullPath)
        {
            fullPath = _fullPath;
            Debug.Assert(!string.IsNullOrEmpty(fullPath));

            // Create parent directory if it doesn't already exist.
            string folder = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            // Create file if it doesn't already exist.
            if (!System.IO.File.Exists(fullPath))
            {
                System.IO.File.Create(fullPath);
            }
        }

        public void Write(string _text)
        {
            readWriteLock.EnterWriteLock();
            try
            {
                using (StreamWriter sw = new StreamWriter(fullPath, true))
                {
                    sw.WriteLine(_text);
                }
            }
            finally
            {
                readWriteLock.ExitWriteLock();
            }
        }

        public string[] ReadAllLines()
        {
            return System.IO.File.ReadAllLines(fullPath);
        }
    }
}
