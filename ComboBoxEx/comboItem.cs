using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ComboBoxEx
{
    public class comboItem : object
    {
        public Image Img { get; set; }
        public string Text { get; set; }

        public comboItem()
        {

        }

        public comboItem(string text)
        {
            Text = text;
        }

        public comboItem(string text, Image img)
        {
            Text = text;
            Img = img;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
