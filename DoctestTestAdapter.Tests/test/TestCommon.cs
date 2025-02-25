using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoctestTestAdapter.Tests
{
    internal static class TestCommon
    {
        internal static void AssertTestCase(TestCase testCase, string source, string fullyQualifiedName, string displayName, string codeFilePath, int lineNumber)
        {
            Assert.IsNotNull(testCase);
            Assert.AreEqual(source, testCase.Source);
            Assert.AreEqual(fullyQualifiedName, testCase.FullyQualifiedName);
            Assert.AreEqual(displayName, testCase.DisplayName);
            Assert.AreEqual(codeFilePath, testCase.CodeFilePath);
            Assert.AreEqual(lineNumber, testCase.LineNumber);
        }
    }
}
