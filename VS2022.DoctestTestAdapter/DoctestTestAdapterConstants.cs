using System;
using System.Collections.Generic;
using System.Linq;
using static System.Windows.Forms.DataFormats;

namespace VS2022.DoctestTestAdapter
{
    public static class DoctestTestAdapterConstants
    {
        // Executor strings/URI
        public const string ExecutorUriString =  "executor://DoctestTestExecutor/v1";
        public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);

        // Settings strings
        public const string SettingsName                = "DoctestTestAdapter";
        public const string OutputFileNodeName          = "OutputFile";
        public const string ProjectFilePathNodeName     = "ProjectFilePath";
        public const string ExecutableFilePathNodeName  = "ExecutableFilePath";
        public const string CommandArgumentsNodeName    = "CommandArguments";

        // Supported file types
        public const string ExeFileExtension    = ".exe";
        public const string DLLFileExtension    = ".dll";
        public const string HFileExtension      = ".h";
        public const string HPPFileExtension    = ".hpp";

        // Doctest strings
        public const string TestResultErrorKeyword = "ERROR: ";
        public static readonly List<string> SkipDecorators = new List<string>() { "[Skip]", "[SKIP]" };
        public static readonly string SkipDecoratorsAsCommandArgument = string.Join(",", string.Join(",", SkipDecorators).Split(',').Select(x => string.Format("*{0}*", x)).ToList());
    }
}
