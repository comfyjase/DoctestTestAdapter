using System;

namespace VS2022.DoctestTestAdapter.Settings
{
    public class DoctestSettingsOutputFileData : IEquatable<DoctestSettingsOutputFileData>
    {
        private string projectFilePath = string.Empty;
        private string executableFilePath = string.Empty;
        private string commandArguments = string.Empty;

        public string ProjectFilePath
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
