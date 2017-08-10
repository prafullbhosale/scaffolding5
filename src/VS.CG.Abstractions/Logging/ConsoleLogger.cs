using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Web.CodeGeneration.Abstractions.Logging
{
    public class ConsoleLogger : ILogger
    {
        private static bool isTrace = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("codegen_trace"));
        private object _syncObject = new object();

        public void Log(string message, LogMessageLevel logMessageLevel = LogMessageLevel.Information)
        {
            lock (_syncObject)
            {
                if (logMessageLevel == LogMessageLevel.Error)
                {
                    Console.Error.WriteLine(message);
                }
                else if (logMessageLevel == LogMessageLevel.Trace && isTrace)
                {
                    Console.Out.WriteLine($"[Trace]: {message}");
                }
                else if (logMessageLevel == LogMessageLevel.Information)
                {
                    Console.Out.WriteLine(message);
                }
            }
        }

        public void LogError(string errorMessage)
        {
            Log(errorMessage, LogMessageLevel.Error);
        }

        public void LogException(Exception ex)
        {
            Log(ex.Message, LogMessageLevel.Error);
            Log(ex.StackTrace, LogMessageLevel.Trace);
        }
    }
}
