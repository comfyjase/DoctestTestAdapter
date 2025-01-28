using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace VS2022.DoctestTestAdapter.Settings
{
    [SettingsName(DoctestTestAdapterConstants.DoctestTestAdapterSettingsName)]
    public class DoctestSettingsProvider : ISettingsProvider
    {
        private List<DoctestSettingsOutputFileData> outputFileData = new List<DoctestSettingsOutputFileData>();
        public List<DoctestSettingsOutputFileData> OutputFileData 
        { 
            get { return outputFileData; } 
        }

        public void Load(XmlReader reader)
        {
            Debug.Assert(reader != null);

            while (reader.Read())
            {
                if (reader.Name.Equals("OutputFile", StringComparison.OrdinalIgnoreCase))
                {
                    string projectFilePathAttribute = reader.GetAttribute("ProjectFilePath");
                    string filePathAttribute = reader.GetAttribute("FilePath");
                    string commandArgumentsAttribute = reader.GetAttribute("CommandArguments");

                    // Note, not providing a check for command arguments because these should be optional.
                    if (!string.IsNullOrEmpty(filePathAttribute))
                    {
                        DoctestSettingsOutputFileData fileData = new DoctestSettingsOutputFileData(projectFilePathAttribute, filePathAttribute, commandArgumentsAttribute);

                        if (!outputFileData.Contains(fileData))
                        {
                            outputFileData.Add(fileData);
                        }
                    }
                }
            }
        }
    }
}
