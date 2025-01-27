using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VS2022.DoctestTestAdapter.Settings
{
    [SettingsName("DoctestTestAdapter")]
    public class DoctestSettingsProvider : ISettingsProvider
    {
        private List<string> outputFilePaths = new List<string>();

        public void Load(XmlReader reader)
        {
            Debug.Assert(reader != null);

            //bool movedToOutputFilesAttribute = reader.MoveToAttribute();
        }
    }
}
