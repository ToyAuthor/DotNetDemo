using SysContainer = System.Collections.Generic;
using SysGraph = System.Drawing;

namespace LostUtility
{
	class Image
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
