using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VS2022.DoctestTestAdapter.Settings
{
    public class DoctestSettingsOutputFileData : IEquatable<DoctestSettingsOutputFileData>
    {
        private string filePath = "";
        private string commandArguments = "";

        public string FilePath
        {
            get { return filePath; }
        }

        public string CommandArguments
        {
            get { return commandArguments; }
        }

        public DoctestSettingsOutputFileData(string _filePath, string _commandArguments)
        {
            filePath = _filePath;
            commandArguments = _commandArguments;
        }

        public bool Equals(DoctestSettingsOutputFileData other)
        {
            return 
            (
                FilePath.Equals(other.FilePath) &&
                CommandArguments.Equals(other.CommandArguments)
            );
        }
    }
}
