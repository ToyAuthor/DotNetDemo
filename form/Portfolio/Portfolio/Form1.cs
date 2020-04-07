using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Xml;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Portfolio
{
    public partial class Form1 : Form
    {
        private Bitmap _image;
        private Pen _pen;
        private float _x, _y;
        private XmlDocument _doc = new XmlDocument();

        public Form1()
        {
            InitializeComponent();
            _image = new Bitmap(300, 270);
            pictureBox1.BackColor = Color.White;
            _pen = new Pen(Color.Black, 2);  // 指定畫筆的顏色與粗細
            richTextBox1.AppendText("這裡用來輸出資訊\r");

            //if (System.IO.Directory.Exists("setting.xml"))   //檔案跟資料夾要分開來測試
            if (System.IO.File.Exists("setting.xml"))
                {
                _doc.Load("setting.xml");
                richTextBox1.AppendText("讀取舊xml\r");
            }
            else
            {
                richTextBox1.AppendText("建立新的xml\r");
                var root = _doc.CreateElement("setting");
                _doc.AppendChild(root);

                var swith = _doc.CreateElement("switch");
                swith.SetAttribute("removeable", "true");
                swith.SetAttribute("test", "false");
                root.AppendChild(swith);
            }

            var main = _doc.SelectSingleNode("setting/switch");

            if (main == null)
            {
                richTextBox1.AppendText("讀xml時出了問題\r");
            }
            else
            {
                XmlElement element = (XmlElement)main;

                if ("true" == element.GetAttribute("removeable"))
                {
                    radioButton1.Checked = true;
                    //radioButton2.Checked = false;
                }
                else
                {
                    radioButton2.Checked = true;
                    //radioButton1.Checked = false;
                }
            }
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
            // Application.Exit();       // 關閉程式
            richTextBox1.AppendText(textBox1.Text + "\r");
        }

        // 開啟圖片檔
        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.AppendText("載入:"+ openFileDialog1.FileName+ "\r");
                var fs = new System.IO.FileStream(openFileDialog1.FileName, System.IO.FileMode.Open);
                _image = new Bitmap(fs);
                fs.Close();
                pictureBox1.Image = _image;
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            _x = e.X;
            _y = e.Y;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "點陣圖 (*.bmp)|*.bmp|JPEG (*.JPG)|*.JPG|" + "GIF(*.GIF)| *. GIF|All File (*.*)|*.*";

            // 執行saveFileDialog1.ShowDialog()，將圖案物件的內容儲存到指定的檔案內
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.AppendText("儲存到:" + saveFileDialog1.FileName + "\r");
                _image.Save(saveFileDialog1.FileName);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _doc.Save("setting.xml");
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            var main = _doc.SelectSingleNode("setting/switch");
            if (main == null)
                return;
            //取得節點內的欄位
            XmlElement element = (XmlElement)main;

            if (radioButton1.Checked == true)
                element.SetAttribute("removeable", "true");
            else
                element.SetAttribute("removeable", "false");

            /* 全部屬性一起挑出來處理
            XmlAttributeCollection attributes = element.Attributes;

            foreach (XmlAttribute item in attributes)
            {
                if (item.Name == "removeable")
                {
                    if (radioButton1.Checked == true)
                    {
                        item.Value = "true";
                    }
                    else
                    {
                        item.Value = "false";
                    }
                }
            }*/
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //宣告畫布的來源是bmp圖案物件
            var g = Graphics.FromImage(_image);

            //如果按下滑鼠左鍵時
            if (e.Button == MouseButtons.Left)
            {
                //隨指標移動不斷在畫布上(圖案物件)畫短點直線
                g.DrawLine(_pen, _x, _y, e.X, e.Y);
                //用圖片方塊picDraw來顯示畫布(圖案物件)的內容
                pictureBox1.Image = _image;
                _x = e.X;
                _y = e.Y;
            }
        }
    }
}
