using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VS.Common.DoctestTestAdapter.IO;

namespace VS.Common.DoctestTestAdapter.Options
{
    public class TestAdapterOptions : ITestAdapterOptions
    {
        // General options
        private GeneralOptionsPage generalOptionsPage = null;
        private bool enableLogging = false;
        private string commandArguments = string.Empty;
        private string testExecutableFilePath = string.Empty;

        private XmlFile optionsFile = null;

        public TestAdapterOptions() : this(null)
        { }

        //TODO_comfyjase_08/02/2025: Add any other option pages here...
        public TestAdapterOptions(GeneralOptionsPage _generalOptionsPage)
        {
            generalOptionsPage = _generalOptionsPage;
            Debug.Assert(generalOptionsPage != null);

            optionsFile = new XmlFile(VS.Common.DoctestTestAdapter.Constants.Options.FilePath);

            generalOptionsPage.PropertyChanged += GeneralOptionsPage_PropertyChanged;

            // Update after all option pages are stored.
            Update();
        }

        private void GeneralOptionsPage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Update();
        }

        public void Update()
        {
            Debug.Assert(generalOptionsPage != null);

            string textToWriteToOptionsFile = 
            (
                "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n"
                + "<" + Constants.XmlNodeNames.Root + ">\n"
                + "\t<" + Constants.XmlNodeNames.Options + ">\n"
            );

            enableLogging = generalOptionsPage.EnableLogging;
            commandArguments = generalOptionsPage.CommandArguments;
            testExecutableFilePath = generalOptionsPage.TestExecutablePath;

            generalOptionsPage.SaveSettingsToStorage();

            textToWriteToOptionsFile +=
            (   
                "\t\t<" + Constants.XmlNodeNames.GeneralOptions + ">\n"
                + "\t\t\t<" + Constants.XmlNodeNames.EnableLogging + " Value=\"" + enableLogging.ToString() + "\"/>\n"
                + "\t\t\t<" + Constants.XmlNodeNames.CommandArguments + " Value=\"" + commandArguments + "\"/>\n"
                + "\t\t\t<" + Constants.XmlNodeNames.TestExecutableFilePath + " Value=\"" + testExecutableFilePath + "\"/>\n"
                + "\t\t</" + Constants.XmlNodeNames.GeneralOptions + ">\n"
            );

            // Any other option pages here too...

            textToWriteToOptionsFile +=
            (
                "\t</" + Constants.XmlNodeNames.Options + ">\n"
                + "</" + Constants.XmlNodeNames.Root + ">\n"
            );

            optionsFile.OverwriteFile(textToWriteToOptionsFile);

            Logger.Instance.WriteLine("Wrote options to file: " + optionsFile.FullPath);
        }

        public bool EnableLogging
        {
            get { return enableLogging; }
        }

        public string CommandArguments
        {
            get { return commandArguments; }
        }

        public string TestExecutableFilePath
        {
            get { return testExecutableFilePath; }
        }
    }
}
