using System;

namespace DoctestTestAdapter.Shared.Helpers
{
    internal static class Constants
    {
        internal const string ExecutorUriString = "executor://DoctestTestExecutor";
        internal static readonly Uri ExecutorUri = new Uri(ExecutorUriString);
    }
}
