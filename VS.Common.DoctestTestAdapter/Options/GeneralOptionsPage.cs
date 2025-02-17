using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace VS.Common.DoctestTestAdapter.Options
{
    public class GeneralOptionsPage : BaseOptionsPage
    {
        [Category(Constants.Options.GeneralCategoryName)]
        [DisplayName(Constants.Options.LoggingOptionName)]
        [Description(Constants.Options.LoggingOptionDescription)]
        public bool EnableLogging
        {
            get { return enableLogging; }
            set { SetValue(ref enableLogging, value); }
        }
        private bool enableLogging = false;

        [Category(Constants.Options.GeneralCategoryName)]
        [DisplayName(Constants.Options.CommandArgumentsOptionName)]
        [Description(Constants.Options.CommandArgumentsOptionDescription)]
        public string CommandArguments
        {
            get { return commandArguments; }
            set { SetValue(ref commandArguments, value); }
        }
        private string commandArguments = string.Empty;

        [Category(Constants.Options.GeneralCategoryName)]
        [DisplayName(Constants.Options.TestExecutableFilePathOptionName)]
        [Description(Constants.Options.TestExecutableFilePathOptionDescription)]
        public string TestExecutablePath
        {
            get { return testExecutablePath; }
            set { SetValue(ref testExecutablePath, value); }
        }
        private string testExecutablePath = string.Empty;
    }
}
