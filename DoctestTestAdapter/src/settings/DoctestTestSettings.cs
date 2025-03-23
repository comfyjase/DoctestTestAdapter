// DoctestTestSettings.cs
//
// Copyright (c) 2025-present Jase Mottershead
//
// MIT License
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace DoctestTestAdapter.Settings
{
    [XmlRoot(RunSettingsXmlNode)]
    public class DoctestTestSettings : TestRunSettings
    {
        public const string RunSettingsXmlNode = "Doctest";
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(DoctestTestSettings));

        public DoctestGeneralSettings GeneralSettings { get; set; } = new DoctestGeneralSettings();

        public DoctestDiscoverySettings DiscoverySettings { get; set; } = new DoctestDiscoverySettings();

        public DoctestExecutorSettings ExecutorSettings { get; set; } = new DoctestExecutorSettings();

        public DoctestTestSettings() : base(RunSettingsXmlNode)
        {}

        public override XmlElement ToXml()
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                Serializer.Serialize(stringWriter, this);

                XmlDocument document = new XmlDocument();
                document.LoadXml(stringWriter.ToString());

                return document.DocumentElement;
            }

            throw new XmlException("Failed to serialize DoctestTestSettings to xml, aborting!");
        }

        //
        // Settings Helper Functions
        //

        // General Settings
        public bool TryGetCommandArguments(out string commandArguments)
        {
            if (GeneralSettings != null && !string.IsNullOrEmpty(GeneralSettings.CommandArguments))
            {
                commandArguments = GeneralSettings.CommandArguments;
                return true;
            }
            commandArguments = null;
            return false;
        }

        public bool TryGetPrintStandardOutput(out bool printStandardOutput)
        {
            if (GeneralSettings != null)
            {
                printStandardOutput = GeneralSettings.PrintStandardOutput;
                return true;
            }
            printStandardOutput = false;
            return false;
        }

        // Discovery Settings
        public bool TryGetSearchDirectories(out List<string> searchDirectories)
        {
            if (DiscoverySettings != null && DiscoverySettings.SearchDirectories != null && DiscoverySettings.SearchDirectories.Count > 0)
            {
                searchDirectories = DiscoverySettings.SearchDirectories;
                return true;
            }
            searchDirectories = null;
            return false;
        }

        // Executor Settings
        public bool TryGetExecutableOverrides(out List<ExecutableOverride> executableOverrides)
        {
            if (ExecutorSettings != null && ExecutorSettings.ExecutableOverrides != null && ExecutorSettings.ExecutableOverrides.Count > 0)
            {
                executableOverrides = ExecutorSettings.ExecutableOverrides;
                return true;
            }
            executableOverrides = null;
            return false;
        }
    }
}
