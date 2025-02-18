using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace VS2022.DoctestTestAdapter
{
    public static class DoctestTestAdapterConstants
    {
        // Executor strings/URI
        public const string ExecutorUriString =  "executor://DoctestTestExecutor/v1";
        public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);
    }
}
