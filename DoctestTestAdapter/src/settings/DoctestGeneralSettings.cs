using System.Xml.Serialization;

namespace DoctestTestAdapter.Settings
{
    [XmlType]
    public class DoctestGeneralSettings
    {
        /// <summary>
        /// These arguments will be supplied first before any doctest command arguments are provided.
        /// E.g. with CommandArguments = --test
        /// Example command line would end up being: --test --test-case=*"[TestDecorator] Test 1"*,*"[TestDecorator] Test 2"*
        /// </summary>
        public string CommandArguments { get; set; }
    }
}
