using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DoctestTestAdapter.Shared.Pdb
{
    enum PdbLineDataFilter
    {
        // Only store one piece of line data per line number.
        Distinct = 0,
        // Add all line data the pdb provides, even if the line numbers are the same (e.g. number can be the same but addresses will be different).
        Everything
    }

    internal sealed class PdbLineData
    {
        public int Number
        {
            get; set;
        }

        public string Address
        {
            get; set;
        }

        public string Namespace
        {
            get; set;
        }

        public string ClassName
        {
            get; set;
        }

        public string LineStr
        {
            get; set;
        }

        public PdbLineData(int number, string address)
        {
            Number = number;
            Address = address;
        }
    }

    internal sealed class PdbData
    {
        public string CodeFilePath
        {
            get; private set;
        }

        public string[] CodeFileLines
        {
            get; private set;
        } = new string[] { };

        public string Name
        {
            get; set;
        }

        public List<PdbLineData> LineData
        {
            get; private set;
        } = new List<PdbLineData>();

        public PdbData(string filePath)
        {
            CodeFilePath = filePath;
            CodeFileLines = File.ReadAllLines(CodeFilePath);
        }

        public PdbLineData AddLineData(int lineNumber, string address, PdbLineDataFilter filter = PdbLineDataFilter.Distinct)
        {
            // For distinct, check if line data already exists
            if (filter == PdbLineDataFilter.Distinct)
            {
                PdbLineData existingPdbLineData = LineData.Find(ld => (ld.Number == lineNumber));
                if (existingPdbLineData != null)
                    return existingPdbLineData;
            }

            // Otherwise, it's completely new or we're interested in everything anyway!
            PdbLineData pdbLineData = new PdbLineData(lineNumber, address);
            pdbLineData.LineStr = CodeFileLines.Skip(lineNumber - 1).Take(1).First().Trim();
            LineData.Add(pdbLineData);
            return pdbLineData;
        }
    }
}
