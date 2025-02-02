using System;
using System.Diagnostics;
using System.IO;
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

        // "C:\\Path\\To\\Debug\\Folder\\DoctestAdapter.log";
        private static string logFilePath = "";

        private VS.Common.DoctestTestAdapter.IO.File logFile = null;

        private Logger()
        {
            //TODO_comfyjase_02/02/2025: Would be really useful to have a timestamped log file.
            //logFilePath = Directory.GetCurrentDirectory() + "\\Logs\\DoctestTestAdapter_" + GetCurrentTimestampForDebugFilename() + ".log";
            logFilePath = Directory.GetCurrentDirectory() + "\\Logs\\DoctestTestAdapter.log";
            logFile = new VS.Common.DoctestTestAdapter.IO.File(logFilePath);

            WriteLine("New logger created! Directory: " + Path.GetDirectoryName(logFilePath) + " File: " + logFilePath);
            WriteLine("DoctestTestAdapter Log Start");
            WriteLine("============================");
        }

        public void Clear()
        {
            Debug.Assert(logFile != null);
            logFile.Clear();
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
        }

        private void WriteLineToLogFile(string _line)
        {
            Debug.Assert(logFile != null);
            logFile.Write(_line);
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
            string message = logTag + separator + filename + separator + lineNumber + separator + functionName + " - " + indents + _line;
            string timestampedMessage = timeStamp + separator + message;
            WriteLineToLogFile(timestampedMessage);
            WriteLineToOutput(message);
        }

        public void Dispose()
        {
            Debug.Assert(logFile != null);
            logFile.Write("DoctestTestAdapter Log End");
            logFile.Write("==========================");
        }
    }
}
