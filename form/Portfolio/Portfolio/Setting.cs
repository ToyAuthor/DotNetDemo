using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;

namespace Portfolio
{
    // 外部呼叫這物件查詢想知道的資訊就好
    // 其他的都交給內部決定
    class Setting
    {
        private XmlDocument _doc = new XmlDocument();
        private Action<string> _print;

        public bool removeable
        {
            get
            {
                var main = _doc.SelectSingleNode("setting/switch");

                if (main == null)
                {
                    _print("讀xml時出了問題");
                }

                var element = (XmlElement)main;

                if ("true" == element.GetAttribute("removeable"))
                {
                    return true;
                }

                return false;
            }
            set
            {
                var main = _doc.SelectSingleNode("setting/switch");

                if (main == null)
                {
                    _print("讀取舊xml");
                    return;
                }

                //取得節點內的欄位
                var element = (XmlElement)main;

                if (value == true)
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
        }

        public Setting(Action<string> printer)
        {
            _print = printer;

            //if (System.IO.Directory.Exists("setting.xml"))   //檔案跟資料夾要分開來測試
            if (System.IO.File.Exists("setting.xml"))
            {
                _doc.Load("setting.xml");
                _print("讀取舊xml");
            }
            else
            {
                _print("建立新的xml");
                var root = _doc.CreateElement("setting");
                _doc.AppendChild(root);

                var swith = _doc.CreateElement("switch");
                swith.SetAttribute("removeable", "true");
                swith.SetAttribute("test", "false");
                root.AppendChild(swith);
            }
        }

        public void save()
        {
            _doc.Save("setting.xml");
        }
    }
}
