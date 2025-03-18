using DoctestTestAdapter.Shared.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoctestTestAdapter.Tests.Helpers
{
	[TestClass]
	public class UtilitiesTest
	{
        [TestMethod]
        public void SolutionDirectory()
            => Assert.IsTrue(Utilities.GetSolutionDirectory().EndsWith("DoctestTestAdapter"));

        [TestMethod]
        public void GetVsInstallDirectory()
            => Assert.IsFalse(string.IsNullOrEmpty(Utilities.GetVSInstallDirectory()));
    }
}
