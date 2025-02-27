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
