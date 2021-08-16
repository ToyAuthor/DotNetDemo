
using SysForm = System.Windows.Forms;
using SysGraph = System.Drawing;

namespace LostUtility
{
	/*
	 * 做一個 PictureBox 的額外包裝
	 * 目的在於更快速的處理特大尺寸圖片
	 * 附帶縮放倍率可用
	 */
	public class PictureBox
	{
		private SysGraph.Bitmap _raw;  // 原始圖片
		private SysGraph.Bitmap _canvas = new SysGraph.Bitmap( 1, 1 );   // 做為 SysForm.PictureBox 的畫布

		private SysGraph.Graphics _pen_canvas;       // 對著 _canvas 作畫
		private SysGraph.Graphics _pen_pictureBox;   // 直接控制 SysForm.PictureBox

		private SysGraph.BufferedGraphics _bufferT;  // _pen_pictureBox 的 buffer，會對著 _buffer 作畫
		private SysGraph.BufferedGraphics _buffer;   // _pen_pictureBox 的 buffer，會定期更新

		private SysForm.PictureBox _frame;
		private SysForm.VScrollBar _vb;
		private SysForm.HScrollBar _hb;

		private bool _vb_visible = false;
		private bool _hb_visible = false;

		private double _ratio = 1;

		public PictureBox( SysForm.PictureBox frame, SysForm.VScrollBar vb, SysForm.HScrollBar hb )
		{
			_frame = frame;
			_vb = vb;
			_hb = hb;

			_pen_pictureBox = _frame.CreateGraphics();
			設定繪圖物件( _frame.Width, _frame.Height );
		}

		public SysForm.PictureBox GetCore()
		{
			return _frame;
		}

		public bool VS_Visible()
		{
			return _vb_visible;
		}

		public bool HS_Visible()
		{
			return _hb_visible;
		}

		// 將最原始的圖片擺進來
		public void SetRawImage( SysGraph.Bitmap image )
		{
			_raw = image;
		}

		public void SetRawImage( SysGraph.Image image )
		{
			_raw = new SysGraph.Bitmap( image );
		}

		public SysGraph.Bitmap GetRawImage()
		{
			return _raw;
		}

		// 依據縮放過的長寬來設定繪圖物件以及緩衝繪圖物件
		private void 設定繪圖物件( int tempWidth, int tempHeight )
		{
			if ( (tempWidth != _canvas.Width || tempHeight != _canvas.Height) && (tempWidth <= _frame.Width || tempHeight <= _frame.Height || (tempWidth > _frame.Width && _canvas.Width < _frame.Width) || (tempHeight > _frame.Height && _canvas.Height < _frame.Height)) )
			{
				_canvas.Dispose();

				_canvas = new SysGraph.Bitmap( tempWidth, tempHeight );

				_pen_canvas = SysGraph.Graphics.FromImage( _canvas );

				_frame.Image = _canvas;

				//Graphics_BmpDisplay.InterpolationMode = ZoomTable[ ZoomIndex ] <= 1 ? SysGraph.Drawing2D.InterpolationMode.NearestNeighbor : SysGraph.Drawing2D.InterpolationMode.HighQualityBicubic;

				_pen_canvas.SmoothingMode = SysGraph.Drawing2D.SmoothingMode.None;

				if ( _bufferT != null ) _bufferT.Dispose();

				if ( _buffer != null ) _buffer.Dispose();

				_bufferT = SysGraph.BufferedGraphicsManager.Current.Allocate( _pen_pictureBox, new SysGraph.Rectangle( 0, 0, _canvas.Width, _canvas.Height ) );
				_buffer = SysGraph.BufferedGraphicsManager.Current.Allocate( _pen_pictureBox, new SysGraph.Rectangle( 0, 0, _canvas.Width, _canvas.Height ) );

				_buffer.Graphics.SmoothingMode = SysGraph.Drawing2D.SmoothingMode.AntiAlias;
			}

			_pen_canvas.InterpolationMode = SysGraph.Drawing2D.InterpolationMode.NearestNeighbor; // ZoomTable[ZoomIndex] <= 1 ? SysGraph.Drawing2D.InterpolationMode.NearestNeighbor : SysGraph.Drawing2D.InterpolationMode.Bicubic;

			_pen_canvas.SmoothingMode = SysGraph.Drawing2D.SmoothingMode.None;
		}

		// 依據縮放過的長寬來設定捲軸
		private void 設定捲軸( int tempWidth, int tempHeight )
		{
			if ( tempWidth > _frame.Width )
			{
				tempWidth = _frame.Width;
				_hb.Visible = true;
				_hb_visible = true;
			}
			else
			{
				_hb.Visible = false;
				_hb_visible = false;
			}

			if ( tempHeight > _frame.Height )
			{
				tempHeight = _frame.Height;
				_vb.Visible = true;
				_vb_visible = true;
			}
			else
			{
				_vb.Visible = false;
				_vb_visible = false;
			}

			double ratio = _ratio;

			if ( _hb_visible )
			{
				_hb.Minimum = 0;
				_hb.Maximum = System.Convert.ToInt32( _raw.Width * ratio );
				_hb.LargeChange = _frame.Width;
			}

			_hb.Value = 0;

			if ( _vb_visible )
			{
				_vb.Minimum = 0;
				_vb.Maximum = System.Convert.ToInt32( _raw.Height * ratio );
				_vb.LargeChange = _frame.Height;
			}

			_vb.Value = 0;
		}

		private double CalFitRatio()
		{
			double pw = _frame.Width;
			double ph = _frame.Height;
			double cw = _raw.Width;
			double ch = _raw.Height;

			if ( (pw / ph) < (cw / ch) )
			{
				return pw / cw;
			}
			else
			{
				return ph / ch;
			}
		}

		// 自己計算適合畫面的尺寸
		public void FitFrame()
		{
			_ratio = CalFitRatio();
			SetRatio( _ratio );
		}

		// 指定縮放比例
		public void SetRatio( double ratio )
		{
			if ( _raw == null ) return;

			_ratio = ratio;

			int tempWidth = System.Convert.ToInt32( _raw.Width * ratio );
			int tempHeight = System.Convert.ToInt32( _raw.Height * ratio );

			設定繪圖物件( tempWidth, tempHeight );

			設定捲軸( tempWidth, tempHeight );

			Render();
		}

		public void Render()
		{
			if ( _raw == null ) return;

			double ratio = _ratio;

			_pen_canvas.DrawImage(
				_raw,
				new SysGraph.Rectangle( 0, 0, _canvas.Width, _canvas.Height ),
				new SysGraph.Rectangle(
					System.Convert.ToInt32( _hb.Value / ratio ),
					System.Convert.ToInt32( _vb.Value / ratio ),
					System.Convert.ToInt32( _canvas.Width / ratio ),
					System.Convert.ToInt32( _canvas.Height / ratio ) ),
				SysGraph.GraphicsUnit.Pixel );

			_bufferT.Graphics.DrawImageUnscaled( _canvas, 0, 0 );

			_bufferT.Render( _buffer.Graphics );

			_buffer.Render();
		}

		public void Update()
		{
			調整卷軸位置及長度();

			SetRatio( _ratio );
		}

		public void 調整卷軸位置及長度()
		{
			var posv = _vb.Location;
			posv.X = _frame.Width;
			_vb.Location = posv;

			var posh = _hb.Location;
			posh.Y = _frame.Height;
			_hb.Location = posh;

			var sizev = _vb.Size;
			sizev.Height = _frame.Height - 17;
			_vb.Size = sizev;

			var sizeh = _hb.Size;
			sizeh.Width = _frame.Width - 17;
			_hb.Size = sizeh;
		}
	}
}
