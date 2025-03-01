using System.Collections.Generic;
using System.Xml.Serialization;

namespace DoctestTestAdapter.Settings
{
    /// <summary>
    /// Overrides should be relative from solution directory.
    /// E.g. with settings:
    /// <ExecutableOverrides>
	///		<ExecutableOverride>
	///			<Key>bin\app.exe</Key>
	///			<Value>bin\app.console.exe</Value>
	///		</ExecutableOverride>
	///	</ExecutableOverrides>
    ///	And solution directory: Path\To\Solution\
    ///	This assumes the executables are located Path\To\Solution\bin\app.exe and Path\To\Solution\bin\app.console.exe
    /// </summary>
    [XmlType]
    public struct ExecutableOverride
    {
        public string Key
        { get; set; }

        public string Value
        { get; set; }

        public ExecutableOverride(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    [XmlType]
    public class DoctestExecutorSettings
    {
        /// <summary>
        /// These arguments will be supplied first before any doctest command arguments are provided.
        /// E.g. with CommandArguments = --test
        /// Example command line would end up being: --test --test-case=*"[TestDecorator] Test 1"*,*"[TestDecorator] Test 2"*
        /// </summary>
        public string CommandArguments { get; set; }

        public List<ExecutableOverride> ExecutableOverrides { get; set; } = new List<ExecutableOverride>();
    }
}
