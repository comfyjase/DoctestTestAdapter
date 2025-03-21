// DoctestDiscoverySettings.cs
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
    [XmlType]
    public class DoctestDiscoverySettings
    {
        /// <summary>
        /// These can be absolute file paths or relative file paths from the solution directory.
        /// Test discovery will find any files and sub folder/files in these directories only.
        /// E.g. With an example .runsettings file with this in the discovery settings:
        /// <SearchDirectories>
		///	    <string>tests</string>
		/// </SearchDirectories>
        /// This will check Path\To\Solution\tests\ and tests\ whichever is first found and valid.
        /// </summary>
        public List<string> SearchDirectories { get; set; } = new List<string>();

        public bool AreSearchDirectoriesValid(string solutionDirectory, out string message)
        {
            message = string.Empty;

            foreach (string searchDirectory in SearchDirectories)
            {
                bool relativePathIsValid = false;
                bool absolutePathIsValid = false;

                // Relative to the solution directory.
                string relativePath = solutionDirectory + "\\" + searchDirectory + "\\";
                if (Directory.Exists(relativePath))
                {
                    relativePathIsValid = true;
                }
                // Otherwise, consider it an absolute path.
                if (Directory.Exists(searchDirectory))
                {
                    absolutePathIsValid = true;    
                }

                // If both relative and absolute paths are invalid, report this.
                if (!relativePathIsValid && !absolutePathIsValid)
                {
                    message += (Constants.WarningMessagePrefix + " - Search directory in the .runsettings file: " + searchDirectory + " doesn't exist relative to the solution directory or as an absolute path. To fix, please make sure a directory named " + searchDirectory + " exists under " + solutionDirectory + " or make sure it exists as an absolute path on your system.");
                }
            }

            return string.IsNullOrEmpty(message);
        }
    }
}
