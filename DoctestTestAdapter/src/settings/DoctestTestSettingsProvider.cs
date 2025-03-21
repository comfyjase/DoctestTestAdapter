// DoctestTestSettingsProvider.cs
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
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Xml;
using System.Xml.Serialization;

namespace DoctestTestAdapter.Settings
{
    [SettingsName(DoctestTestSettings.RunSettingsXmlNode)]
    public sealed class DoctestTestSettingsProvider : ISettingsProvider
    {
        private XmlSerializer Serializer = new XmlSerializer(typeof(DoctestTestSettings));

        public DoctestTestSettings Settings { get; private set; } = new DoctestTestSettings();

        public void Load(XmlReader reader)
        {
            try
            {
                if (reader.Read() && reader.Name == DoctestTestSettings.RunSettingsXmlNode)
                {
                    DoctestTestSettings settings = Serializer.Deserialize(reader) as DoctestTestSettings;
                    Settings = settings ?? new DoctestTestSettings();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Loading DoctestTestAdapter settings failed! {e}");
            }
        }

        public static DoctestTestSettings LoadSettings(IDiscoveryContext discoveryContext)
        {
            DoctestTestSettingsProvider doctestTestSettingsProvider = discoveryContext.RunSettings?.GetSettings(DoctestTestSettings.RunSettingsXmlNode) as DoctestTestSettingsProvider;
            return doctestTestSettingsProvider?.Settings ?? new DoctestTestSettings();
        }
    }
}
