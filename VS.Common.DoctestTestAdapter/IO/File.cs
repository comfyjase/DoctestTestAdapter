using System;
using System.Diagnostics;
using System.IO;
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

        private Mutex mutex = null;
        private bool mutexExists = false;
        private bool mutexUnauthorized = false;
        private bool mutexCreated = false;

        public File(string _fullPath)
        {
            fullPath = _fullPath;
            Debug.Assert(!string.IsNullOrEmpty(fullPath));

            try
            {
                mutex = Mutex.OpenExisting(fullPath.Replace("\\", ""));
                mutexExists = true;
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                Trace.WriteLine("WaitHandleCannotBeOpenedException is okay, just means the mutex does not exist yet - will attempt to create a new one.");
                mutexExists = false;
            }
            catch (UnauthorizedAccessException ex)
            {
                Trace.WriteLine("Unauthorized access: {0}", ex.Message);
                mutexUnauthorized = true;
            }

            if (!mutexExists)
            {
                mutex = new Mutex(false, fullPath.Replace("\\", ""), out mutexCreated);
                Debug.Assert(mutexCreated);
            }
            else if (mutexUnauthorized)
            {
                try
                {
                    Trace.WriteLine("Should you change permission?");
                }
                catch (UnauthorizedAccessException ex)
                {
                    Trace.WriteLine("Unauthorized access: " + ex.Message);
                    return;
                }
            }

            Trace.WriteLine("Process: " + System.Diagnostics.Process.GetCurrentProcess().ProcessName + " Id: " + System.Diagnostics.Process.GetCurrentProcess().Id);

            // Create parent directory if it doesn't already exist.
            string folder = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            // Create file if it doesn't already exist.
            if (!System.IO.File.Exists(fullPath))
            {
                using (StreamWriter sw = System.IO.File.CreateText(fullPath))
                {
                    Trace.WriteLine("Successfully created file: " + fullPath);
                }
            }
        }

        ~File()
        {
            mutex.Dispose();
        }

        public void Clear()
        {
            System.IO.File.WriteAllText(fullPath, string.Empty);
        }

        public void Write(string _text)
        {
            Trace.WriteLine("Process: " + Process.GetCurrentProcess().ProcessName + " Id: " + Process.GetCurrentProcess().Id + " wants to write to file: " + FileName);
            
            // Is there any lock currently active for this file? If so, wait until the lock is released...
            try
            {
                Trace.WriteLine("Process: " + Process.GetCurrentProcess().ProcessName + " Id: " + Process.GetCurrentProcess().Id + " waiting on mutex to release lock...");
                mutex.WaitOne();
            }
            // If all else fails and the mutex hasn't been released for some reason.
            // Assume file may be corrupted at this point and just clear it.
            catch (AbandonedMutexException)
            {
                Clear();
            }

            // Thread/Process can now write to this file.
            try
            {
                Trace.WriteLine("Process: " + Process.GetCurrentProcess().ProcessName + " Id: " + Process.GetCurrentProcess().Id + " can now write to file: " + FileName);

                using (StreamWriter sw = new StreamWriter(fullPath, true))
                {
                    sw.WriteLine(_text);
                }
            }
            // Once done, make sure to release mutex for another thread/process to be able to write.
            finally
            {
                Trace.WriteLine("Process: " + Process.GetCurrentProcess().ProcessName + " Id: " + Process.GetCurrentProcess().Id + " finished writing to file: " + FileName);
                mutex.ReleaseMutex();
            }
        }

        public string[] ReadAllLines()
        {
            return System.IO.File.ReadAllLines(fullPath);
        }
    }
}
