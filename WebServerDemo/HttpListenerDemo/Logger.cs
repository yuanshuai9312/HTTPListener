using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace CommonLib.LogOperation
{
    public enum LogLevel
    {
        [Description("[信息]")]
        Info = 1,
        [Description("[警告]")]
        Warning = 2,
        [Description("[中断]")]
        Throw = 3,
        [Description("[异常]")]
        Exception = 4,
        [Description("[错误]")]
        Error = 5,
        [Description("[退出]")]
        Exit = 6
    }

    /// <summary>
    /// 日志类。利用C#自带的Trace机制实现，使用本类的函数前，必须先注册适当的TraceListener。
    /// </summary>
    public static class Logger
    {
        private static object _lockHelper = new object();

        public static ConsoleTraceListener RegisterConsoleListener()
        {
            ConsoleTraceListener listener = new ConsoleTraceListener();
            Trace.Listeners.Add(listener);
            return listener;
        }

        public static void Exception(string message)
        {
            WriteEntry(message, "异常");
        }

        public static void Exception(Exception ex)
        {
            WriteEntry(ex.Message, "异常");
        }

        public static void Throw(string message)
        {
            WriteEntry(message, "中断");
            throw new Exception(message);
        }

        public static void Throw(Exception ex)
        {
            WriteEntry(ex.Message, "中断");
            throw ex;
        }

        public static void Exit(string message)
        {
            WriteEntry(message, "退出");
            Environment.Exit(Environment.ExitCode);
        }

        public static void Exit(Exception ex)
        {
            WriteEntry(ex.Message, "退出");
            Environment.Exit(Environment.ExitCode);
        }

        public static void Error(string message)
        {
            WriteEntry(message, "错误");
        }

        public static void Error(Exception ex)
        {
            WriteEntry(ex.Message, "错误");
        }

        public static void Warning(string message)
        {
            WriteEntry(message, "警告");
        }

        public static void Info(string message)
        {
            WriteEntry(message, "信息");
        }

        private static void WriteEntry(string message, string type)
        {
            lock (_lockHelper)
            {
                Trace.WriteLine(string.Format("[{0}][{1}] : {2}",
                                      DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                      type,
                                      message));
            }
        }
    }
}
