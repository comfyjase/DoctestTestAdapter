using System;
using System.IO;
using System.Linq;

namespace DoctestTestAdapter.Shared.Helpers
{
    internal static class Utilities
    {
        internal static string GetSolutionDirectory()
        {
            //TODO_comfyjase_25/02/2025: Check if AppContext.BaseDirectory works when running VS Exp for custom test adapter.
            DirectoryInfo directory = new DirectoryInfo(AppContext.BaseDirectory);

            while (directory != null && !directory.EnumerateFiles("*.sln").Any())
                directory = directory.Parent;

            return directory?.FullName ?? throw new FileNotFoundException($"Could not find solution directory {directory}");
        }

        internal static string GetProjectDirectory(string projectFileType)
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);

            while (directory != null && !directory.EnumerateFiles("*.sln").Any() && !directory.EnumerateFiles("*" + projectFileType).Any())
                directory = directory.Parent;

            return directory?.FullName ?? throw new FileNotFoundException($"Could not find project directory {directory}");
        }
    }
}
