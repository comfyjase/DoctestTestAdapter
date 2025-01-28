using System;

namespace VS2022.DoctestTestAdapter.Settings
{
    public class DoctestSettingsOutputFileData : IEquatable<DoctestSettingsOutputFileData>
    {
        private String projectFilePath = String.Empty;
        private String filePath = String.Empty;
        private String commandArguments = String.Empty;

        public String ProjectFilePath
        {
            get { return projectFilePath; }
        }

        public string FilePath
        {
            get { return filePath; }
        }

        public string CommandArguments
        {
            get { return commandArguments; }
        }

        public DoctestSettingsOutputFileData(string _projectFilePath, string _filePath, string _commandArguments)
        {
            projectFilePath = _projectFilePath;
            filePath = _filePath;
            commandArguments = _commandArguments;
        }

        public bool Equals(DoctestSettingsOutputFileData other)
        {
            return 
            (
                projectFilePath.Equals(other.projectFilePath) &&
                FilePath.Equals(other.FilePath) &&
                CommandArguments.Equals(other.CommandArguments)
            );
        }
    }
}
