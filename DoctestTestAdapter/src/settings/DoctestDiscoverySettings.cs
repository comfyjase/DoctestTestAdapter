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
        /// Each entry should be considered relative from the solution directory.
        /// Test discovery will find any files and sub folder/files in these directories only.
        /// E.g. With an example .runsettings file with this in the discovery settings:
        /// <SearchDirectories>
		///	    <string>tests</string>
		/// </SearchDirectories>
        /// And with a solution file located in: Path\To\Solution\
        /// Test discovery will make sure only source code files under 
        /// (and any sub folders within this directory): 
        /// Path\To\Solution\tests\ will be considered.
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
