using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

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
        private static string LogFilepath;

        private Logger()
        {
            // TODO: Test this...
            //string vsixLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string vsixLocation = Directory.GetCurrentDirectory();
            LogFilepath = vsixLocation + GetCurrentTimestampForDebugFilename() + " logs.txt";
            SetupLogFile();
        }

        private void SetupLogFile()
        {
            if (!File.Exists(LogFilepath))
            {
                // Create the debug file.
                using (StreamWriter sw = System.IO.File.CreateText(LogFilepath))
                {
                    sw.WriteLine(GetCurrentTimestampForLogs() + " DoctestTestAdapter Log Start");
                    sw.WriteLine(GetCurrentTimestampForLogs() + " ============================");
                }
            }
            else
            {
                // Clear the contents of the file.
                File.WriteAllText(LogFilepath, string.Empty);
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

        private void WriteLineToLogFile(string line)
        {
            if (File.Exists(LogFilepath))
            {
                using (StreamWriter sw = System.IO.File.AppendText(LogFilepath))
                {
                    sw.WriteLine(GetCurrentTimestampForLogs() + " " + line);
                }
            }
        }

        public void WriteLine(string line,
            int indentLevel = 0,
            [CallerMemberName]  string memberName = "",
            [CallerFilePath]    string sourceFilePath = "",
            [CallerLineNumber]  int sourceLineNumber = 0)
        {
            string indents = "";
            for (int i = 0; i < indentLevel; i++)
            {
                indents += "\t";
            }
            // C:Path/To/Source/Code/Class.cs Line: 31 FunctionA - Debug message.
            string message = sourceFilePath + " Line: " + sourceLineNumber.ToString() + " " + memberName + " - " + indents + line;
            WriteLineToLogFile(message);
            Trace.WriteLine(message);
        }

        public void Dispose()
        {
            if (File.Exists(LogFilepath))
            {
                using (StreamWriter sw = File.AppendText(LogFilepath))
                {
                    sw.WriteLine(GetCurrentTimestampForLogs() + " DoctestTestAdapter Log End");
                    sw.WriteLine(GetCurrentTimestampForLogs() + " ==========================");
                }
            }
        }
    }
}
