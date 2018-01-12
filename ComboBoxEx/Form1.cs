using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

//C# winform combobox控件中子项加删除按钮
namespace ComboBoxEx
{
    public partial class Form1 : Form
    {
        [DllImport("user32")]
        private static extern int GetComboBoxInfo(IntPtr hwnd, out COMBOBOXINFO comboInfo);
        struct RECT
        {
            public int left, top, right, bottom;
        }
        struct COMBOBOXINFO
        {
            public int cbSize;
            public RECT rcItem;
            public RECT rcButton;
            public int stateButton;
            public IntPtr hwndCombo;
            public IntPtr hwndItem;
            public IntPtr hwndList;
        }

        public Form1()
        {
            InitializeComponent();
            comboBox1.HandleCreated += (s, e) =>
            {
                COMBOBOXINFO combo = new COMBOBOXINFO();
                combo.cbSize = Marshal.SizeOf(combo);
                GetComboBoxInfo(comboBox1.Handle, out combo);
                hwnd = combo.hwndList;
                init = false;
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.DrawMode = DrawMode.OwnerDrawFixed;
            comboBox1.Items.Add(new comboItem("sgset", pictureBox1.Image));
            comboBox1.Items.Add(new comboItem("sdgsdg", pictureBox1.Image));
            comboBox1.Items.Add(new comboItem("sdgsdg", pictureBox1.Image));
        }

        bool init;
        IntPtr hwnd;
        NativeCombo nativeCombo = new NativeCombo();
        //This is to store the Rectangle info of your Icons
        //Key:  the Item index
        //Value: the Rectangle of the Icon of the item (not the Rectangle of the item)
        Dictionary<int, Rectangle> dict = new Dictionary<int, Rectangle>();
        public class NativeCombo : NativeWindow
        {
            //this is custom MouseDown event to hook into later
            public event MouseEventHandler MouseDown;
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == 0x201)//WM_LBUTTONDOWN = 0x201
                {
                    int x = m.LParam.ToInt32() & 0x00ff;
                    int y = m.LParam.ToInt32() >> 16;
                    if (MouseDown != null) MouseDown(null, new MouseEventArgs(MouseButtons.Left, 1, x, y, 0));
                }
                base.WndProc(ref m);
            }
        }
        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {

            if ((e.State & DrawItemState.Selected) != 0)//鼠标选中在这个项上
            {
                //渐变画刷
                LinearGradientBrush brush = new LinearGradientBrush(e.Bounds, Color.FromArgb(255, 251, 237),
                                                 Color.FromArgb(255, 236, 181), LinearGradientMode.Vertical);
                //填充区域
                Rectangle borderRect = new Rectangle(0, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height - 2);
                e.Graphics.FillRectangle(brush, borderRect);
                //画边框
                Pen pen = new Pen(Color.FromArgb(229, 195, 101));
                e.Graphics.DrawRectangle(pen, borderRect);
            }
            else
            {
                SolidBrush brush = new SolidBrush(Color.FromArgb(217, 223, 230));
                e.Graphics.FillRectangle(brush, e.Bounds);
            }
            //获得项图片,绘制图片
            comboItem item = (comboItem)comboBox1.Items[e.Index];
            Image img = item.Img;

            //图片绘制的区域
            Rectangle imgRect = new Rectangle(e.Bounds.Width - 18, e.Bounds.Y, 15, 15);
            dict[e.Index] = imgRect;
            if (img != null && (e.State & DrawItemState.Selected) != 0)
            {
                e.Graphics.DrawImage(img, imgRect);
            }
            Rectangle textRect =
                new Rectangle(10, imgRect.Y, e.Bounds.Width - imgRect.Width, e.Bounds.Height + 2);
            string itemText = comboBox1.Items[e.Index].ToString();
            StringFormat strFormat = new StringFormat();
            strFormat.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawString(itemText, new Font("宋体", 14), Brushes.Black, textRect, strFormat);

        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            if (!init)
            {
                //Register the MouseDown event handler <--- THIS is WHAT you want.
                nativeCombo.MouseDown += comboListMouseDown;
                nativeCombo.AssignHandle(hwnd);
                init = true;
            }
        }

        //This is the MouseDown event handler to handle the clicked icon
        private void comboListMouseDown(object sender, MouseEventArgs e)
        {
            foreach (var kv in dict)
            {
                if (kv.Value.Contains(e.Location))
                {
                    //Show the item index whose the corresponding icon was held down
                    //MessageBox.Show(kv.Key.ToString());
                    return;
                }
            }
        }
    }
}