using SysCompiler = System.Runtime.CompilerServices;

namespace LostUtility
{
	public class Debug
	{
		public enum LogLevel : uint
		{
			all,
			trace,
			debug,
			info,
			warn,
			error,
			fatal,
			off,
		}

		// 預設是給一個不做事的函式
		static private System.Action<string> _printer = ( string str ) => { };

		static public void SetPrinter( System.Action<string> act )
		{
			_printer = act;
		}

		// 可以當成公共輸出管道
		static public void Print( string str, LogLevel level = LogLevel.trace )
		{
			switch ( level )
			{
				case LogLevel.trace:
					_printer( "[trace]" + str );
					break;
				case LogLevel.debug:
					_printer( "[debug]" + str );
					break;
				case LogLevel.info:
					_printer( "[info]" + str );
					break;
				case LogLevel.warn:
					_printer( "[warn]" + str );
					break;
				case LogLevel.error:
					_printer( "[error]" + str );
					break;
				case LogLevel.fatal:
					_printer( "[fatal]" + str );
					break;
				default:
					Oops();
					break;
			}
		}

		// 印出呼叫 Oops() 的地點
		static public void Oops( [SysCompiler.CallerLineNumber] int lineNumber = 0, [SysCompiler.CallerFilePath] string filename = null )//, [CallerMemberName] string caller = null)
		{
			_printer( "[debug]" + filename + ":" + lineNumber );
		}

		// 還是別把 throw 這個字眼包裝進來好了
		static public System.Exception Exception( [SysCompiler.CallerLineNumber] int lineNumber = 0, [SysCompiler.CallerFilePath] string filename = null )
		{
			_printer( "[error]" + filename + ":" + lineNumber );  // 就算異常沒人捕獲也至少印出地點了
			return new System.Exception();
		}
	}
}
