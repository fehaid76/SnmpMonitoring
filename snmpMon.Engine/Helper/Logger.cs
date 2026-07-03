using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snmpMon.Engine.Helper
{
    public static class Logger
    {
        private const string SourceName = "snmpMon";
        private const string LogName = "Application";

        static Logger()
        {
            try
            {
                if (!EventLog.SourceExists(SourceName))
                {
                    EventLog.CreateEventSource(SourceName, LogName);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Event Sorce could not be created. Run as Admin.");
            }
        }

        public static void LogCriticalException(Exception ex, string ccontextMessage)
        {
            string fullMessage = $"Context: {ccontextMessage}\n" +
                                 $"Exception: {ex.Message}\n" +
                                 $"Stack Trace:\n {ex.StackTrace}";

            try
            {
                EventLog.WriteEntry(SourceName, fullMessage, EventLogEntryType.Error);
            }
            catch
            {
                Debug.WriteLine($"EventViewer Failed. Original Error: {fullMessage}");
            }
        }
    }
}
