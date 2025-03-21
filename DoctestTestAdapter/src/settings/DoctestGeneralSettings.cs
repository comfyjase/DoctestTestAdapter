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
        public string CommandArguments { get; set; } = string.Empty;

        /// <summary>
        /// This will be used to determine if the user wants to see any console output from this test adapter.
        /// Might be useful for debugging.
        /// </summary>
        public bool PrintStandardOutput { get; set; } = false;

        //TODO_comfyjase_21/03/2025: PrintTestOutput setting
        // This would be useful to be able to only print relevant doctest output to the test explorer window (helpful for debugging tests).
        // Tried to implement this but it seems a bit trickier because I need the test results to dump into an xml file to record the relevant information.
        // If I try running --reporter=console,xml --out=testResult.xml then it seems to print the console output into the xml file.
        // If I try running --reporter=console,xml then it seems to print all of the xml data to the console output.
        // For what I have in mind, I need to be able to run an xml reporter and console reporter at the same time and produce those outputs separately - or something along those lines.
    }
}
