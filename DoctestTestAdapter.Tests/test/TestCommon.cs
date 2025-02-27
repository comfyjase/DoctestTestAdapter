using DoctestTestAdapter.Shared.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoctestTestAdapter.Tests
{
    internal static class TestCommon
    {
        internal static string ExampleExecutableFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Debug\\UsingDoctestMain\\UsingDoctestMain.exe";
        internal static string ExampleExecutableUsingDLLFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Debug\\ExecutableUsingDLL\\ExecutableUsingDLL.exe";

        internal static readonly string ExampleTestRunSettings =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n"
            + "<RunSettings>\n"
                + "\t<RunConfiguration>\n"
                    + "\t\t<MaxCpuCount>1</MaxCpuCount>\n"
                    + "\t\t<ResultsDirectory>./TestResults</ResultsDirectory>\n"
                    + "\t\t<!-- set default session timeout to 5m -->\n"
                    + "\t\t<TestSessionTimeout>500000</TestSessionTimeout>\n"
                    + "\t\t<TreatNoTestsAsError>true</TreatNoTestsAsError>\n"
                + "\t</RunConfiguration>\n\n"

                + "\t<Doctest>\n"
                    + "\t\t<CommandArguments>\"--test\"</CommandArguments>\n"
                + "\t</Doctest>\n"
            + "</RunSettings>\n";

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
