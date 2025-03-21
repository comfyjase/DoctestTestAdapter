// TestCaseComparer.cs
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

using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;

namespace DoctestTestAdapter.Shared.EqualityComparers
{
    internal class TestCaseComparer : IEqualityComparer<TestCase>
    {
        public bool Equals(TestCase x, TestCase y)
        {
            if (x == y)
                return true;
            if (x == null || y == null)
                return false;

            return 
            (
                x.Source == y.Source
                && x.FullyQualifiedName == y.FullyQualifiedName
                && x.DisplayName == y.DisplayName
                && x.CodeFilePath == y.CodeFilePath
                && x.LineNumber == y.LineNumber
            );
        }

        public int GetHashCode(TestCase obj)
        {
            return obj != null ? obj.Id.GetHashCode() : 0;
        }
    }
}
