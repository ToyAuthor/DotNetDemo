using SysDLL = System.Runtime.InteropServices;

namespace LostUtility
{
	class WinAPI
	{
		public const int WM_SYSCOMMAND = 0x112;
		public const uint MF_BYPOSITION = 0x400;

		public const int GWL_STYLE = -16;
		public const int WS_VISIBLE = 0x10000000;
		public const int WS_CHILD = 0x40000000;

		[SysDLL.DllImport( "user32.dll" )]
		static public extern System.IntPtr GetSystemMenu( System.IntPtr hWnd, bool bRevert );

		[SysDLL.DllImport( "user32.dll" )]
		static public extern bool InsertMenu( System.IntPtr hMenu, uint wPosition, uint wFlags, uint wIDNewItem, string lpNewItem );

		// 用來釋放 Bitmap 的記憶體資源，應該用不到才對
		[SysDLL.DllImport( "gdi32.dll" )]
		static public extern bool DeleteObject( System.IntPtr hObject );

		[SysDLL.DllImport( "user32.dll", SetLastError = true )]
		static public extern int SetParent( System.IntPtr hWndChild, System.IntPtr hWndNewParent );

		[SysDLL.DllImport( "user32.dll", EntryPoint = "SetWindowLong" )]
		static public extern int SetWindowLong( System.IntPtr hWnd, int nIndex, int dwNewLong );

		[SysDLL.DllImport( "user32.dll" )]
		static public extern bool MoveWindow( System.IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool BRePaint );

		[SysDLL.DllImport( "user32.dll", EntryPoint = "DestroyWindow" )]
		static public extern bool DestroyWindow( System.IntPtr hWnd );

		[SysDLL.DllImport( "msvcrt.dll", SetLastError = false )]
		static public extern System.IntPtr Memcpy( System.IntPtr dest, System.IntPtr src, int count );

		// https://help4windows.com/windows_7_shell32_dll.shtml
		// 依給定 ID 來選擇想要的 icon
		// Usage:
		//     WinAPI.ExtractIcon( "shell32.dll", 83, true ).ToBitmap();
		//
		// shell32.dll、imageres.dll、vault.dll、mmcndmgr.dll 都可以選
		// 有時候也能用 System.Drawing.SystemIcons.Information.ToBitmap();
		static public System.Drawing.Icon ExtractIcon( string file, int index, bool isLargeIcon )
		{
			System.IntPtr large;
			System.IntPtr small;

			ExtractIconEx( file, index, out large, out small, 1 );

			try
			{
				return System.Drawing.Icon.FromHandle( isLargeIcon ? large : small );
			}
			catch
			{
				return null;
			}
		}

		[SysDLL.DllImport( "Shell32.dll", EntryPoint = "ExtractIconExW", CharSet = SysDLL.CharSet.Unicode, ExactSpelling = true, CallingConvention = SysDLL.CallingConvention.StdCall )]
		static private extern int ExtractIconEx( string sFile, int iIndex, out System.IntPtr piLargeVersion, out System.IntPtr piSmallVersion, int amountIcons );
	}
}
