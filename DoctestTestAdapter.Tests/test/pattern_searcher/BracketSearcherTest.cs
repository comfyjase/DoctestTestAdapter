// BracketSearcherTest.cs
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

using DoctestTestAdapter.Shared.PatternSearcher;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DoctestTestAdapter.Tests.PatternSearcher
{
    [TestClass]
    public class BracketSearcherTest
    {
        private BracketSearcher _bracketSearcher = new BracketSearcher();
        private List<string> _exampleCode = new List<string>()
        {
            "namespace A",
            "{",
                "namespace B",
                "{",
                    "void Foo()",
                    "{",
                    "}",
                "}",
            "}"
        };
        private List<string> _exampleCodeVariant = new List<string>()
        {
            "namespace A {",
                "namespace B {",
                    "void Foo() {",
                    "}",
                "}",
            "}"
        };
        private List<string> _exampleCodeVariantTwo = new List<string>()
        {
            "int i = 0;",
            "if (true) {",
                "i = 0;",
            "} else if (false) {",
                "i = 1;",
            "} else {",
                "i = 2;",
            "}"
        };
        private bool _onFoundOpenBracketFunctionCalled = false;
        private bool _onFoundCloseBracketFunctionCalled = false;
        private bool _onLeaveBracketScopeFunctionCalled = false;

        private void OnFoundOpenBracket(object sender, BracketSearcherEventArgs e)
        {
            _onFoundOpenBracketFunctionCalled = true;
        }

        private void OnFoundCloseBracket(object sender, BracketSearcherEventArgs e)
        {
            _onFoundCloseBracketFunctionCalled = true;
        }

        private void OnLeaveBracketScope(object sender, BracketSearcherEventArgs e)
        {
            _onLeaveBracketScopeFunctionCalled = true;
        }

        [TestMethod]
        public void FoundOpenBracket()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                Assert.IsNotNull(_bracketSearcher);
                _onFoundOpenBracketFunctionCalled = false;

                _bracketSearcher.OnFoundOpenBracket += OnFoundOpenBracket;
                _bracketSearcher.Check(_exampleCode[1]);
                _bracketSearcher.OnFoundOpenBracket -= OnFoundOpenBracket;

                Assert.IsTrue(_onFoundOpenBracketFunctionCalled);
            });
        }

        [TestMethod]
        public void FoundOpenBracketVariant()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                Assert.IsNotNull(_bracketSearcher);
                _onFoundOpenBracketFunctionCalled = false;

                _bracketSearcher.OnFoundOpenBracket += OnFoundOpenBracket;
                _bracketSearcher.Check(_exampleCodeVariant[0]);
                _bracketSearcher.OnFoundOpenBracket -= OnFoundOpenBracket;

                Assert.IsTrue(_onFoundOpenBracketFunctionCalled);
            });
        }

        [TestMethod]
        public void FoundOpenBracketVariantTwo()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                Assert.IsNotNull(_bracketSearcher);
                _onFoundOpenBracketFunctionCalled = false;

                _bracketSearcher.OnFoundOpenBracket += OnFoundOpenBracket;
                _bracketSearcher.Check(_exampleCodeVariantTwo[1]);
                _bracketSearcher.OnFoundOpenBracket -= OnFoundOpenBracket;

                Assert.IsTrue(_onFoundOpenBracketFunctionCalled);
            });
        }

        [TestMethod]
        public void FoundCloseBracket()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                Assert.IsNotNull(_bracketSearcher);
                _onFoundCloseBracketFunctionCalled = false;

                _bracketSearcher.OnFoundCloseBracket += OnFoundCloseBracket;
                _bracketSearcher.Check(_exampleCode[5]);
                _bracketSearcher.Check(_exampleCode[6]);
                _bracketSearcher.OnFoundCloseBracket -= OnFoundCloseBracket;

                Assert.IsTrue(_onFoundCloseBracketFunctionCalled);
            });
        }

        [TestMethod]
        public void FoundCloseBracketVariant()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                Assert.IsNotNull(_bracketSearcher);
                _onFoundCloseBracketFunctionCalled = false;

                _bracketSearcher.OnFoundCloseBracket += OnFoundCloseBracket;
                _bracketSearcher.Check(_exampleCodeVariant[2]);
                _bracketSearcher.Check(_exampleCodeVariant[3]);
                _bracketSearcher.OnFoundCloseBracket -= OnFoundCloseBracket;

                Assert.IsTrue(_onFoundCloseBracketFunctionCalled);
            });
        }

        [TestMethod]
        public void FoundCloseBracketVariantTwo()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                Assert.IsNotNull(_bracketSearcher);
                _onFoundCloseBracketFunctionCalled = false;

                _bracketSearcher.OnFoundCloseBracket += OnFoundCloseBracket;
                _bracketSearcher.Check(_exampleCodeVariantTwo[1]);
                _bracketSearcher.Check(_exampleCodeVariantTwo[3]);
                _bracketSearcher.OnFoundCloseBracket -= OnFoundCloseBracket;

                Assert.IsTrue(_onFoundCloseBracketFunctionCalled);
            });
        }

        [TestMethod]
        public void LeaveScope()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                Assert.IsNotNull(_bracketSearcher);
                _onLeaveBracketScopeFunctionCalled = false;

                _bracketSearcher.OnLeaveBracketScope += OnLeaveBracketScope;
                _exampleCode.ForEach(s => _bracketSearcher.Check(s));
                _bracketSearcher.OnLeaveBracketScope -= OnLeaveBracketScope;

                Assert.IsTrue(_onLeaveBracketScopeFunctionCalled);
            });
        }

        [TestMethod]
        public void LeaveScopeVariant()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                Assert.IsNotNull(_bracketSearcher);
                _onLeaveBracketScopeFunctionCalled = false;

                _bracketSearcher.OnLeaveBracketScope += OnLeaveBracketScope;
                _exampleCodeVariant.ForEach(s => _bracketSearcher.Check(s));
                _bracketSearcher.OnLeaveBracketScope -= OnLeaveBracketScope;

                Assert.IsTrue(_onLeaveBracketScopeFunctionCalled);
            });
        }

        [TestMethod]
        public void LeaveScopeVariantTwo()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                Assert.IsNotNull(_bracketSearcher);
                _onLeaveBracketScopeFunctionCalled = false;

                _bracketSearcher.OnLeaveBracketScope += OnLeaveBracketScope;
                _exampleCodeVariantTwo.ForEach(s => _bracketSearcher.Check(s));
                _bracketSearcher.OnLeaveBracketScope -= OnLeaveBracketScope;

                Assert.IsTrue(_onLeaveBracketScopeFunctionCalled);
            });
        }

        [TestMethod]
        public void Search()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                Assert.IsNotNull(_bracketSearcher);
                Assert.AreEqual(0, _bracketSearcher.NumberOfUnpairedBrackets);

                _bracketSearcher.Check(_exampleCode[0]);
                Assert.AreEqual(0, _bracketSearcher.NumberOfUnpairedBrackets);

                _bracketSearcher.Check(_exampleCode[1]);
                Assert.AreEqual(1, _bracketSearcher.NumberOfUnpairedBrackets);

                _bracketSearcher.Check(_exampleCode[3]);
                Assert.AreEqual(2, _bracketSearcher.NumberOfUnpairedBrackets);

                _bracketSearcher.Check(_exampleCode[5]);
                Assert.AreEqual(3, _bracketSearcher.NumberOfUnpairedBrackets);

                _bracketSearcher.Check(_exampleCode[6]);
                Assert.AreEqual(2, _bracketSearcher.NumberOfUnpairedBrackets);

                _bracketSearcher.Check(_exampleCode[7]);
                Assert.AreEqual(1, _bracketSearcher.NumberOfUnpairedBrackets);

                _bracketSearcher.Check(_exampleCode[8]);
                Assert.AreEqual(0, _bracketSearcher.NumberOfUnpairedBrackets);
            });
        }

        [TestMethod]
        public void SearchVariant()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                Assert.IsNotNull(_bracketSearcher);
                Assert.AreEqual(0, _bracketSearcher.NumberOfUnpairedBrackets);

                _bracketSearcher.Check(_exampleCodeVariant[0]);
                Assert.AreEqual(1, _bracketSearcher.NumberOfUnpairedBrackets);

                _bracketSearcher.Check(_exampleCodeVariant[1]);
                Assert.AreEqual(2, _bracketSearcher.NumberOfUnpairedBrackets);

                _bracketSearcher.Check(_exampleCodeVariant[2]);
                Assert.AreEqual(3, _bracketSearcher.NumberOfUnpairedBrackets);

                _bracketSearcher.Check(_exampleCodeVariant[3]);
                Assert.AreEqual(2, _bracketSearcher.NumberOfUnpairedBrackets);

                _bracketSearcher.Check(_exampleCodeVariant[4]);
                Assert.AreEqual(1, _bracketSearcher.NumberOfUnpairedBrackets);

                _bracketSearcher.Check(_exampleCodeVariant[5]);
                Assert.AreEqual(0, _bracketSearcher.NumberOfUnpairedBrackets);
            });
        }

        [TestMethod]
        public void SearchVariantTwo()
        {
            TestCommon.AssertErrorOutput(() =>
            {
                Assert.IsNotNull(_bracketSearcher);
                Assert.AreEqual(0, _bracketSearcher.NumberOfUnpairedBrackets);

                _bracketSearcher.Check(_exampleCodeVariantTwo[1]);
                Assert.AreEqual(1, _bracketSearcher.NumberOfUnpairedBrackets);

                _bracketSearcher.Check(_exampleCodeVariantTwo[3]);
                Assert.AreEqual(1, _bracketSearcher.NumberOfUnpairedBrackets);

                _bracketSearcher.Check(_exampleCodeVariantTwo[5]);
                Assert.AreEqual(1, _bracketSearcher.NumberOfUnpairedBrackets);

                _bracketSearcher.Check(_exampleCodeVariantTwo[7]);
                Assert.AreEqual(0, _bracketSearcher.NumberOfUnpairedBrackets);
            });
        }
    }
}
