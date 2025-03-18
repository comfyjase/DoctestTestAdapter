using Microsoft.VisualStudio.TestPlatform.ObjectModel;
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
    }
}
