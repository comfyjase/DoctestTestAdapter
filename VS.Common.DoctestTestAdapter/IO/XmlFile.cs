using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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

        public void Write(string _text)
        {

        }
    }
}
