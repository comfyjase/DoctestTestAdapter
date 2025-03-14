using DoctestTestAdapter.Shared.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoctestTestAdapter.Tests
{
    internal static class TestCommon
    {
#if DEBUG
        internal static string UsingDoctestMainExecutableFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Debug\\UsingDoctestMain\\UsingDoctestMain.exe";
        internal static string ExecutableUsingDLLExecutableFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Debug\\ExecutableUsingDLL\\ExecutableUsingDLL.exe";
        internal static string DLLExecutableFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Debug\\ExecutableUsingDLL\\DLL.dll";

        internal static string UsingDoctestMainPdbFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Debug\\UsingDoctestMain\\UsingDoctestMain.pdb";
        internal static string ExecutableUsingDLLPdbFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Debug\\ExecutableUsingDLL\\ExecutableUsingDLL.pdb";
        internal static string DLLExecutablePdbFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Debug\\DLL\\DLL.pdb";
#else
        internal static string UsingDoctestMainExecutableFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Release\\UsingDoctestMain\\UsingDoctestMain.exe";
        internal static string ExecutableUsingDLLExecutableFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Release\\ExecutableUsingDLL\\ExecutableUsingDLL.exe";
        internal static string DLLExecutableFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Release\\ExecutableUsingDLL\\DLL.dll";

        internal static string UsingDoctestMainPdbFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Release\\UsingDoctestMain\\UsingDoctestMain.pdb";
        internal static string ExecutableUsingDLLPdbFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Release\\ExecutableUsingDLL\\ExecutableUsingDLL.pdb";
        internal static string DLLExecutablePdbFilePath = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\bin\\x64\\Release\\DLL\\DLL.pdb";
#endif

        internal static string UsingDoctestMainTestHeaderFile = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\UsingDoctestMain\\TestIsEvenUsingDoctestMain.h";
        internal static string UsingCustomMainTestHeaderFile = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\UsingCustomMain\\TestIsEvenUsingCustomMain.h";
        internal static string ExecutableUsingDLLTestHeaderFile = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\DLLExample\\ExecutableUsingDLL\\TestIsEvenExecutableUsingDLL.h";
        internal static string DLLTestHeaderFile = Utilities.GetSolutionDirectory() + "\\DoctestTestAdapter.Examples\\DLLExample\\DLL\\TestIsEvenDLL.h";

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
