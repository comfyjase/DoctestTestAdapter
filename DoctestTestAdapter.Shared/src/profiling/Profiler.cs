using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System;

namespace DoctestTestAdapter.Shared.Profiling
{
    internal class Profiler
    {
        private Stopwatch _watch = new Stopwatch();

        public Profiler()
        { }

        public void Start([CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (Helpers.Constants.ProfilingEnabled)
            {
                Console.WriteLine("=======================================================================================================");
                Console.WriteLine("[Test Adapter for Doctest][Profiler] - Start");
                Console.WriteLine("[Test Adapter for Doctest][Profiler] - SourceFile: " + Path.GetFileName(sourceFilePath) + " Function: " + memberName);
                _watch.Start();
            }
        }

        public void End([CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (Helpers.Constants.ProfilingEnabled)
            {
                _watch.Stop();
                Console.WriteLine("[Test Adapter for Doctest][Profiler] - SourceFile: " + Path.GetFileName(sourceFilePath) + " Function: " + memberName + " took " + _watch.Elapsed.TotalSeconds + " seconds");
                Console.WriteLine("[Test Adapter for Doctest][Profiler] - End");
                Console.WriteLine("=======================================================================================================");
            }
        }
    }
}
