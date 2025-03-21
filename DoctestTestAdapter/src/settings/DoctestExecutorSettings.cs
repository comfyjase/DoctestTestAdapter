// DoctestExecutorSettings.cs
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

using DoctestTestAdapter.Shared.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace DoctestTestAdapter.Settings
{
    /// <summary>
    /// These can be absolute file paths or relative file paths from the solution directory.
    /// E.g. with settings:
    /// <ExecutableOverrides>
    ///		<ExecutableOverride>
    ///			<Key>bin\app.exe</Key>
    ///			<Value>bin\app.console.exe</Value>
    ///		</ExecutableOverride>
    ///	</ExecutableOverrides>
    ///	Will check for Path\To\Solution\bin\app.exe and bin\app.exe - whichever is first found and valid.
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
