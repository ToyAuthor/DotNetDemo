using SysForm = System.Windows.Forms;

namespace LostUtility
{
	public class Utils
	{
		// 設定右鍵能出現什麼
		static public SysForm.ToolStripMenuItem MakeMenuItem( string title, SysForm.MouseEventHandler act, bool isMarked = false )
		{
			var item = new SysForm.ToolStripMenuItem();

			item.Text = title;
			item.MouseUp += act;

			// 先預留這招
			if ( isMarked )
			{
				item.Checked = true;
			}

			return item;
		}

		// 用來裝 sub menu 的版本
		static public SysForm.ToolStripMenuItem MakeMenuItem( string title, params SysForm.ToolStripMenuItem[] values )
		{
			var item = new SysForm.ToolStripMenuItem();

			item.Text = title;

			for ( int i = 0 ; i < values.Length ; i++ )
			{
				item.DropDownItems.Add( values[i] );
			}

			return item;
		}
	}
}
