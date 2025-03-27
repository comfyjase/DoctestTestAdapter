# Test Adapter for Doctest

## Overview
There are two different solutions provided: `DoctestTestAdapter.sln` and `DoctestTestAdapter.Godot.sln`. The only difference between these two solution files is that `DoctestTestAdapter.Godot.sln` includes the godot game engine source code and some godot specific unit tests in a `DoctestTestAdapter.Tests.Godot` project. See below table for reference:
| | `DoctestTestAdapter.sln` | `DoctestTestAdapter.Godot.sln` |
| --- | --- | --- |
| DoctestTestAdapter.Examples | ✅ | ✅ |
| DoctestTestAdapter.Examples.Godot | ❌ | ✅ |
| DoctestTestAdapter | ✅ | ✅ |
| DoctestTestAdapter.Shared | ✅ | ✅ |
| DoctestTestAdapter.Tests | ✅ | ✅ |
| DoctestTestAdapter.Tests.Godot | ❌ | ✅ |
| DoctestTestAdapter.Tests.Shared | ✅ | ✅ |
| DoctestTestAdapter.VSIX | ✅ | ✅ |  

It's setup this way for convenience in case a developer wanted quicker iteration times on a task/bug and didn't want to build the godot engine source. However, it is extremely important to note that **testing should always be done for both the doctest and godot examples to maintain stability for the test adapter**.  

### DoctestTestAdapter.Examples
This project has 4 different C++ projects: `UsingDoctestMain`, `UsingCustomMain`, `ExecutableUsingDLL` and `DLL`. These are setup to provide a small example codebase with different doctest setups and to showcase DLL support.
* UsingDoctestMain - Implements doctest unit tests using `DOCTEST_CONFIG_IMPLEMENT_WITH_MAIN`.
* UsingCustomMain - Implements doctest unit tests using a custom main function `int main(int argc, char** argv)` and sets up `doctest::Context context`.
* ExecutableUsingDLL - Implements doctest unit tests using `DOCTEST_CONFIG_IMPLEMENTATION_IN_DLL` and a custom main function which loads the `DLL` project and sets up `doctest::Context context`.
* DLL - Implements doctest unit tests using `DOCTEST_CONFIG_IMPLEMENTATION_IN_DLL` and `DOCTEST_CONFIG_IMPLEMENT`.
There is also a separate `Examples.sln` which just has all of the `DoctestTestAdapter.Examples` projects in separate from the rest of the `DoctestTestAdapter` code. It's useful for debugging any test adapter code changes to select this solution to open up.

### DoctestTestAdapter.Examples.Godot
This C++ project contains the `godot` engine source code from the `master` branch. This is another useful solution to open up for testing any test adapter code changes.  
> [!NOTE]  
> Please remember to setup a `.runsettings` file for your `godot` example solution. Documentation on `.runsettings` can be found [here](https://learn.microsoft.com/en-us/visualstudio/test/configure-unit-tests-by-using-a-dot-runsettings-file?view=vs-2022).  
> Here are example settings that can be used for godot:  
> ```xml
> <?xml version="1.0" encoding="utf-8"?>  
> <RunSettings>  
> 	<RunConfiguration>  
> 		<!-- set default session timeout to 5m -->  
> 		<TestSessionTimeout>500000</TestSessionTimeout>  
> 		<TreatNoTestsAsError>true</TreatNoTestsAsError>  
> 	</RunConfiguration>  
> 	<Doctest>  
> 		<GeneralSettings>  
> 			<CommandArguments>--headless --test</CommandArguments>  
> 		</GeneralSettings>  
> 		<DiscoverySettings>  
> 			<SearchDirectories>  
> 				<string>modules</string>  
> 				<string>tests</string>  
> 			</SearchDirectories>  
> 		</DiscoverySettings>  
> 	</Doctest>  
> </RunSettings>  
> ```

### DoctestTestAdapter
This C# project is the main project to bring together the test adapter code. Here the [`ITestDiscoverer`](https://github.com/microsoft/vstest/blob/782a46231902762286f6958631437d635bfdd249/src/Microsoft.TestPlatform.ObjectModel/Adapter/Interfaces/ITestDiscoverer.cs) and [`ITestExecutor`](https://github.com/microsoft/vstest/blob/782a46231902762286f6958631437d635bfdd249/src/Microsoft.TestPlatform.ObjectModel/Adapter/Interfaces/ITestExecutor.cs) are implemented in `DoctestTestDiscoverer` and `DoctestTestExecutor`. 
* DoctestDiscoverySettings - Settings specific to discovering tests. Currently, this only consists of `SearchDirectories` which allows the user to specify relative or absolute source code folder paths to search for tests in.
* DoctestExecutorSettings - Settings specific to executing tests. Implemented `ExecutableOverrides` previously, but then discovered it wasn't really needed for these example setups but it's kept in just in case anyone might find a use for it. User can specify relative or absolute file paths to executables to use instead of the discovered ones.
* DoctestGeneralSettings - Settings that are shared across discovery/executing. Contains `CommandArguments` and `PrintStandardOutput`.
* DoctestTestSettings - Implements [`TestRunSettings`](https://github.com/microsoft/vstest/blob/782a46231902762286f6958631437d635bfdd249/src/Microsoft.TestPlatform.ObjectModel/RunSettings/TestRunSettings.cs). Contains `DoctestGeneralSettings`, `DoctestDiscoverySettings` and `DoctestExecutorSettings`. Also implements helper methods to access settings for each of these classes.
* DoctestTestSettingsProvider - Implements [`ISettingsProvider`](https://github.com/microsoft/vstest/blob/782a46231902762286f6958631437d635bfdd249/src/Microsoft.TestPlatform.ObjectModel/Adapter/Interfaces/ISettingsProvider.cs). Provides a way to load `DoctestTestSettings`.
* DoctestTestDiscoverer - Makes use of the `TestCaseFactory` to create test cases and sends each one to the discovery sink.
* DoctestTestExecutor - Creates `DoctestExecutable` and `DoctestExecutableTestBatch` to actually run the doctest unit tests.

### DoctestTestAdapter.Shared
This C# shared project includes most of the different classes that handle different parts of logic for making this test adapter work. Anything that doesn't directly implement a class or interface from the [vstest](https://github.com/microsoft/vstest) framework should be stored in this project.
* TestCaseComparer - Used to provide equality checking for [`TestCase`](https://github.com/microsoft/vstest/blob/782a46231902762286f6958631437d635bfdd249/src/Microsoft.TestPlatform.ObjectModel/TestCase.cs).
* CVDumpExecutable - This class is a wrapper around starting a `Process` for the third party `cvdump.exe` to print out what source files are included in a given pdb file by reading the stringtable.
* DoctestExecutable - This class represents the discovered executable from the user's projects with doctest unit tests in. Can access test suite names, test case names and run unit tests using this class. This is also responsible for recording test starts and test finishes and reporting the [TestResults](https://github.com/microsoft/vstest/blob/782a46231902762286f6958631437d635bfdd249/src/Microsoft.TestPlatform.ObjectModel/TestResult.cs).
* DoctestExecutableTestBatch - Used to store lists of `TestCase`s, relevant command arguments and the test report file path. These are created per executable and then further batches are created as necessary to make sure the command line arguments for the DoctestExecutables are within the [windows command prompt string limit](https://learn.microsoft.com/en-us/troubleshoot/windows-client/shell-experience/command-line-string-limitation) (if there happens to be lots of test cases).
* DumpBinExecutable - Wrapper around starting a [`dumpbin`](https://learn.microsoft.com/en-us/cpp/build/reference/dumpbin-options?view=msvc-170) `Process` to be able to get any dependencies and the PDB file path.
* Executable - Base class for any executable processes to inherit from. Handles logic for starting a process and either waiting for the process to exit or wait for an exit event to be raised. Also has the option to attach to a process with a debugger using an [`IFrameworkHandle`](https://github.com/microsoft/vstest/blob/782a46231902762286f6958631437d635bfdd249/src/Microsoft.TestPlatform.ObjectModel/Adapter/Interfaces/IFrameworkHandle.cs).
* TestCaseFactory - Has functions for creating a single test case (from specific test case data) or a list of test cases (from a given executable). This class makes use of the keywords to check source file lines.
* Constants - Static class where constant variables can live.
* Utilities - Static class where helpful functions that don't quite belong in a single class can live.
* ClassKeyword - Searches source code for the `class` keyword within source files and storing class names to help create test cases later on.
* CustomMacroKeyword - Searches source code for custom `#define` macros that are wrappers around doctest macros (e.g. `TEST_CASE`, `TEST_CASE_FIXTURE`, `TEST_CASE_TEMPLATE`).
* DoctestTestCaseFixtureKeyword - Searches source code for `TEST_CASE_FIXTURE` macros.
* DoctestTestCaseKeyword - Searches source code for `TEST_CASE` macros and creates individual test cases.
* DoctestTestCaseMayFailKeyword - Searches source code for `TEST_CASE_MAY_FAIL` macros (didn't realise they were godot specific until after this was implemented). Leaving in for completeness and ease of use.
* DoctestTestCaseTemplateKeyword - Searches source code for `TEST_CASE_TEMPLATE` macros.
* DoctestTestSuiteKeyword - Searches source code for `TEST_SUITE` macros.
* IKeyword - Interface for all keywords to implement.
* Keyword - Base keyword class for other keywords to inherit from where appropriate. Contains logic for providing the keyword search, tracking when the line is inside of a namespace/class scope and calling `OnEnterKeywordScope` and `OnExitKeywordScope`.
* NamespaceKeyword - Searches source code for the `namespace` keyword and stores it to help create test cases later on.
* BracketSearcher - Tracks how many brackets have been paired by searching for '{' and '}' within a `string` line.
* Profiler - Basically `StopWatch` with printing.

### DoctestTestAdapter.Tests
This C# project holds most of the unit tests for the test adapter. These should be self explanatory but for completeness, see the below table for unit tests and their descriptions.

| Unit Test Name | Unit Test Description |
| --- | --- |
| DoctestTestDiscovererTest.DiscoverExe | Uses `DoctestTestDiscoverer` to discover tests from `UsingDoctestMain.exe` and assert the test cases. |
| DoctestTestDiscovererTest.DiscoverExeAndDLL | Uses `DoctestTestDiscoverer` to discover tests from `ExecutableUsingDLL.exe` and assert the test cases. |
| DoctestTestDiscovererTest.DiscoverExeWithPrintStandardOutputSetting | Uses `DoctestTestDiscoverer` to discover tests from `UsingDoctestMain.exe` and assert the test cases and assert that standard output was printed during discovery. |
| TestCaseComparerTest.CompareDifferentTestCases | Creates two different test cases and uses a new `TestCaseComparer` to assert that they are not equal. |
| TestCaseComparerTest.CompareSameTestCases | Creates one test case and creates a copy of it and then uses a new `TestCaseComparer` to assert that they are equal. |
| TestCaseComparerTest.CompareTestCaseAgainstNull | Creates one test case and one null test case and then uses a new `TestCaseComparer` to assert that they are not equal. |
| CVDumpExecutableTest.SourceFilesDLL | Uses the third party `cvdump.exe` to get the source files from `DLL.dll` and assert the source file paths. |
| CVDumpExecutableTest.SourceFilesExe | Uses the third party `cvdump.exe` to get the source files from `UsingDoctestMain.exe` and assert the source file paths. |
| DoctestExecutableTest.TestCaseNamesDLL | Gets all of the test case names from `ExecutableUsingDLL.exe` and asserts them. |
| DoctestExecutableTest.TestCaseNamesExe | Gets all of the test case names from `UsingDoctestMain.exe` and asserts them. |
| DoctestExecutableTest.TestSuiteNamesDLL | Gets all of the test suite names from `ExecutableUsingDLL.exe` and asserts them. |
| DoctestExecutableTest.TestSuiteNamesExe | Gets all of the test suite names from `UsingDoctestMain.exe` and asserts them. |
| DumpBinExecutableTest.DependenciesDLL | Uses `dumpbin` to get all of the dll dependencies for `DLL.dll`. |
| DumpBinExecutableTest.DependenciesExe | Uses `dumpbin` to get all of the dll dependencies for `ExecutableUsingDLL.exe`. |
| DumpBinExecutableTest.PDBFilePathDLL | Uses `dumpbin` to get the pdb file path for `DLL.dll`. |
| DumpBinExecutableTest.PDBFilePathExe | Uses `dumpbin` to get the pdb file path for `UsingDoctestMain.exe`. |
| DoctestTestExecutorTest.ExecuteExe | Uses `DoctestTestExecutor` to run the unit tests for `UsingDoctestMain.exe` and assert the test results/messages. |
| DoctestTestExecutorTest.ExecuteExeAndDLL | Uses `DoctestTestExecutor` to run the unit tests for `ExecutableUsingDLL.exe` and assert the test results/messages. |
| DoctestTestExecutorTest.ExecuteExeWithExeOverrideSetting | Uses `DoctestTestExecutor` with an exe override to run `UsingCustomMain.exe` instead of `UsingDoctestMain.exe` and assert the test results/messages. |
| TestCaseFactoryTest.CreateTestCasesExe | Uses `TestCaseFactory` to create test cases for `UsingDoctestMain.exe` and assert them. |
| TestCaseFactoryTest.CreateTestCasesExeAndDLL | Uses `TestCaseFactory` to create test cases for `ExecutableUsingDLL.exe` and assert them. |
| UtilitiesTest.VsInstallDirectory | Asserts that the `GetVsInstallDirectory` exists. |
| UtilitiesTest.SolutionDirectory | Asserts that the `GetSolutionDirectory` ends with `DoctestTestAdapter`. |
| UtilitiesTest.SolutionDirectoryExe | Asserts that the `GetSolutionDirectory` relative from `UsingDoctestMain.exe` ends with `DoctestTestAdapter.Examples`. |
| UtilitiesTest.SolutionDirectoryDLL | Asserts that the `GetSolutionDirectory` relative from `DLL.dll` ends with `DoctestTestAdapter.Examples`. |
| CustomTestCaseFixtureMacroKeywordTest.FindInNamespace | Uses the `NamespaceKeyword`, `CustomMacroKeyord` and `DoctestTestCaseFixtureKeyword` classes to search for `CUSTOM_TEST_CASE_FIXTURE_MACRO` inside of a namespace and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| CustomTestCaseFixtureMacroKeywordTest.FindInNamespaceAndTestSuite | Uses the `NamespaceKeyword`, `DoctestTestSuiteKeyword`, `CustomMacroKeyord` and `DoctestTestCaseFixtureKeyword` classes to search for `CUSTOM_TEST_CASE_FIXTURE_MACRO` inside of a namespace and test suite and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| CustomTestCaseFixtureMacroKeywordTest.FindInNestedNamespace | Uses the `NamespaceKeyword`, `CustomMacroKeyord` and `DoctestTestCaseFixtureKeyword` classes to search for `CUSTOM_TEST_CASE_FIXTURE_MACRO` inside of a nested namespace and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| CustomTestCaseFixtureMacroKeywordTest.FindInNoNamespaceOrTestSuite | Uses the `CustomMacroKeyord` and `DoctestTestCaseFixtureKeyword` classes to search for `CUSTOM_TEST_CASE_FIXTURE_MACRO` inside of no namespace or test suite and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| CustomTestCaseFixtureMacroKeywordTest.FindInTestSuite | Uses the `DoctestTestSuiteKeyword`, `CustomMacroKeyord` and `DoctestTestCaseFixtureKeyword` classes to search for `CUSTOM_TEST_CASE_FIXTURE_MACRO` inside of a test suite and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| CustomTestCaseMacroKeywordTest.FindInNamespace | Uses the `NamespaceKeyword`, `CustomMacroKeyord` and `DoctestTestCaseKeyword` classes to search for `CUSTOM_TEST_CASE_MACRO` inside of a namespace and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| CustomTestCaseMacroKeywordTest.FindInNamespaceAndTestSuite | Uses the `NamespaceKeyword`, `DoctestTestSuiteKeyword`, `CustomMacroKeyord` and `DoctestTestCaseKeyword` classes to search for `CUSTOM_TEST_CASE_MACRO` inside of a namespace and test suite and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| CustomTestCaseMacroKeywordTest.FindInNestedNamespace | Uses the `NamespaceKeyword`, `CustomMacroKeyord` and `DoctestTestCaseKeyword` classes to search for `CUSTOM_TEST_CASE_MACRO` inside of a nested namespace and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| CustomTestCaseMacroKeywordTest.FindInNoNamespaceOrTestSuite | Uses the `CustomMacroKeyord` and `DoctestTestCaseKeyword` classes to search for `CUSTOM_TEST_CASE_MACRO` inside of no namespace or test suite and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| CustomTestCaseMacroKeywordTest.FindInTestSuite | Uses the `DoctestTestSuiteKeyword`, `CustomMacroKeyord` and `DoctestTestCaseKeyword` classes to search for `CUSTOM_TEST_CASE_MACRO` inside of a test suite and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| CustomTestCaseTemplateMacroKeywordTest.FindInNamespace | Uses the `NamespaceKeyword`, `CustomMacroKeyord` and `DoctestTestCaseTemplateKeyword` classes to search for `CUSTOM_TEST_CASE_TEMPLATE_MACRO` inside of a namespace and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| CustomTestCaseTemplateMacroKeywordTest.FindInNamespaceAndTestSuite | Uses the `NamespaceKeyword`, `DoctestTestSuiteKeyword`, `CustomMacroKeyord` and `DoctestTestCaseTemplateKeyword` classes to search for `CUSTOM_TEST_CASE_TEMPLATE_MACRO` inside of a namespace and test suite and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| CustomTestCaseTemplateMacroKeywordTest.FindInNestedNamespace | Uses the `NamespaceKeyword`, `CustomMacroKeyord` and `DoctestTestCaseTemplateKeyword` classes to search for `CUSTOM_TEST_CASE_TEMPLATE_MACRO` inside of a nested namespace and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| CustomTestCaseTemplateMacroKeywordTest.FindInNoNamespaceOrTestSuite | Uses the `CustomMacroKeyord` and `DoctestTestCaseTemplateKeyword` classes to search for `CUSTOM_TEST_CASE_TEMPLATE_MACRO` inside of no namespace or test suite and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| CustomTestCaseTemplateMacroKeywordTest.FindInTestSuite | Uses the `DoctestTestSuiteKeyword`, `CustomMacroKeyord` and `DoctestTestCaseTemplateKeyword` classes to search for `CUSTOM_TEST_CASE_TEMPLATE_MACRO` inside of a test suite and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| DoctestTestCaseFixtureKeywordTest.FindInNamespace | Uses the `NamespaceKeyword` and `DoctestTestCaseFixtureKeyword` classes to search for `TEST_CASE_FIXTURE` inside of a namespace and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| DoctestTestCaseFixtureKeywordTest.FindInNamespaceAndTestSuite | Uses the `NamespaceKeyword`, `DoctestTestSuiteKeyword`, and `DoctestTestCaseFixtureKeyword` classes to search for `TEST_CASE_FIXTURE` inside of a namespace and test suite and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| DoctestTestCaseFixtureKeywordTest.FindInNestedNamespace | Uses the `NamespaceKeyword` and `DoctestTestCaseFixtureKeyword` classes to search for `TEST_CASE_FIXTURE` inside of a nested namespace and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| DoctestTestCaseFixtureKeywordTest.FindInNoNamespaceOrTestSuite | Uses the `DoctestTestCaseFixtureKeyword` class to search for `TEST_CASE_FIXTURE` inside of no namespace or test suite and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| DoctestTestCaseFixtureKeywordTest.FindInTestSuite | Uses the `DoctestTestSuiteKeyword` and `DoctestTestCaseFixtureKeyword` classes to search for `TEST_CASE_FIXTURE` inside of a test suite and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| DoctestTestCaseKeywordTest.FindInNamespace | Uses the `NamespaceKeyword` and `DoctestTestCaseKeyword` classes to search for `TEST_CASE` inside of a namespace and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| DoctestTestCaseKeywordTest.FindInNamespaceAndTestSuite | Uses the `NamespaceKeyword`, `DoctestTestSuiteKeyword`, and `DoctestTestCaseKeyword` classes to search for `TEST_CASE` inside of a namespace and test suite and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| DoctestTestCaseKeywordTest.FindInNestedNamespace | Uses the `NamespaceKeyword` and `DoctestTestCaseKeyword` classes to search for `TEST_CASE` inside of a nested namespace and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| DoctestTestCaseKeywordTest.FindInNoNamespaceOrTestSuite | Uses the `DoctestTestCaseKeyword` class to search for `TEST_CASE` inside of no namespace or test suite and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| DoctestTestCaseKeywordTest.FindInTestSuite | Uses the `DoctestTestSuiteKeyword` and `DoctestTestCaseKeyword` classes to search for `TEST_CASE` inside of a test suite and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| DoctestTestCaseTemplateKeywordTest.FindInNamespace | Uses the `NamespaceKeyword` and `DoctestTestCaseTemplateKeyword` classes to search for `TEST_CASE_TEMPLATE` inside of a namespace and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| DoctestTestCaseTemplateKeywordTest.FindInNamespaceAndTestSuite | Uses the `NamespaceKeyword`, `DoctestTestSuiteKeyword`, and `DoctestTestCaseTemplateKeyword` classes to search for `TEST_CASE_TEMPLATE` inside of a namespace and test suite and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| DoctestTestCaseTemplateKeywordTest.FindInNestedNamespace | Uses the `NamespaceKeyword` and `DoctestTestCaseTemplateKeyword` classes to search for `TEST_CASE_TEMPLATE` inside of a nested namespace and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| DoctestTestCaseTemplateKeywordTest.FindInNoNamespaceOrTestSuite | Uses the `DoctestTestCaseTemplateKeyword` class to search for `TEST_CASE_TEMPLATE` inside of no namespace or test suite and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| DoctestTestCaseTemplateKeywordTest.FindInTestSuite | Uses the `DoctestTestSuiteKeyword` and `DoctestTestCaseTemplateKeyword` classes to search for `TEST_CASE_TEMPLATE` inside of a test suite and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| DoctestTestSuiteKeywordTest.FindNested | Uses the `NamespaceKeyword` and `DoctestTestSuiteKeyword` classes to search for `TEST_SUITE` inside of a namespace and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| DoctestTestSuiteKeywordTest.FindSingle | Uses the `DoctestTestSuiteKeyword` class to search for `TEST_SUITE` and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| NamespaceKeywordTest.FindNested | Uses the `NamespaceKeyword` class to search for nested namespaces and assert they exist in `TestIsEvenUsingDoctestMain.h`. |
| NamespaceKeywordTest.FindSingle | Uses the `NamespaceKeyword` class to search for a single namespace and assert it exists in `TestIsEvenUsingDoctestMain.h`. |
| BracketSearcherTest.FoundCloseBracket | Uses `BracketSearcher` to assert that the `OnFoundCloseBracket` event is called for a small code example. |
| BracketSearcherTest.FoundOpenBracket | Uses `BracketSearcher` to assert that the `OnFoundOpenBracket` event is called for a small code example. |
| BracketSearcherTest.OnLeaveBracketScope | Uses `BracketSearcher` to assert that the `OnLeaveBracketScope` event is called for a small code example. |
| BracketSearcherTest.Search | Uses `BracketSearcher` to check each line from a small code example and asserts the number of paired brackets as it goes along. |
| DoctestDiscoverySettingsTest.SearchDirectoriesAbsolute | Uses some example settings with absolute search directories and asserts they are valid using `DoctestDiscoverySettings`. |
| DoctestDiscoverySettingsTest.SearchDirectoriesInvalid | Uses some example settings with invalid (non-existent) search directories and asserts they are invalid using `DoctestDiscoverySettings`. |
| DoctestDiscoverySettingsTest.SearchDirectoriesRelative | Uses some example settings with relative search directories and asserts they are valid using `DoctestDiscoverySettings`. |
| DoctestExecutorSettingsTest.ExecutableOverrideAbsolute | Uses some example settings with absolute executable override file paths and asserts they are valid using `DoctestExecutorSettings`. |
| DoctestExecutorSettingsTest.ExecutableOverrideInvalid | Uses some example settings with invalid (non-existent) executable override file paths and asserts they are invalid using `DoctestExecutorSettings`. |
| DoctestExecutorSettingsTest.ExecutableOverrideRelative | Uses some example settings with relative executable override file paths and asserts they are valid using `DoctestExecutorSettings`. |
| DoctestGeneralSettingsTest.CommandArguments | Uses example settings with a command argument set and asserts it's not null or empty when accessing it from `DoctestGeneralSettings`. |
| DoctestGeneralSettingsTest.PrintStandardOutput | Uses example settings with `PrintStandardOutput` set to true and asserts it is true when accessing it from `DoctestGeneralSettings`. |
| DoctestTestSettingsTest.CommandArgumentsHelper | Loads example settings with a command argument defined and uses the `TryGetCommandArguments` helper function in `DoctestTestSettings` and asserts the result. |
| DoctestTestSettingsTest.ExecutableOverridesHelper | Loads example settings with an executable override defined and uses the `TryGetExecutableOverrides` helper function in `DoctestTestSettings` and asserts the result. |
| DoctestTestSettingsTest.Load | Uses `DoctestTestSettingsProvider` to load settings and asserts it's not null. |
| DoctestTestSettingsTest.PrintStandardOutputHelper | Loads example settings with `PrintStandardOutput` set to true and uses the `TryGetPrintStandardOutput` helper function in `DoctestTestSettings` and asserts the result. |
| DoctestTestSettingsTest.SearchDirectoriesHelper | Loads example settings with a search directory defined and uses the `TryGetSearchDirectories` helper function in `DoctestTestSettings` and asserts the result. |

### DoctestTestAdapter.Tests.Godot
This C# project holds the godot specific unit tests for the test adapter. These should be self explanatory but for completeness, see the below table for unit tests and their descriptions.

| Unit Test Name | Unit Test Description |
| --- | --- |
| GodotDoctestTestDiscovererTest.DiscoverExe | Uses `DoctestTestDiscoverer` to discover tests from `godot.windows.editor.dev.x86_64.exe` and assert that there are no missing test cases. |
| GodotDoctestTestExecutorTest.ExecuteExe | Uses `DoctestTestExecutor` to run the unit tests for `godot.windows.editor.dev.x86_64.exe` and asserts the number of expected test results. |
| DoctestTestCaseMayFailKeywordTest.Find | Uses `NamespaceKeyword` and `DoctestTestCaseMayFailKeyword` to search for `TEST_CASE_MAY_FAIL` and assert it exists in `test_random_number_generator.h`. |

### DoctestTestAdapter.Tests.Shared
This C# shared project contains any classes that should be shared for any C# unit test projects for this test adapter.
* TestCommon - Stores example file paths, example .runsettings and some custom assert functions for tests to reuse.

### DoctestTestAdapter.VSIX
This C# project just contains references to the relevant projects for producing a `.vsix` file and manifest file for updating information about the `vsix`. This project can be run to test any code changes for the test adapter, it will open up the experimental version of whatever Visual Studio version you are using.
