using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ILInject
{
    public partial class ILInject : Form
    {
        public ILInject()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void LogStr(String data)

        {
            textBox1.Clear();
            textBox1.AppendText(DateTime.Now.ToString("HH:mm:ss ") + data + "\r\n");
        }
        private void button_getpath_Click(object sender, EventArgs e)
            
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "请选择要打开的DLL文件";
            openFileDialog.Filter = "*.dll|*.dll|所有文件(*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string Name = openFileDialog.FileName;
                   
                    textBox_getpath.Tag = Path.GetDirectoryName(Name);
                    textBox_getpath.Text = Name;
                    



                }
                catch (Exception) { }
            }
        }

        private void button_inject_Click(object sender, EventArgs e)
        {
            textBox1.Clear();

            String name = Path.GetFileNameWithoutExtension(textBox_getpath.Text);

            string path = (string)textBox_getpath.Tag+"\\";
            string ret = Inject.InjectIntoCSharp(path + name+".dll", path + name+"_inject" +
                ".dll");
            ret += "\r\nInject OK!";
            LogStr(ret);
        }
        
    }
}
