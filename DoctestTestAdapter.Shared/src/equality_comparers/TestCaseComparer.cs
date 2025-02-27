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
