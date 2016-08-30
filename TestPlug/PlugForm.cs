using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PlugsRoot;

namespace PlugTool
{
    [PluginInfo("动态插件", "1.0.0.2012", "作者", "分组", false)]
    public partial class PlugForm : PlugRoot
    {
        public PlugForm()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            application.MyStatusStrip.Items.Add("插件被点击了；");
            application.MyStatusStrip.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            application.MyStatusStrip.Items.Clear();
            application.MyStatusStrip.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            application.AddCommand("127.0.0.1:connect_one_server{@10.34.130.44@}");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string s = "返回值：\r\n";
            for (int x = 0; x < application.aReturn.Count; x++)
            {
                s += application.aReturn[x].ToString() + "\r\n";
            }
            MessageBox.Show(s, "提示");
        }
    }
}
