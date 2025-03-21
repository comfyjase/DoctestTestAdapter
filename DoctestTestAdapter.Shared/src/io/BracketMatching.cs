﻿// BracketMatching.cs
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

namespace DoctestTestAdapter.Shared.IO
{
    internal class BracketMatching
    {
        private bool _isInside = false;
        private Stack<int> _bracketCounter = new Stack<int>();
        private Action _onLeaveBracketScope = null;

        public bool IsInside
        {
            get { return _isInside; }
        }

        public BracketMatching(Action onLeaveBracketScope) 
        {
            _onLeaveBracketScope = onLeaveBracketScope;
        }

        public void Check(string line)
        {
            foreach (char letter in line)
            {
                switch (letter)
                {
                    case '{':
                    {
                        _bracketCounter.Push(_bracketCounter.Count + 1);
                        _isInside = true;
                        break;
                    }
                    case '}':
                    {
                        if (_bracketCounter.Count > 0)
                            _bracketCounter.Pop();

                        if (_bracketCounter.Count == 0)
                        {
                            _isInside = false;
                            if (_onLeaveBracketScope != null)
                                _onLeaveBracketScope();
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
