using DoctestTestAdapter.Settings;
using DoctestTestAdapter.Shared.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace DoctestTestAdapter.Tests.Settings
{
    [TestClass]
    public class DoctestDiscoverySettingsTest
    {
        private string _solutionDirectory = Utilities.GetSolutionDirectory(Directory.GetParent(TestCommon.UsingDoctestMainExecutableFilePath).FullName);

        [TestMethod]
        public void SearchDirectoriesRelative()
        {
            DoctestTestSettings doctestSettings = TestCommon.LoadDoctestSettings(new DoctestTestSettingsProvider(), TestCommon.DiscoveryRunSettingsRelativeSearchDirectoryExample);

            Assert.IsNotNull(doctestSettings);
            Assert.IsNotNull(doctestSettings.DiscoverySettings);
            Assert.IsNotNull(doctestSettings.DiscoverySettings.SearchDirectories);
            Assert.IsNotEmpty(doctestSettings.DiscoverySettings.SearchDirectories);
            Assert.HasCount(1, doctestSettings.DiscoverySettings.SearchDirectories);
            Assert.AreEqual("UsingDoctestMain", doctestSettings.DiscoverySettings.SearchDirectories[0]);
            
            Assert.IsTrue(doctestSettings.DiscoverySettings.AreSearchDirectoriesValid(_solutionDirectory, out string message));
            Assert.IsTrue(string.IsNullOrEmpty(message));
        }

        [TestMethod]
        public void SearchDirectoriesAbsolute()
        {
            DoctestTestSettings doctestSettings = TestCommon.LoadDoctestSettings(new DoctestTestSettingsProvider(), TestCommon.DiscoveryRunSettingsAbsoluteSearchDirectoryExample);
            
            Assert.IsNotNull(doctestSettings);
            Assert.IsNotNull(doctestSettings.DiscoverySettings);
            Assert.IsNotNull(doctestSettings.DiscoverySettings.SearchDirectories);
            Assert.IsNotEmpty(doctestSettings.DiscoverySettings.SearchDirectories);
            Assert.HasCount(1, doctestSettings.DiscoverySettings.SearchDirectories);
            Assert.AreEqual(_solutionDirectory + "\\UsingDoctestMain", doctestSettings.DiscoverySettings.SearchDirectories[0]);

            Assert.IsTrue(doctestSettings.DiscoverySettings.AreSearchDirectoriesValid(_solutionDirectory, out string message));
            Assert.IsTrue(string.IsNullOrEmpty(message));
        }

        [TestMethod]
        public void SearchDirectoriesInvalid()
        {
            DoctestTestSettings doctestSettings = TestCommon.LoadDoctestSettings(new DoctestTestSettingsProvider(), TestCommon.DiscoveryRunSettingsInvalidSearchDirectoryExample);

            Assert.IsNotNull(doctestSettings);
            Assert.IsNotNull(doctestSettings.DiscoverySettings);
            Assert.IsNotNull(doctestSettings.DiscoverySettings.SearchDirectories);
            Assert.IsNotEmpty(doctestSettings.DiscoverySettings.SearchDirectories);
            Assert.HasCount(1, doctestSettings.DiscoverySettings.SearchDirectories);
            Assert.AreEqual("NonExistentDirectory", doctestSettings.DiscoverySettings.SearchDirectories[0]);

            Assert.IsFalse(doctestSettings.DiscoverySettings.AreSearchDirectoriesValid(_solutionDirectory, out string message));
            Assert.IsFalse(string.IsNullOrEmpty(message));
        }
    }
}
