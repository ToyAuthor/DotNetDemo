
namespace LostUtility
{
	public class Time
	{
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
