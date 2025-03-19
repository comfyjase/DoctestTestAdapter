using DoctestTestAdapter.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoctestTestAdapter.Tests.Settings
{
    [TestClass]
    public class DoctestGeneralSettingsTest
    {
        [TestMethod]
        public void CommandArguments()
        {
            DoctestTestSettings doctestSettings = TestCommon.LoadDoctestSettings(new DoctestTestSettingsProvider(), TestCommon.GeneralRunSettingsExample);
            Assert.IsNotNull(doctestSettings);
            Assert.IsNotNull(doctestSettings.GeneralSettings);
            Assert.IsFalse(string.IsNullOrEmpty(doctestSettings.GeneralSettings.CommandArguments));
            Assert.AreEqual("--test", doctestSettings.GeneralSettings.CommandArguments);
        }
    }
}
