using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PR2_SMS;

namespace MyDebug
{
    class TraceHelper
    {
        private static StringPass setStatusMsg = null;
        private static DateTime dt;
        public static void InitializeTrace(string FileName)
        {
            try
            {
                StreamWriter sw = new StreamWriter(FileName, true, Encoding.Default);
                TextWriterTraceListener listener = new TextWriterTraceListener(sw);
                Trace.Listeners.Add(listener);
            }
            catch(Exception)
            {
            }
        }
        public static void InitializeStatusDelegate(StringPass del)
        {
            setStatusMsg = del;
        }
        public static void StartTimer()
        {       
            dt = DateTime.MinValue;
            dt = DateTime.Now;
        }
        public static string StopTimer()
        {
            int secs = DateTime.Now.Second - dt.Second;
            int res = DateTime.Now.Millisecond - dt.Millisecond;
            
            if (secs == 0)
            {
                return res.ToString();
            }
            if (secs > 0)
            {
                res += 1000 * secs;
                return res.ToString();
            }
            if (secs < 0)
            {
               // throw new Exception("TraceHelperError: passed more, than minute");
            }
            return "bad"; 
        }
        public static int EndTimer()
        {
            int secs = DateTime.Now.Second - dt.Second;
            if ( secs == 0)
            {
                return DateTime.Now.Millisecond - dt.Millisecond;
            }
            if (secs > 0)
            {
                return 1000 * secs + DateTime.Now.Millisecond - dt.Millisecond;
            }
            if (secs < 0)
            {
                
            }
            return -1;
        }
        public static void Report(string Message)
        {
            if (setStatusMsg != null)
            {
                try
                {
                	setStatusMsg(ref Message);
                }
                catch { };
            }
            Trace.WriteLine(Message);
        }
        public static void Indent()
        {
            Trace.Indent(); 
        }
        public static void UnIndent()
        {
            Trace.Unindent();
        }
        internal static void Release()
        {
            Trace.Close();
        }
    }
}
