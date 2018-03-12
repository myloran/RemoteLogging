using System;
using System.Collections.Generic;

namespace Okwy.Logging
{
	public static class MainLog
	{
		public static LogLevel globalLogLevel
		{
			get
			{
				return MainLog._globalLogLevel;
			}
			set
			{
				MainLog._globalLogLevel = value;
				foreach (Logger logger in MainLog._loggers.Values)
				{
					logger.logLevel = value;
				}
			}
		}

		public static void Trace(string message)
		{
			MainLog._logger.Trace(message);
		}

		public static void Debug(string message)
		{
			MainLog._logger.Debug(message);
		}

		public static void Info(string message)
		{
			MainLog._logger.Info(message);
		}

		public static void Warn(string message)
		{
			MainLog._logger.Warn(message);
		}

		public static void Error(string message)
		{
			MainLog._logger.Error(message);
		}

		public static void Fatal(string message)
		{
			MainLog._logger.Fatal(message);
		}

		public static void Assert(bool condition, string message)
		{
			MainLog._logger.Assert(condition, message);
		}

		public static void AddAppender(LogDelegate appender)
		{
			MainLog._appenders = (LogDelegate)Delegate.Combine(MainLog._appenders, appender);
			foreach (Logger logger in MainLog._loggers.Values)
			{
				logger.OnLog += appender;
			}
		}

		public static void RemoveAppender(LogDelegate appender)
		{
			MainLog._appenders = (LogDelegate)Delegate.Remove(MainLog._appenders, appender);
			foreach (Logger logger in MainLog._loggers.Values)
			{
				logger.OnLog -= appender;
			}
		}

		public static Logger GetLogger(string name)
		{
			Logger logger;
			if (!MainLog._loggers.TryGetValue(name, out logger))
			{
				logger = new Logger(name);
				logger.logLevel = MainLog.globalLogLevel;
				logger.OnLog += MainLog._appenders;
				MainLog._loggers.Add(name, logger);
			}
			return logger;
		}

		public static void ResetLoggers()
		{
			MainLog._loggers.Clear();
		}

		public static void ResetAppenders()
		{
			MainLog._appenders = null;
		}

		static LogLevel _globalLogLevel;

		static LogDelegate _appenders;

		static readonly Dictionary<string, Logger> _loggers = new Dictionary<string, Logger>();

		static readonly Logger _logger = MainLog.GetLogger("fabl");
	}
}
