using DoctestTestAdapter.Shared.Helpers;
using System.Collections.Generic;
using System.IO;
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
        public List<ExecutableOverride> ExecutableOverrides { get; set; } = new List<ExecutableOverride>();

        public bool AreExecutableOverridesValid(string solutionDirectory, out string message)
        {
            message = string.Empty;

            foreach (ExecutableOverride executableOverride in ExecutableOverrides)
            {
                bool relativeKeyPathIsValid = false;
                bool absoluteKeyPathIsValid = false;
                bool relativeValuePathIsValid = false;
                bool absoluteValuePathIsValid = false;

                string keyFilePath = executableOverride.Key;
                string valueFilePath = executableOverride.Value;

                // Relative to the solution directory.
                string relativePath = solutionDirectory + "\\" + keyFilePath;
                if (File.Exists(relativePath))
                {
                    relativeKeyPathIsValid = true;
                }
                // Otherwise, consider it an absolute path.
                if (File.Exists(keyFilePath))
                {
                    absoluteKeyPathIsValid = true;
                }

                relativePath = solutionDirectory + "\\" + valueFilePath;
                if (File.Exists(relativePath))
                {
                    relativeValuePathIsValid = true;
                }
                if (File.Exists(valueFilePath))
                {
                    absoluteValuePathIsValid = true;
                }

                // If both relative and absolute paths are invalid, report this.
                if (!relativeKeyPathIsValid && !absoluteKeyPathIsValid)
                {
                    message += (Constants.WarningMessagePrefix + " - Executable override key in the .runsettings file: " + keyFilePath + " doesn't exist relative to the solution directory or as an absolute path. To fix, please make sure a file named " + keyFilePath + " exists under " + solutionDirectory + " or make sure it exists as an absolute path on your system.");
                }
                if (!relativeValuePathIsValid && !absoluteValuePathIsValid)
                {
                    message += (Constants.WarningMessagePrefix + " - Executable override value in the .runsettings file: " + valueFilePath + " doesn't exist relative to the solution directory or as an absolute path. To fix, please make sure a directory named " + valueFilePath + " exists under " + solutionDirectory + " or make sure it exists as an absolute path on your system.");
                }
            }

            return string.IsNullOrEmpty(message);
        }
    }
}
