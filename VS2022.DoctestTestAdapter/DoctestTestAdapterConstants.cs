using System;

namespace VS2022.DoctestTestAdapter
{
    public static class DoctestTestAdapterConstants
    {
        // Executor strings/URI
        public const String ExecutorUriString =  "executor://DoctestTestExecutor/v1";
        public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);

        // Settings strings
        public const String SettingsName                = "DoctestTestAdapter";
        public const String OutputFileNodeName          = "OutputFile";
        public const String ProjectFilePathNodeName     = "ProjectFilePath";
        public const String ExecutableFilePathNodeName  = "ExecutableFilePath";
        public const String CommandArgumentsNodeName    = "CommandArguments";

        // Supported file types
        public const String ExeFileExtension    = ".exe";
        public const String DLLFileExtension    = ".dll";
        public const String HFileExtension      = ".h";
        public const String HPPFileExtension    = ".hpp";
    }
}
