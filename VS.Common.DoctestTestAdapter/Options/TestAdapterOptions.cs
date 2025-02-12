using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VS.Common.DoctestTestAdapter.Options
{
    public class TestAdapterOptions : ITestAdapterOptions
    {
        // General options
        private GeneralOptionsPage generalOptionsPage = null;
        private bool enableLogging = false;

        public TestAdapterOptions() : this(null)
        { }

        //TODO_comfyjase_08/02/2025: Add any other option pages here...
        public TestAdapterOptions(GeneralOptionsPage _generalOptionsPage)
        {
            generalOptionsPage = _generalOptionsPage;
            Debug.Assert(generalOptionsPage != null);

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
            if (generalOptionsPage != null)
            {
                enableLogging = generalOptionsPage.EnableLogging;
                generalOptionsPage.SaveSettingsToStorage();
            }
        }

        public bool EnableLogging
        {
            get { return enableLogging; }
        }
    }
}
