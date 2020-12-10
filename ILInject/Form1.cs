using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HelloLog;
namespace ILInject
{
    public partial class ILInject : Form
    {
        public ILInject()
        {
            InitializeComponent();
        }

        public delegate void InvokeMsg0(HelloLog.Task x, CompetedEventArgs args);
        public void ShowOneStartMsg(HelloLog.Task x, CompetedEventArgs args)
        {
            if (this.textBox1.InvokeRequired)
            {
                InvokeMsg0 msgCallback = new InvokeMsg0(ShowOneStartMsg);
                textBox1.Invoke(msgCallback, new object[] { x , args });
            }
            else
            {

                textBox1.Text += x.object_name + " begin!"+ "  线程id：" + Convert.ToString(args.id)+Environment.NewLine;
            }
        }

        public delegate void InvokeMsg2(CompetedEventArgs args);
        public void ShowAllDoneMsg(CompetedEventArgs args)
        {
            if (this.textBox1.InvokeRequired)
            {
                InvokeMsg2 msgCallback = new InvokeMsg2(ShowAllDoneMsg);
                textBox1.Invoke(msgCallback, new object[] { args });
            }
            else
            {
                textBox1.Text += "完成率：" + Convert.ToString(args.CompetedPrecent) + "%  All Job finished!" + Environment.NewLine;
            }
        }



        public delegate void InvokeMsg1(HelloLog.Task x, CompetedEventArgs args);
        public void ShowOneDoneMsg(HelloLog.Task x, CompetedEventArgs args)
        {
            if (this.textBox1.InvokeRequired)
            {
                InvokeMsg1 msgCallback = new InvokeMsg1(ShowOneDoneMsg);
                textBox1.Invoke(msgCallback, new object[] { x, args });
            }
            else
            {

                textBox1.Text += x.object_name + " finished!" + "  线程id：" + Convert.ToString(args.id) 
                    + "  " + Environment.NewLine;

               
            }
        }

        //消息回调响应
        public delegate void InvokeobjMsg(String str);
        public void ShowObjManagerMsg(String str)
        {
            if (this.textBox1.InvokeRequired)
            {
                InvokeobjMsg msgCallback = new InvokeobjMsg(ShowObjManagerMsg);
                textBox1.Invoke(msgCallback, new object[] {str});
            }
            else
            {

                textBox1.Text += str + "  " + Environment.NewLine;


            }
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {

            ObManager.Instance.ObjManagerMessage += ShowObjManagerMsg;

            Register register = new Register();
            register.RegisterUser(ObManager.Instance);
            GCHandle h = GCHandle.Alloc(ObManager.Instance);

            IntPtr addr = GCHandle.ToIntPtr(h);
            
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
            String Class_names = textBox_class.Text;
            String[] str =null;
            if (checkBox_inputclass.Checked)
            {
                str = Class_names.Split(';');
            }

            string path = (string)textBox_getpath.Tag+"\\";
            string ret = Inject.InjectIntoCSharp(path + name+".dll", path + name+"_inject" +
                ".dll", str);
            ret += "\r\nInject OK!";
            LogStr(ret);
        }

        private void checkBox_inputclass_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_inputclass.Checked)
            {
                textBox_class.Enabled = true;
            }
            else
            {
                textBox_class.Enabled = false;
            }
        }

        private void textBox_class_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
