using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace VS.Common
{
    public class Logger
    {
        private static readonly Lazy<Logger> lazyLoggerInstance = new Lazy<Logger>(() => new Logger());

        public static Logger Instance 
        { 
            get { return lazyLoggerInstance.Value; } 
        }

        // "C:\\Path\\To\\Debug\\Folder\\";
        private static string logDirectory;
        // "C:\\Path\\To\\Debug\\Folder\\Log.txt";
        private static string logFilepath;

        private Logger()
        {
            if (VSUtilities.ShouldAttachDebugger())
            {
                Debugger.Launch();
            }

            SetupLogDirectory();
            SetupLogFile();
        }

        private void SetupLogDirectory()
        {
            logDirectory = Directory.GetCurrentDirectory() + "\\DoctestTestAdapterLogs\\";
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
        }

        private void SetupLogFile()
        {
            logFilepath = logDirectory + GetCurrentTimestampForDebugFilename() + ".txt";

            if (!File.Exists(logFilepath))
            {
                // Create the debug file.
                using (StreamWriter sw = System.IO.File.CreateText(logFilepath))
                {
                    sw.WriteLine(GetCurrentTimestampForLogs() + " DoctestTestAdapter Log Start");
                    sw.WriteLine(GetCurrentTimestampForLogs() + " ============================");
                }
            }
            else
            {
                // Clear the contents of the file.
                File.WriteAllText(logFilepath, string.Empty);
            }
        }

        private string GetCurrentTimestampForDebugFilename()
        {
            DateTime currentTime = DateTime.Now;
            string currentTimestampAsString = "[" + currentTime.ToString("dd.MM.yyyy-HH.mm.ss tt") + "]";
            return currentTimestampAsString;
        }

        private string GetCurrentTimestampForLogs()
        {
            DateTime currentTime = DateTime.Now;
            string currentTimestampAsString = "[" + currentTime.ToString("dd/MM/yyyy HH:mm:ss tt") + "]";
            return currentTimestampAsString;
        }

        private void WriteLineToLogFile(string line)
        {
            if (File.Exists(logFilepath))
            {
                using (StreamWriter sw = System.IO.File.AppendText(logFilepath))
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
            if (File.Exists(logFilepath))
            {
                using (StreamWriter sw = File.AppendText(logFilepath))
                {
                    sw.WriteLine(GetCurrentTimestampForLogs() + " DoctestTestAdapter Log End");
                    sw.WriteLine(GetCurrentTimestampForLogs() + " ==========================");
                }
            }
        }
    }
}
