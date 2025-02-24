using System.Diagnostics;
using VS.Common.DoctestTestAdapter.IO;

namespace VS.Common.DoctestTestAdapter.Options
{
    public class TestAdapterOptions : ITestAdapterOptions
    {
        private string solutionDirectory = string.Empty;

        // General options
        private GeneralOptionsPage generalOptionsPage = null;
        private bool enableLogging = false;
        private string commandArguments = string.Empty;
        private string testExecutableFilePath = string.Empty;

        private XmlFile optionsFile = null;

        public TestAdapterOptions() : this(null, null)
        { }

        //TODO_comfyjase_08/02/2025: Add any other option pages here...
        public TestAdapterOptions(string _solutionDirectory, GeneralOptionsPage _generalOptionsPage)
        {
            solutionDirectory = _solutionDirectory;

            generalOptionsPage = _generalOptionsPage;
            Debug.Assert(generalOptionsPage != null);

            //public static readonly string FilePath = Directory.GetCurrentDirectory() + "\\DoctestTestAdapter\\Options" + XmlFileExtension;
            optionsFile = new XmlFile(solutionDirectory + "\\DoctestTestAdapter\\Options.xml");

            generalOptionsPage.PropertyChanged += GeneralOptionsPage_PropertyChanged;

            // Update after all option pages are stored.
            Update();

            generalOptionsPage.SaveSettingsToStorage();
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

            textToWriteToOptionsFile +=
            (   
                "\t\t<" + Constants.XmlNodeNames.GeneralOptions + ">\n"
                + "\t\t\t<" + Constants.XmlNodeNames.EnableLogging + " Value=\"" + enableLogging.ToString() + "\"/>\n"
                + "\t\t\t<" + Constants.XmlNodeNames.CommandArguments + " Value=\"" + commandArguments + "\"/>\n"
                + "\t\t\t<" + Constants.XmlNodeNames.TestExecutableFilePath + " Value=\"" + testExecutableFilePath + "\"/>\n"
                + "\t\t\t<" + Constants.XmlNodeNames.SolutionDirectory + " Value=\"" + solutionDirectory + "\"/>\n"
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

        public string SolutionDirectory
        {
            get { return solutionDirectory; }
        }
    }
}
