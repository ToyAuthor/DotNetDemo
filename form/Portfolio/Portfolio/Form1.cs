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
        private Bitmap _image;

        public Form1()
        {
            InitializeComponent();
            radioButton1.Checked = true;
            richTextBox1.AppendText("這裡用來輸出資訊\r");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            { 
                richTextBox1.Clear();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.AppendText(textBox1.Text + "\r");
        }

        // 開啟圖片檔
        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.AppendText("載入:"+ openFileDialog1.FileName+ "\r");
                System.IO.FileStream fs = new System.IO.FileStream(openFileDialog1.FileName, System.IO.FileMode.Open);
                _image = new Bitmap(fs);
                fs.Close();
                pictureBox1.Image = _image;
            }
        }
    }
}
