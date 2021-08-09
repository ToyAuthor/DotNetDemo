using SysContainer = System.Collections.Generic;
using SysTask =System.Threading;

namespace LostUtility
{
	public class Threading
	{
		public class Mailbox    // : SynchronizationContext
		{
			private readonly SysContainer.Queue<System.Action> _messagesQueue = new SysContainer.Queue<System.Action>();   // 待執行的訊息工作佇列
			private readonly object _syncHandle = new object();                    // 擔任 mutex 的工作
			private bool _isRunning = true;                                        // 是否正在執行中

			// 這樣的用法比另一個 Post 更加直覺
			public void Post( System.Action act )
			{
				lock ( _syncHandle )
				{
					// 將要處理的訊息工作，加入佇列中
					_messagesQueue.Enqueue( act );
					SysTask.Monitor.Pulse( _syncHandle );
				}
			}

			// Blocking
			public bool Read( ref System.Action act )
			{
				act = RetriveItem();

				if ( act == null ) return false;
				return true;
			}

			// Non-blocking
			public bool Peek( ref System.Action act )
			{
				lock ( _syncHandle )
				{
					if ( CanContinue() && _messagesQueue.Count == 0 )
					{
						return false;
					}

					if ( _isRunning == false )
					{
						return false;
					}

					act = _messagesQueue.Dequeue();

					return true;
				}
			}

			public bool IsEmpty()
			{
				lock ( _syncHandle )
				{
					if ( _messagesQueue.Count == 0 ) return true;

					return false;
				}
			}

			public int Length
			{
				get
				{
					lock ( _syncHandle )
					{
						return _messagesQueue.Count;
					}
				}
			}

			// 直接禁止同步調用
			//public override void Send( SendOrPostCallback codeToRun, object state )
			//{
			//	throw new NotImplementedException();
			//}

			// 異步執行
			//public override void Post( SendOrPostCallback codeToRun, object state )
			//{
			//	lock ( _syncHandle )
			//	{
			//		// 將要處理的訊息工作，加入佇列中
			//		_messagesQueue.Enqueue( () => codeToRun( state ) );
			//		Monitor.Pulse( _syncHandle );
			//	}
			//}

			// 啟用一個訊息幫浦，不推薦這個用法，應該外部自己決定迴圈該長怎樣
			//public void RunMessagePump()
			//{
			//	while ( CanContinue() )
			//	{
			//		Action nextToRun = RetriveItem();
			//		if ( nextToRun != null )
			//		{
			//			nextToRun();
			//		}
			//	}
			//}

			// 要求訊息幫浦停止運作
			public void Cancel()
			{
				lock ( _syncHandle )
				{
					_isRunning = false;
					SysTask.Monitor.Pulse( _syncHandle );   // Monitor.PulseAll 也許更適合這裡的需要
				}
			}

			//--------------------------------------------------------------------------

			// 取出待處理的訊息工作項目
			private System.Action RetriveItem()
			{
				lock ( _syncHandle )
				{
					while ( CanContinue() && _messagesQueue.Count == 0 )
					{
						SysTask.Monitor.Wait( _syncHandle );
					}
					if ( _isRunning == true )
					{
						return _messagesQueue.Dequeue();
					}
					else
					{
						return null;
					}
				}
			}

			// 是否可以繼續執行？
			private bool CanContinue()
			{
				lock ( _syncHandle )
				{
					return _isRunning;
				}
			}
		}
	}
}
