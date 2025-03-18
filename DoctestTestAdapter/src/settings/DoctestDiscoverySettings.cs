﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace DoctestTestAdapter.Settings
{
    [XmlType]
    public class DoctestDiscoverySettings
    {
        /// <summary>
        /// These arguments will be supplied first before any doctest command arguments are provided.
        /// E.g. with CommandArguments = --test
        /// Example command line would end up being: --test --test-case=*"[TestDecorator] Test 1"*,*"[TestDecorator] Test 2"*
        /// </summary>
        public string CommandArguments { get; set; }

        /// <summary>
        /// Each entry should be considered relative from the solution directory.
        /// Test discovery will find any files and sub folder/files in these directories only if specified.
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
    }
}
