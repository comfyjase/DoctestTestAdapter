﻿// Profiler.cs
//
// Copyright (c) 2025-present Jase Mottershead
//
// MIT License
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System;

namespace DoctestTestAdapter.Shared.Profiling
{
    internal class Profiler
    {
        private Stopwatch _watch = new Stopwatch();

        internal Profiler()
        { }

        internal void Start([CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (Helpers.Constants.ProfilingEnabled)
            {
                Console.WriteLine("=======================================================================================================");
                Console.WriteLine("[Test Adapter for Doctest][Profiler] - Start");
                Console.WriteLine("[Test Adapter for Doctest][Profiler] - SourceFile: " + Path.GetFileName(sourceFilePath) + " Function: " + memberName);
                _watch.Start();
            }
        }

        internal void End([CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
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
