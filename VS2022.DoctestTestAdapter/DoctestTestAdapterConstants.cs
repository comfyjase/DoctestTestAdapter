using System;

namespace VS.Common
{
    public static class DoctestTestAdapterConstants
    {
        public const String ExecutorUriString =  "executor://DoctestTestExecutor/v1";
        public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);
    }
}
