using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace VS.Common.DoctestTestAdapter.Options
{
    public class GeneralOptionsPage : BaseOptionsPage
    {
        private bool enableLogging = false;

        [Category(Constants.Options.GeneralCategoryName)]
        [DisplayName(Constants.Options.LoggingOptionName)]
        [Description(Constants.Options.LoggingOptionDescription)]
        public bool EnableLogging
        {
            get { return enableLogging; }
            set { SetValue(ref enableLogging, value); }
        }
    }
}
