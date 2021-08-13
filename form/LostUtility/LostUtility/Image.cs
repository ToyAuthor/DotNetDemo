
/*

這邊的討論列了很多做法來獲取 byte array 的 IntPtr
https://stackoverflow.com/questions/537573/how-to-get-intptr-from-byte-in-c-sharp/23838643


// C++
class Data
{
	public:
		int m_Int;          // int
		char m_Str[32];     // 字串
		int m_IntAry[10];   // int陣列
};

// C#
[StructLayout(LayoutKind.Sequential, Pack = 4)]
[Serializable]
public class Data
{
    public int m_Int;                 // int
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string m_Str;              // 字串
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public int[] m_IntAry;            // int陣列
};

[DllImport(SysPluginBase.PlatformDllName)]
private static extern System.IntPtr Test_Func();

public Data GetData()
{
    System.IntPtr DataPtr = Test_Func();

    if (DataPtr == System.IntPtr.Zero)
    return null;

    Data Dta = (Data)Marshal.PtrToStructure(DataPtr, typeof(Data));
    return Dta;
}

// 結構中不要使用bool, 因為長度有可能是32bit, 用 byte 比較保險
*/


using SysContainer = System.Collections.Generic;
using SysGraph = System.Drawing;
using SysCpp = System.Runtime.InteropServices;

namespace LostUtility
{
	public class Image
	{
		// ARGB -> 4
		// RGB -> 3
		static private int GetPixelSize( SysGraph.Bitmap image )
		{
			// 因為傳出來的是 bit 數目而不是 byte
			// 所以要除以8
			return (SysGraph.Image.GetPixelFormatSize( image.PixelFormat )) >> 3;
		}

		static public SysGraph.Imaging.BitmapData LockImage( SysGraph.Bitmap image )
		{
			return image.LockBits( new SysGraph.Rectangle( 0, 0, image.Width, image.Height ), SysGraph.Imaging.ImageLockMode.ReadWrite, image.PixelFormat );
		}

		// http://www.ebaomonthly.com/window/photo/lesson/colorList.htm
		static public SysGraph.Color MakeColor( byte red, byte green, byte blue )
		{
			return SysGraph.Color.FromArgb( red, green, blue );
		}

		//-------------------------------------------------------------------------

		static private SysContainer.Dictionary<string, string>  _Types = new SysContainer.Dictionary<string, string>()
		{
			{ "FFD8", ".jpg" },
			{ "424D", ".bmp" },
			{ "474946", ".gif" },
			{ "89504E470D0A1A0A", ".png" }
		};

		// 查一下圖檔可能的副檔名
		static public string DetectFormat( string str )
		{
			string builtHex = string.Empty;

			using ( var S = System.IO.File.OpenRead( str ) )
			{
				for ( int i = 0 ; i < 8 ; i++ )
				{
					builtHex += S.ReadByte().ToString( "X2" );

					if ( _Types.ContainsKey( builtHex ) )
					{
						return _Types[builtHex];
					}
				}
			}

			return string.Empty;
		}

		//-------------------------------------------------------------------------

		[SysCpp.DllImport( @"C:\Users\User\Downloads\code\DotNetDemo\form\LostUtility\x64\Debug\CppImage.dll", EntryPoint = "cppimage_tset_string" )]
		private static extern System.IntPtr StrFormCpp( System.IntPtr str );

		static public string StringFromCpp( string str )
		{
			/*
			 * SysCpp.Marshal.StringToBSTR( str ) -> 轉成 wchar_t*              需要用 SysCpp.Marshal.FreeBSTR 收尾
			 * SysCpp.Marshal.StringToHGlobalAnsi( str ) -> UTF-8 的 char*      需要用 SysCpp.Marshal.FreeHGlobal 收尾
			 *
			 * Marshal.AllocHGlobal 跟 Marshal.FreeHGlobal 的搭配更常見
			 */
			//return SysCpp.Marshal.PtrToStringUni( StrFormCpp( SysCpp.Marshal.StringToBSTR( str ) ) );


			var temp = SysCpp.Marshal.StringToBSTR( str );

			string result = SysCpp.Marshal.PtrToStringUni( StrFormCpp( temp ) );

			SysCpp.Marshal.FreeBSTR( temp );

			return result;
		}

		//-------------------------------------------------------------------------

		// 不確定這樣做有沒有意義
		static public void Free( SysGraph.Bitmap image )
		{
			WinAPI.DeleteObject( image.GetHbitmap() );
		}

		static public void GetPixel( SysGraph.Bitmap image, int x, int y, Color color )
		{
			int width = image.Width;
			int height = image.Height;

			if ( (x < 1) || (y < 1) )
			{
				Debug.Oops();
				throw Debug.Exception();
			}

			if ( (x > width) || (y > height) )
			{
				Debug.Oops();
				throw Debug.Exception();
			}

			switch ( image.PixelFormat )
			{
				case SysGraph.Imaging.PixelFormat.Format8bppIndexed:
					Debug.Oops();//暫時還不支援
					break;

				case SysGraph.Imaging.PixelFormat.Format24bppRgb:
					{
						var data = LockImage( image );

						unsafe
						{
							byte* ptr = (byte*)data.Scan0;

							ptr += (data.Stride) * (y - 1);   // 先移到目標行
							ptr += (x - 1) * 3;

							// RGB 在 little - endian 裝置上的順序會是 BGR，這根本是陷阱吧?
							color.R = System.Convert.ToSingle( ptr[2] );
							color.G = System.Convert.ToSingle( ptr[1] );
							color.B = System.Convert.ToSingle( ptr[0] );
						}

						image.UnlockBits( data );

						break;
					}

				case SysGraph.Imaging.PixelFormat.Format32bppArgb:
					{
						var data = LockImage( image );

						unsafe
						{
							byte* ptr = (byte*)data.Scan0;

							ptr += (data.Stride) * (y - 1);   // 先移到目標行
							ptr += (x - 1) * 4;

							color.R = System.Convert.ToSingle( ptr[2] );
							color.G = System.Convert.ToSingle( ptr[1] );
							color.B = System.Convert.ToSingle( ptr[0] );
						}

						image.UnlockBits( data );

						break;
					}
				case SysGraph.Imaging.PixelFormat.Format32bppRgb:
					{
						var data = LockImage( image );

						unsafe
						{
							byte* ptr = (byte*)data.Scan0;

							ptr += (data.Stride) * (y - 1);   // 先移到目標行
							ptr += (x - 1) * 4;

							// 這裡假定不被使用的 byte 是在 blue 的旁邊
							color.R = System.Convert.ToSingle( ptr[3] );
							color.G = System.Convert.ToSingle( ptr[2] );
							color.B = System.Convert.ToSingle( ptr[1] );
						}

						image.UnlockBits( data );

						break;
					}
				default:
					throw Debug.Exception();
			}
		}
	}
}
