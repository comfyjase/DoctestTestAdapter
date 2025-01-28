using System;

namespace VS2022.DoctestTestAdapter.Settings
{
    public class DoctestSettingsOutputFileData : IEquatable<DoctestSettingsOutputFileData>
    {
        private String projectFilePath = String.Empty;
        private String executableFilePath = String.Empty;
        private String commandArguments = String.Empty;

        public String ProjectFilePath
        {
            get { return projectFilePath; }
        }

        public string ExecutableFilePath
        {
            get { return executableFilePath; }
        }

        public string CommandArguments
        {
            get { return commandArguments; }
        }

        public DoctestSettingsOutputFileData(string _projectFilePath, string _filePath, string _commandArguments)
        {
            projectFilePath = _projectFilePath;
            executableFilePath = _filePath;
            commandArguments = _commandArguments;
        }

        public bool Equals(DoctestSettingsOutputFileData other)
        {
            return 
            (
                projectFilePath.Equals(other.projectFilePath) &&
                ExecutableFilePath.Equals(other.ExecutableFilePath) &&
                CommandArguments.Equals(other.CommandArguments)
            );
        }
    }
}
