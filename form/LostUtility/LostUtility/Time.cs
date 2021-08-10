
namespace LostUtility
{
	public class Time
	{
		//-----------------------------------------------------------

		public class Seconds
		{
			// 取得當下時間，單位:秒
			public static ulong GetTick()
			{
				// return System.Convert.ToUInt64( System.DateTime.UtcNow.AddHours( 8 ).Subtract( new System.DateTime( 1970, 1, 1 ) ).TotalSeconds );
				return System.Convert.ToUInt64( System.DateTime.Now.Subtract( new System.DateTime( 1970, 1, 1 ) ).TotalSeconds );
			}

			// 這函式的功能跟 StringToTick 是成對的
			public static string TickToString( ulong tick )
			{
				return (new System.DateTime( 1970, 1, 1 )).AddSeconds( System.Convert.ToInt32( tick ) ).ToString( "yyyy_MM_dd_HH_mm_ss" );
			}

			// 這函式的功能跟 TickToString 是成對的
			public static ulong StringToTick( string tick )
			{
				return System.Convert.ToUInt64( System.DateTime.ParseExact( tick, "yyyy_MM_dd_HH_mm_ss", System.Globalization.CultureInfo.InvariantCulture ).Subtract( new System.DateTime( 1970, 1, 1 ) ).TotalSeconds );
			}
		}

		//-----------------------------------------------------------

		public class Timer
		{
			private long _tick = 0;

			public long Restart()
			{
				long temp = _tick;
				_tick = System.DateTime.Now.Ticks;

				// 單位毫秒
				return (_tick - temp) / 10000;
			}

			public long Elapsed()
			{
				// 單位毫秒
				return (System.DateTime.Now.Ticks - _tick) / 10000;
			}
		}

		// 因為官網說 Stopwatch 比 System.DateTime.Now.Ticks 更適合評估效能
		// 因此這裡準備第二個版本備用
		public class Timer2
		{
			// 想要更細的精度就使用 System.Diagnostics.Stopwatch.ElapsedTicks
			private System.Diagnostics.Stopwatch _sw = new System.Diagnostics.Stopwatch();
			private long _tick = 0;

			public Timer2()
			{
				_sw.Start();
			}

			// 單位毫秒
			public long Restart()
			{
				long temp = _tick;

				_tick = _sw.ElapsedMilliseconds;

				// 單位毫秒
				return (_tick - temp) / 10000;
			}

			public long Elapsed()
			{
				// 單位毫秒
				return (_sw.ElapsedMilliseconds - _tick) / 10000;
			}
		}
	}
}
