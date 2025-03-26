// BracketSearcher.cs
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

using System;
using System.Collections.Generic;

namespace DoctestTestAdapter.Shared.PatternSearcher
{
    internal class BracketSearcherEventArgs : EventArgs
    {
        internal int BracketNumber { get; private set; }

        internal BracketSearcherEventArgs(int bracketNumber)
        {
            BracketNumber = bracketNumber;
        }
    }

    internal class BracketSearcher
    {
        private Stack<int> _bracketCounter = new Stack<int>();

        internal event EventHandler<BracketSearcherEventArgs> OnFoundOpenBracket;
        internal event EventHandler<BracketSearcherEventArgs> OnFoundCloseBracket;
        internal event EventHandler<BracketSearcherEventArgs> OnLeaveBracketScope;

        internal int NumberOfUnpairedBrackets
        {
            get { return _bracketCounter.Count; }
        }

        internal BracketSearcher()
        { }

        internal void Check(string line)
        {
            foreach (char letter in line)
            {
                switch (letter)
                {
                    case '{':
                    {
                        int bracketNumber = _bracketCounter.Count;
                        _bracketCounter.Push(bracketNumber);
                        OnFoundOpenBracket?.Invoke(this, new BracketSearcherEventArgs(bracketNumber));
                        break;
                    }
                    case '}':
                    {
                        if (_bracketCounter.Count > 0)
                        {
                            int bracketNumber = _bracketCounter.Pop();

                            OnFoundCloseBracket?.Invoke(this, new BracketSearcherEventArgs(bracketNumber));

                            if (_bracketCounter.Count == 0)
                            {
                                OnLeaveBracketScope?.Invoke(this, new BracketSearcherEventArgs(bracketNumber));
                            }
                        }

                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }
        }
    }
}
