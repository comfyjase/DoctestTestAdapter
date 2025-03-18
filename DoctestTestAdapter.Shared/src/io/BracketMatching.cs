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
