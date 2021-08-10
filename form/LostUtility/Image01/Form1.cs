using SysForm = System.Windows.Forms;
using Tools = LostUtility;

namespace Image01
{
	public partial class Form1 : SysForm.Form
	{
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

		private void button1_Click( object sender, System.EventArgs e )
		{
			Tools.Debug.Print( "Ans:" + Tools.Image.StringFromCpp( "haha" ) );
		}
	}
}
