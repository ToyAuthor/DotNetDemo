using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Portfolio
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            richTextBox1.AppendText("這裡用來輸出資訊\r");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }
    }
}
