using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace VS.Common.DoctestTestAdapter
{
    public class Logger
    {
        private static readonly Lazy<Logger> instance = new Lazy<Logger>(() => new Logger());

        public static Logger Instance 
        {
            get 
            {
                return instance.Value;
            }
        }

        // "C:\\Path\\To\\Debug\\Folder\\";
        private static string logDirectory = "";
        // "C:\\Path\\To\\Debug\\Folder\\[Date-Time].log";
        private static string logFilepath = "";

        private Logger()
        {
            SetupLogDirectory();
            SetupLogFile();
            WriteLine("New logger created");
        }

        private void SetupLogDirectory()
        {
            // There doesn't appear to be a way to query the associated project name during discovery time from here.
            // E.g. C++DoctestProjectA
            // I could store a projectName static variable in this class and then set it when first iterating through source files.
            // But that feels very hacky and not very generic/good.
            // Instead, I'm just creating a unique ID for the logs to be stored under - not as ideal as trying to get the calling project name but it works!
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
                // Create the log file.
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

        /// <summary>
        /// This is separate from the function below because we need to respect the Windows filenaming rules.
        /// Can't have certain characters like "/" or ":" etc.
        /// So use a slightly different format just for the filename.
        /// </summary>
        /// <returns></returns>
        private string GetCurrentTimestampForDebugFilename()
        {
            DateTime currentTime = DateTime.Now;
            string currentTimestampAsString = "[" + currentTime.ToString("dd.MM.yyyy-HH.mm.ss tt") + "]";
            return currentTimestampAsString;
        }

        /// <summary>
        /// Will be prefixed to all messages in the log file.
        /// </summary>
        /// <returns></returns>
        private string GetCurrentTimestampForLogs()
        {
            DateTime currentTime = DateTime.Now;
            string currentTimestampAsString = "[" + currentTime.ToString("dd/MM/yyyy HH:mm:ss tt") + "]";
            return currentTimestampAsString;
        }

        private void WriteLineToOutput(string _line)
        {
            Trace.WriteLine(_line);
            Console.WriteLine(_line);
        }

        private void WriteLineToLogFile(string _line)
        {
            if (File.Exists(logFilepath))
            {
                using (StreamWriter sw = File.AppendText(logFilepath))
                {
                    sw.WriteLine(_line);
                }
            }
        }

        /// <summary>
        /// Writes _line to the debug/console output as well as to the log file.
        /// Note: This will automatically prefix a timestamp, logtag, filename, line number and function name to the message.
        /// E.g. 
        /// private void Foo()
        /// {
        ///     // Will print out: [Timestamp] [DoctestTestAdapter] Class.cs Line: 31 Foo - Debug message here.
        ///     Logger.Instance.WriteLine("Debug message here.");
        /// }
        /// 
        /// </summary>
        /// <param name="_line"></param>
        /// <param name="_indentLevel"></param>
        /// <param name="_memberName"></param>
        /// <param name="_sourceFilePath"></param>
        /// <param name="_sourceLineNumber"></param>
        public void WriteLine(string _line, int _indentLevel = 0, [CallerMemberName] string _memberName = "", [CallerFilePath] string _sourceFilePath = "", [CallerLineNumber] int _sourceLineNumber = 0)
        {
            string indents = "";
            for (int i = 0; i < _indentLevel; i++)
            {
                indents += "\t";
            }
            
            string timeStamp = GetCurrentTimestampForLogs();
            string logTag = "[DoctestTestAdapter]";
            string filename = Path.GetFileName(_sourceFilePath);
            string lineNumber = "Line: " + _sourceLineNumber.ToString();
            string functionName = _memberName;
            string separator = " ";

            // Full message should end up being: [Timestamp] [DoctestTestAdapter] Class.cs Line: 31 FunctionA - Debug message.
            string message = timeStamp + separator + logTag + separator + filename + separator + lineNumber + separator + functionName + " - " + indents + _line;
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
