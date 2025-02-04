using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace VS.Common.DoctestTestAdapter.IO
{
    public class File : IFile
    {
        protected string fullPath = string.Empty;
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

        public File(string _fullPath)
        {
            fullPath = _fullPath;
            Debug.Assert(!string.IsNullOrEmpty(fullPath));

            Trace.WriteLine("Process: " + System.Diagnostics.Process.GetCurrentProcess().ProcessName + " Id: " + System.Diagnostics.Process.GetCurrentProcess().Id);

            InitializeDirectory();
            InitializeFile();
        }

        protected void InitializeDirectory()
        {
            // Create parent directory if it doesn't already exist.
            string folder = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        virtual protected void InitializeFile()
        {
            // Create file if it doesn't already exist.
            if (!System.IO.File.Exists(fullPath))
            {
                using (StreamWriter sw = System.IO.File.CreateText(fullPath))
                {
                    Trace.WriteLine("Successfully created file: " + fullPath);
                }
            }
        }

        public void Clear()
        {
            Trace.WriteLine("Process: " + Process.GetCurrentProcess().ProcessName + " Id: " + Process.GetCurrentProcess().Id + " clearing file: " + FileName);
            System.IO.File.WriteAllText(fullPath, string.Empty);
        }

        public void WriteLine(string _text)
        {
            Trace.WriteLine("Process: " + Process.GetCurrentProcess().ProcessName + " Id: " + Process.GetCurrentProcess().Id + " wants to write to file: " + FileName);

            Action<Mutex> writeToFile = (Mutex _mutex) =>
            {
                try
                {
                    Trace.WriteLine("Process: " + Process.GetCurrentProcess().ProcessName + " Id: " + Process.GetCurrentProcess().Id + " waiting on mutex to release lock...");
                    _mutex.WaitOne();
                }
                // If all else fails and the mutex hasn't been released for some reason.
                // Assume file may be corrupted at this point and just clear it.
                catch (AbandonedMutexException)
                {
                    Trace.WriteLine("Process: " + Process.GetCurrentProcess().ProcessName + " Id: " + Process.GetCurrentProcess().Id + " clearing file because of AbandonedMutexException");
                    Clear();
                }

                // Thread/Process can now write to this file.
                try
                {
                    Trace.WriteLine("Process: " + Process.GetCurrentProcess().ProcessName + " Id: " + Process.GetCurrentProcess().Id + " can now write to file: " + FileName);

                    using (StreamWriter sw = System.IO.File.AppendText(fullPath))
                    {
                        sw.WriteLine(_text);
                    }
                }
                // Once done, make sure to release mutex for another thread/process to be able to write.
                catch (Exception ex)
                {
                    Trace.WriteLine("Exception: " + ex.Message);
                }
            };

            bool mutexAlreadyExists = false;

            try
            {
                using (Mutex mutex = Mutex.OpenExisting(fullPath.Replace("\\", "")))
                {
                    mutexAlreadyExists = true;
                    writeToFile(mutex);

                    Trace.WriteLine("Process: " + Process.GetCurrentProcess().ProcessName + " Id: " + Process.GetCurrentProcess().Id + " releasing mutex for " + Path.GetFileName(fullPath));
                    mutex.ReleaseMutex();
                }
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                mutexAlreadyExists = false;
            }

            if (!mutexAlreadyExists)
            {
                using (Mutex mutex = new Mutex(false, fullPath.Replace("\\", ""), out bool mutexCreated))
                {
                    writeToFile(mutex);

                    Trace.WriteLine("Process: " + Process.GetCurrentProcess().ProcessName + " Id: " + Process.GetCurrentProcess().Id + " releasing mutex for " + Path.GetFileName(fullPath));
                    mutex.ReleaseMutex();
                }
            }
        }

        public string[] ReadAllLines()
        {
            return System.IO.File.ReadAllLines(fullPath);
        }
    }
}
