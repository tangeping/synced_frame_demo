using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

namespace CBFrame.Utils
{
    /// <summary>
    /// 日志等级声明。
    /// </summary>
    [Flags]
    public enum LogLevel
    {
        /// <summary>
        /// 缺省
        /// </summary>
        NONE = 0,
        /// <summary>
        /// 调试
        /// </summary>
        DEBUG = 1,
        /// <summary>
        /// 信息
        /// </summary>
        INFO = 2,
        /// <summary>
        /// 警告
        /// </summary>
        WARNING = 4,
        /// <summary>
        /// 错误
        /// </summary>
        ERROR = 8,
        /// <summary>
        /// 异常
        /// </summary>
        EXCEPT = 16,
        /// <summary>
        /// 关键错误
        /// </summary>
        CRITICAL = 32,
    }

    /// <summary>
    /// 日志控制类。
    /// </summary>
    /// 
    public class Logger
    {
        /// <summary>
        /// 当前日志记录等级。
        /// </summary>
        public static LogLevel CurrentLogLevels = LogLevel.DEBUG | LogLevel.INFO | LogLevel.WARNING | LogLevel.ERROR | LogLevel.CRITICAL | LogLevel.EXCEPT;
        private const Boolean SHOW_STACK = true;
        private static LogWriter m_logWriter;
        public static string DebugFilterStr = string.Empty;

        static Logger()
        {
            m_logWriter = new LogWriter();
            //Application.RegisterLogCallback(new Application.LogCallback(ProcessExceptionReport));
        }

        public static void Release()
        {
            m_logWriter.Release();
        }

        public static void UploadLogFile()
        {
            m_logWriter.UploadTodayLog();
        }

        /// <summary>
        /// 调试日志。
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="isShowStack">是否显示调用栈信息</param>
        public static void Debug(object message, Boolean isShowStack = SHOW_STACK, int user = 0)
        {
            return;
            //if (user != 11)
            //    return;
            if (DebugFilterStr != "") return;

            if (LogLevel.DEBUG == (CurrentLogLevels & LogLevel.DEBUG))
                Log(string.Concat(" [DEBUG]: ", isShowStack ? GetStackInfo() : "", message), LogLevel.DEBUG);
        }

        /// <summary>
        /// 扩展debug
        /// </summary>
        /// <param name="message"></param>
        /// <param name="filter">只输出与在DebugMsg->filter里面设置的filter一样的debug</param>
        public static void Debug(string filter, object message, Boolean isShowStack = SHOW_STACK)
        {

            if (DebugFilterStr != "" && DebugFilterStr != filter) return;
            if (LogLevel.DEBUG == (CurrentLogLevels & LogLevel.DEBUG))
            {
                Log(string.Concat(" [DEBUG]: ", isShowStack ? GetStackInfo() : "", message), LogLevel.DEBUG);
            }

        }

        public static void DebugFormat(string format, params object[] args)
        {
            if (DebugFilterStr != "") return;

            if (LogLevel.DEBUG == (CurrentLogLevels & LogLevel.DEBUG))
            {
                Log(string.Concat(" [DEBUG]: ", string.Format(format, args)), LogLevel.DEBUG);
            }
        }

        /// <summary>
        /// 信息日志。
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Info(object message, Boolean isShowStack = SHOW_STACK)
        {
            if (LogLevel.INFO == (CurrentLogLevels & LogLevel.INFO))
                Log(string.Concat(" [INFO]: ", isShowStack ? GetStackInfo() : "", message), LogLevel.INFO);
        }

        public static void InfoFormat(string message, params object[] args)
        {
            if (LogLevel.INFO == (CurrentLogLevels & LogLevel.INFO))
                Log(string.Concat(" [INFO]: ", string.Format(message, args)), LogLevel.INFO);
        }

        /// <summary>
        /// 警告日志。
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Warning(object message, Boolean isShowStack = SHOW_STACK)
        {
            if (LogLevel.WARNING == (CurrentLogLevels & LogLevel.WARNING))
                Log(string.Concat(" [WARNING]: ", isShowStack ? GetStackInfo() : "", message), LogLevel.WARNING);
        }

        /// <summary>
        /// 异常日志。
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Error(object message, Boolean isShowStack = SHOW_STACK)
        {
            if (LogLevel.ERROR == (CurrentLogLevels & LogLevel.ERROR))
                Log(string.Concat(" [ERROR]: ", message, '\n', isShowStack ? GetStacksInfo() : ""), LogLevel.ERROR);
        }

        public static void ErrorFormat(string format, params object[] args)
        {
            if (LogLevel.ERROR == (CurrentLogLevels & LogLevel.ERROR))
                Log(string.Concat(" [ERROR]: ", string.Format(format, args)), LogLevel.ERROR);
        }

        /// <summary>
        /// 关键日志。
        /// </summary>
        /// <param name="message">日志内容</param>
        public static void Critical(object message, Boolean isShowStack = SHOW_STACK)
        {
            if (LogLevel.CRITICAL == (CurrentLogLevels & LogLevel.CRITICAL))
                Log(string.Concat(" [CRITICAL]: ", message, '\n', isShowStack ? GetStacksInfo() : ""), LogLevel.CRITICAL);
        }

        /// <summary>
        /// 异常日志。
        /// </summary>
        /// <param name="ex">异常实例。</param>
        public static void Except(Exception ex, object message = null)
        {
            if (LogLevel.EXCEPT == (CurrentLogLevels & LogLevel.EXCEPT))
            {
                Exception innerException = ex;
                while (innerException.InnerException != null)
                {
                    innerException = innerException.InnerException;
                }
                Log(string.Concat(" [EXCEPT]: ", message == null ? "" : message + "\n", ex.Message, innerException.StackTrace), LogLevel.CRITICAL);
            }
        }

        /// <summary>
        /// 获取堆栈信息。
        /// </summary>
        /// <returns></returns>
        private static String GetStacksInfo()
        {
            StringBuilder sb = new StringBuilder();
            StackTrace st = new StackTrace(true);
            var sf = st.GetFrames();
            for (int i = 2; i < sf.Length; i++)
            {
                sb.AppendLine(sf[i].ToString());
            }

            return sb.ToString();
        }

        /// <summary>
        /// 写日志。
        /// </summary>
        /// <param name="message">日志内容</param>
        private static void Log(string message, LogLevel level, bool writeEditorLog = false)
        {
            //var msg = string.Concat(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff"), message);
            //m_logWriter.WriteLog(msg, level, writeEditorLog);
            m_logWriter.WriteLog(message, level, writeEditorLog);
            //Debugger.Log(0, "TestRPC", message);
        }

        /// <summary>
        /// 获取调用栈信息。
        /// </summary>
        /// <returns>调用栈信息</returns>
        private static String GetStackInfo()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(2);//[0]为本身的方法 [1]为调用方法
            var method = sf.GetMethod();

            return String.Format("{0}.{1}(): ", method.ReflectedType.Name, method.Name);

        }

        private static void ProcessExceptionReport(string message, string stackTrace, LogType type)
        {
            var logLevel = LogLevel.DEBUG;
            switch (type)
            {
                case LogType.Assert:
                    logLevel = LogLevel.DEBUG;
                    break;
                case LogType.Error:
                    logLevel = LogLevel.ERROR;
                    break;
                case LogType.Exception:
                    logLevel = LogLevel.EXCEPT;
                    break;
                case LogType.Log:
                    logLevel = LogLevel.DEBUG;
                    break;
                case LogType.Warning:
                    logLevel = LogLevel.WARNING;
                    break;
                default:
                    break;
            }

            if (logLevel == (CurrentLogLevels & logLevel))
                Log(string.Concat(" [SYS_", logLevel, "]: ", message, '\n', stackTrace), logLevel, false);
        }

        public static void Debug(string fileName, string Content)
        {
            string m_logPath = UnityEngine.Application.dataPath + "/log/";
            if (!Directory.Exists(m_logPath))
                Directory.CreateDirectory(m_logPath);

            string m_logFileName = "log_{0}_{1}.txt";
            string m_logFilePath = String.Concat(m_logPath, String.Format(m_logFileName, DateTime.Today.ToString("yyyyMMdd"),fileName));

            FileStream m_fs = new FileStream(m_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

            StreamWriter sw = new StreamWriter(m_fs);

            sw.WriteLine(GetStackInfo() +" " + Content);

            sw.Flush();

            sw.Close();
            m_fs.Close();
        }

    }

    /// <summary>
    /// 日志写入文件管理类。
    /// </summary>
    public class LogWriter
    {
        private string m_logPath = UnityEngine.Application.dataPath + "/log/";
        private string m_logFileName = "log_{0}.txt";
        private string m_logFilePath;
        private FileStream m_fs;
        private StreamWriter m_sw;
        private Action<String, LogLevel, bool> m_logWriter;
        private readonly static object m_locker = new object();

        /// <summary>
        /// 默认构造函数。
        /// </summary>
        public LogWriter()
        {
            if (!Directory.Exists(m_logPath))
                Directory.CreateDirectory(m_logPath);
            m_logFilePath = String.Concat(m_logPath, String.Format(m_logFileName, DateTime.Today.ToString("yyyyMMdd")));
            try
            {
                m_logWriter = Write;
                m_fs = new FileStream(m_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                m_sw = new StreamWriter(m_fs);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(ex.Message);
            }
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Release()
        {
            lock (m_locker)
            {
                if (m_sw != null)
                {
                    m_sw.Close();
                    m_sw.Dispose();
                }
                if (m_fs != null)
                {
                    m_fs.Close();
                    m_fs.Dispose();
                }
            }
        }

        public void UploadTodayLog()
        {
            //lock (m_locker)
            //{
            //    using (var fs = new FileStream(m_logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //    {
            //        using (StreamReader sr = new StreamReader(fs))
            //        {
            //            var content = sr.ReadToEnd();
            //            var fn = Utils.GetFileName(m_logFilePath);//.Replace('/', '\\')
            //            if (MogoWorld.theAccount != null)
            //            {
            //                fn = string.Concat(MogoWorld.theAccount.name, "_", fn);
            //            }
            //            DownloadMgr.Instance.UploadLogFile(fn, content);
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 写日志。
        /// </summary>
        /// <param name="msg">日志内容</param>
        public void WriteLog(string msg, LogLevel level, bool writeEditorLog)
        {
#if UNITY_IPHONE
            m_logWriter(msg, level, writeEditorLog);
#else
            //m_logWriter.BeginInvoke(msg, level, writeEditorLog, null, null);
#endif
            Write(msg, level, writeEditorLog);
        }

        private void Write(string msg, LogLevel level, bool writeEditorLog)
        {
            lock (m_locker)
                try
                {
                    if (writeEditorLog)
                    {
                        switch (level)
                        {
                            case LogLevel.DEBUG:
                            case LogLevel.INFO:
                                UnityEngine.Debug.Log(msg);
                                break;
                            case LogLevel.WARNING:
                                UnityEngine.Debug.LogWarning(msg);
                                break;
                            case LogLevel.ERROR:
                            case LogLevel.EXCEPT:
                            case LogLevel.CRITICAL:
                                UnityEngine.Debug.LogError(msg);
                                break;
                            default:
                                break;
                        }
                    }
                    if (m_sw != null)
                    {
                        m_sw.WriteLine(msg);
                        m_sw.Flush();
                    }
                    else
                    {
                        UnityEngine.Debug.LogError("m_sw is null");
                    }
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError(ex.Message);
                }
        }
    }
}