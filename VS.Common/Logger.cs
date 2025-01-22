using System;
using System.Diagnostics;
using System.IO;

namespace VS.Common
{
    public class Logger
    {
        private static readonly Logger instance = new Logger();
        public static Logger Instance
        {
            get { return instance; }
        }

        // = "C:\\Path\\To\\Debug\\Folder\\Log.txt";
        private static string DEBUGFILEPATH;

        private Logger()
        {
            //string vsixLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string vsixLocation = Directory.GetCurrentDirectory();
            DEBUGFILEPATH = vsixLocation + GetCurrentTimestampForDebugFilename() + " logs.txt";
            SetupDebugFile();
        }

        private void SetupDebugFile()
        {
            // This text is added only once to the file.
            if (!File.Exists(DEBUGFILEPATH))
            {
                // Create a file to write to.
                using (StreamWriter sw = System.IO.File.CreateText(DEBUGFILEPATH))
                {
                    sw.WriteLine(GetCurrentTimestampForLogs() + " DoctestTestAdapter Log Start");
                    sw.WriteLine(GetCurrentTimestampForLogs() + " ============================");
                }
            }
            else
            {
                File.WriteAllText(DEBUGFILEPATH, string.Empty);
            }
        }

        private string GetCurrentTimestampForDebugFilename()
        {
            DateTime currentTime = DateTime.Now;
            string currentTimestampAsString = "[" + currentTime.ToString("D HH.mm.ss tt") + "]";
            return currentTimestampAsString;
        }

        private string GetCurrentTimestampForLogs()
        {
            DateTime currentTime = DateTime.Now;
            string currentTimestampAsString = "[" + currentTime.ToString("d HH:mm:ss tt") + "]";
            return currentTimestampAsString;
        }

        private void WriteLineToDebugFile(string line)
        {
            if (File.Exists(DEBUGFILEPATH))
            {
                // Create a file to write to.
                using (StreamWriter sw = System.IO.File.AppendText(DEBUGFILEPATH))
                {
                    sw.WriteLine(GetCurrentTimestampForLogs() + " " + line);
                }
            }
        }

        public void WriteLine(string line)
        {
            WriteLineToDebugFile(line);
            Trace.WriteLine(line);
        }

        public void Dispose()
        {
            if (File.Exists(DEBUGFILEPATH))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.AppendText(DEBUGFILEPATH))
                {
                    sw.WriteLine(GetCurrentTimestampForLogs() + " DoctestTestAdapter Log End");
                    sw.WriteLine(GetCurrentTimestampForLogs() + " ==========================");
                }
            }
        }
    }
}
