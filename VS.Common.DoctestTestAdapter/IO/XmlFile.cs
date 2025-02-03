using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;

namespace VS.Common.DoctestTestAdapter.IO
{
    public class XmlFile : VS.Common.DoctestTestAdapter.IO.File
    {
        private XmlDocument xmlDocument = null;
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

                XmlElement doctestRootNode = xmlDocument.CreateElement("DiscoveredExecutables");
                doctestRootNode.IsEmpty = false;
                xmlDocument.AppendChild(doctestRootNode);

                // Save to the given filepath
                xmlDocument.Save(fullPath);
            }
        }

        public void BatchWrite(string _text)
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

                string xmlAllText = System.IO.File.ReadAllText(fullPath);
                string insertAfterThisString = "<DiscoveredExecutables>";
                int index = xmlAllText.IndexOf(insertAfterThisString) + insertAfterThisString.Length;
                xmlAllText = xmlAllText.Insert(index, _text);
                System.IO.File.WriteAllText(fullPath, xmlAllText);
            }
            // Once done, make sure to release mutex for another thread/process to be able to write.
            finally
            {
                Trace.WriteLine("Process: " + Process.GetCurrentProcess().ProcessName + " Id: " + Process.GetCurrentProcess().Id + " finished writing to file: " + FileName);
                mutex.ReleaseMutex();
            }
        }
    }
}
