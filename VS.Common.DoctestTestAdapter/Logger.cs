using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace VS.Common.DoctestTestAdapter
{
    public class Logger
    {
        private static Logger instance = null;
        private static readonly object padlock = new object();

        public static Logger Instance 
        {
            get 
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Logger();
                    }
                    return instance;
                }
            }
        }

        // "C:\\Path\\To\\Debug\\Folder\\";
        private static string logDirectory;
        // "C:\\Path\\To\\Debug\\Folder\\Log.log";
        private static string logFilepath;

        private static int projectID = 0;

        private Logger()
        {
            if (VSUtilities.ShouldAttachDebugger())
            {
                Debugger.Launch();
            }

            SetupLogDirectory();
            SetupLogFile();
            WriteLine("New logger created");
        }

        private void SetupLogDirectory()
        {
            // There doesn't appear to be a way to query the associated project name during discovery time from here.
            // E.g. C++DoctestProjectA
            // I could store a projectName static var in this class and then set it when first iterating through source files.
            // But that feels very hacky and not very generic/good.
            // This isn't much better but at least provides unique folders for storing logs somewhere.
            Guid guid = Guid.NewGuid();
            string uniqueIDStr = guid.ToString("n");
            string currentDirectory = Directory.GetCurrentDirectory();
            string vsixLocation = currentDirectory;
            logDirectory = vsixLocation + "\\Logs\\DoctestTestAdapter\\" + uniqueIDStr + "\\";
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            WriteLineToOutput("[DoctestTestAdapter] Log directory: " + logDirectory);
        }

        private void SetupLogFile()
        {
            logFilepath = logDirectory + GetCurrentTimestampForDebugFilename() + ".log";

            if (!File.Exists(logFilepath))
            {
                // Create the debug file.
                using (StreamWriter sw = File.CreateText(logFilepath))
                {
                    WriteLineToOutput("[DoctestTestAdapter] Created log file: " + logFilepath);

                    DirectoryInfo parentDirectoryInfo = Directory.GetParent(logFilepath);
                    if(parentDirectoryInfo != null)
                    {
                        sw.WriteLine(GetCurrentTimestampForLogs() + " [DoctestTestAdapter] " + parentDirectoryInfo.Name);
                    }
                    sw.WriteLine(GetCurrentTimestampForLogs() + " [DoctestTestAdapter] " + "DoctestTestAdapter Log Start");
                    sw.WriteLine(GetCurrentTimestampForLogs() + " [DoctestTestAdapter] " + "============================");
                }
            }
            else
            {
                WriteLineToOutput("[DoctestTestAdapter] Log file " + logFilepath + " already exists, clearing file");

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

        private void WriteLineToOutput(string line)
        {
            Trace.WriteLine(line);
            Console.WriteLine(line);
        }

        private void WriteLineToLogFile(string line)
        {
            if (File.Exists(logFilepath))
            {
                using (StreamWriter sw = File.AppendText(logFilepath))
                {
                    sw.WriteLine(line);
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
            // Full message should end up being: [Timestamp] [DoctestTestAdapter] Class.cs Line: 31 FunctionA - Debug message.
            string message = GetCurrentTimestampForLogs() + " [DoctestTestAdapter] " + Path.GetFileName(sourceFilePath) + " Line: " + sourceLineNumber.ToString() + " " + memberName + " - " + indents + line;
            WriteLineToLogFile(message);
            WriteLineToOutput(message);
        }

        public void Dispose()
        {
            if (File.Exists(logFilepath))
            {
                using (StreamWriter sw = File.AppendText(logFilepath))
                {
                    WriteLineToOutput("[DoctestTestAdapter] Created log file: " + logFilepath);

                    sw.WriteLine("[DoctestTestAdapter] " + GetCurrentTimestampForLogs() + " DoctestTestAdapter Log End");
                    sw.WriteLine("[DoctestTestAdapter] " + GetCurrentTimestampForLogs() + " ==========================");
                }
            }
        }
    }
}
