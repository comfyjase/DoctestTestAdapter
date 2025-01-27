using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using VS.Common.DoctestTestAdapter;

namespace VS2022.DoctestTestAdapter.Settings
{
    [SettingsName("DoctestTestAdapter")]
    public class DoctestSettingsProvider : ISettingsProvider
    {
        private List<DoctestSettingsOutputFileData> outputFileData = new List<DoctestSettingsOutputFileData>();

        public void Load(XmlReader reader)
        {
            Debug.Assert(reader != null);

            while (reader.Read())
            {
                if (reader.Name.Equals("OutputFile", StringComparison.OrdinalIgnoreCase))
                {
                    //Logger.Instance.WriteLine("Node Name: " + reader.Name + " Node Type: " + reader.NodeType.ToString() + " Node Value: " + reader.Value);

                    string filePathAttribute = reader.GetAttribute("FilePath");
                    string commandArgumentsAttribute = reader.GetAttribute("CommandArguments");

                    // Note, not providing a check for command arguments because these should be optional.
                    if (!string.IsNullOrEmpty(filePathAttribute))
                    {
                        DoctestSettingsOutputFileData fileData = new DoctestSettingsOutputFileData(filePathAttribute, commandArgumentsAttribute);

                        if (!outputFileData.Contains(fileData))
                        {
                            outputFileData.Add(fileData);

                            Logger.Instance.WriteLine("Stored output file data FilePath: " + fileData.FilePath + " CommandArguments: " + fileData.CommandArguments);
                        }
                    }
                }
            }
        }
    }
}
