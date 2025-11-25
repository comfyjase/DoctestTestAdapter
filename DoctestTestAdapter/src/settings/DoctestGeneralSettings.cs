// DoctestGeneralSettings.cs
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

using System.Xml.Serialization;

namespace DoctestTestAdapter.Settings
{
    [XmlType]
    public class DoctestGeneralSettings
    {
        /// <summary>
        /// Used to provide the test adapter with a folder path for searching for source files under.
        /// </summary>
        public string RootDirectory { get; set; } = string.Empty;

        /// <summary>
        /// These arguments will be supplied first before any doctest command arguments are provided.
        /// E.g. with CommandArguments = --test
        /// Example command line would end up being: --test --test-case=*"[TestDecorator] Test 1"*,*"[TestDecorator] Test 2"*
        /// </summary>
        public string CommandArguments { get; set; } = string.Empty;

        /// <summary>
        /// Whether to send extra log messages to the test window or not.
        /// Prints DumpBin and CVDump exe outputs which might be helpful for debugging if something is going wrong.
        /// </summary>
        public bool EnableDebugLogs { get; set; } = false;
    }
}
