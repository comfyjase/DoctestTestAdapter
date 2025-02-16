using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml;

namespace VS.Common.DoctestTestAdapter.IO
{
    public class XmlFile : VS.Common.DoctestTestAdapter.IO.File
    {
        private XmlDocument xmlDocument = null;
        private string initialElementName = "DoctestTestAdapter";

        public XmlDocument XmlDocument
        { 
            get { return xmlDocument; } 
        }

        public XmlFile(string _fullPath) : base(_fullPath)
        {
            Debug.Assert(Path.GetExtension(_fullPath).Equals(".xml", StringComparison.OrdinalIgnoreCase));
        }

        override protected void InitializeFile()
        {
            xmlDocument = new XmlDocument();

            if (System.IO.File.Exists(fullPath))
            {
                xmlDocument.Load(fullPath);
            }
            else
            {
                // Root node: <?xml version="1.0" encoding="utf-8" ?>
                XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = xmlDocument.DocumentElement;
                xmlDocument.InsertBefore(xmlDeclaration, root);

                XmlElement doctestRootNode = xmlDocument.CreateElement(initialElementName);
                doctestRootNode.IsEmpty = false;
                xmlDocument.AppendChild(doctestRootNode);

                // Save to the given filepath
                xmlDocument.Save(fullPath);
            }
        }

        public void OverwriteFile(string _text)
        {
            Clear();
            WriteAllText(_text);
        }

        public void WriteAllText(string _text)
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
                    System.IO.File.WriteAllText(fullPath, _text);
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

        public void InsertAfter(string _insertAfterThisString, string _text)
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

                    string xmlAllText = System.IO.File.ReadAllText(fullPath);
                    //string insertAfterThisString = "<" + initialElementName + ">";
                    int index = xmlAllText.IndexOf(_insertAfterThisString) + _insertAfterThisString.Length;
                    xmlAllText = xmlAllText.Insert(index, _text);
                    System.IO.File.WriteAllText(fullPath, xmlAllText);
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
            catch(WaitHandleCannotBeOpenedException)
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
    }
}
