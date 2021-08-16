using SysForm = System.Windows.Forms;
using SysGraph = System.Drawing;
using Tools = LostUtility;

namespace PictureBox01
{
	public partial class Form1 : SysForm.Form
	{
		private Tools.PictureBox _pictureBox;
		private SysGraph.Bitmap _image;

		private void Print( string str )
		{
			BeginInvoke( (System.Action)(() =>
			{
				richTextBox1.AppendText( str + System.Environment.NewLine );
				richTextBox1.Update();
			}) );
		}

		public Form1()
		{
			InitializeComponent();
			Tools.Debug.SetPrinter( Print );
		}

		private void Form1_Load( object sender, System.EventArgs e )
		{
			_pictureBox = new Tools.PictureBox( pictureBox1, vScrollBar1, hScrollBar1 );
			InitPictureBoxMenu( _pictureBox );
		}

		static private void InitPictureBoxMenu( Tools.PictureBox frame )
		{
			var menu = new SysForm.ContextMenuStrip();

			menu.Items.Add( Tools.Utils.MakeMenuItem( "手動更新", ( object sender, SysForm.MouseEventArgs e ) => frame.Update() ) );
			menu.Items.Add( Tools.Utils.MakeMenuItem( "縮放比例",
								Tools.Utils.MakeMenuItem( "Auto", ( object sender, SysForm.MouseEventArgs e ) => frame.FitFrame() ),
								Tools.Utils.MakeMenuItem( "50%", ( object sender, SysForm.MouseEventArgs e ) => frame.SetRatio( 0.5 ) ),
								Tools.Utils.MakeMenuItem( "100%", ( object sender, SysForm.MouseEventArgs e ) => frame.SetRatio( 1 ) ),
								Tools.Utils.MakeMenuItem( "150%", ( object sender, SysForm.MouseEventArgs e ) => frame.SetRatio( 1.5 ) )
				) );
			menu.Items.Add( Tools.Utils.MakeMenuItem( "說明", ( object sender, SysForm.MouseEventArgs e ) => SysForm.MessageBox.Show( "只是個 PictureBox" ) ) );

			frame.GetCore().ContextMenuStrip = menu;
		}

		// 載入圖片
		private void button1_Click( object sender, System.EventArgs e )
		{
			var dialog = new SysForm.OpenFileDialog();

			if ( dialog.ShowDialog() == SysForm.DialogResult.OK )
			{
				using ( var fs = new System.IO.FileStream( dialog.FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read ) )
				{
					_image = new SysGraph.Bitmap( fs );

					_pictureBox.SetRawImage( _image );
					_pictureBox.FitFrame();
					_pictureBox.Update();
				}
			}
		}

		private void vScrollBar1_ValueChanged( object sender, System.EventArgs e )
		{
			_pictureBox.Render();
		}

		private void hScrollBar1_ValueChanged( object sender, System.EventArgs e )
		{
			_pictureBox.Render();
		}
	}
}
