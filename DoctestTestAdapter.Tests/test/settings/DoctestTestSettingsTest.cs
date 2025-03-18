using DoctestTestAdapter.Settings;
using FakeItEasy;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoctestTestAdapter.Tests.Settings
{
	[TestClass]
	public class DoctestTestSettingsTest
	{
		[TestMethod]
		public void Load() =>
            Assert.IsNotNull(DoctestTestSettingsProvider.LoadSettings(A.Fake<IRunContext>()));
    }
}
