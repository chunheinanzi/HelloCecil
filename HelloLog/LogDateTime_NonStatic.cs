using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using System.Threading;
using System.Runtime.InteropServices;
/*1.通过Cecil可以完成插码
2.自定义代码封装成DLL，与插码后程序放在一起执行
3.可以通过反射获取类中字段（public）*/
/*自定义模块DLL，等待插入的代码*/
namespace HelloLog
{

    public class LogBase
    {


    }


    class LogOne : LogBase
    {
        public List<int> list = new List<int>();
        public int abc = 10;
        public string qwe = "qwe == LogOne : LogBase";
        public static Int64 index;
        protected static Int64 pro;
        private int def { get; set; }
    }


    public class Register
    {
        public void RegisterUser(object obj)
        {
            LogDateTime_NonStatic.Instance.LogDT_Field(obj);
        }
    }

    public sealed  class LogDateTime_NonStatic

    {
        private List<object> item_list_;

        public object obj_;

        private static readonly LogDateTime_NonStatic instance = new LogDateTime_NonStatic();
       

        public static LogDateTime_NonStatic Instance
        {
            get
            {
                return instance;
            }
        }

        private LogDateTime_NonStatic() {
            item_list_ = new List<object>();
            Thread thread = new Thread(new ThreadStart(FieldDump));//创建线程

            thread.Start();
           
        }

        public void LogDT(String tag, String data)
        {
            Debug.Log(this.ToString() + "////" + tag + "-: " + data);
        }



        public void LogDT_Field(object obj)
        {
            obj_ = obj;
            if (obj is null)
            {
                return;
            }
            item_list_.Add(obj);
                                                   //启动线程

        }

        public string getMemory(object o) // 获取引用类型的内存地址方法    
        {
            GCHandle h = GCHandle.Alloc(o, GCHandleType.WeakTrackResurrection);

            IntPtr addr = GCHandle.ToIntPtr(h);

            return "0x" + addr.ToString("X");
        }


        public void FieldDump()
        {
            do {
                Debug.Log("LogDateTime_NonStatic " + "Thread Start");
                Thread.Sleep(10*1000);


                foreach (object obj in item_list_)
            {

                object logbase = (object)obj;

                LogDT(" Class Info ", string.Format("Date[{0}],namespace[{1}],class[{2}],hashcode[{3}]",
                     DateTime.Now.ToString(), 
                     logbase.GetType().Namespace, 
                     logbase.GetType().Name,
                     logbase.GetHashCode()));

                    //try {
                    //    FieldInfo[] infos = logbase.GetType().GetFields();//获取类共有中字段
                    //    foreach (FieldInfo item in infos)//遍历类中字段并赋值
                    //                                     //foreach (PropertyInfo item in properties)
                    //    {
                    //        string Fields = "public number：\n";
                    //        string name = item.Name; //名称
                    //        object value = item.GetValue(logbase);  //值
                    //        Fields += string.Format("[Fields_name == {0}: {1} -- {2}],", name, value, value.GetType());

                    //        //if (name.Equals("score"))
                    //        //{
                    //        //    item.SetValue(logbase, 900);
                    //        //    Fields += "\n";
                    //        //    Fields += string.Format("[new][Fields_name == [{0}:{1} -- {2}]\n",
                    //        //        item.Name, item.GetValue(logbase), item.GetValue(logbase).GetType());
                    //        //}

                    //        //Fields += "\n=========================\n";
                    //        // LogDT("Fields", Fields);
                    //    }


                    //    //获取非共有成员

                    //    BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;

                    //    infos = logbase.GetType().GetFields(flag);//获取类共有中字段

                    //    foreach (FieldInfo item in infos)//遍历类中字段并赋值
                    //                                     //foreach (PropertyInfo item in properties)
                    //    {
                    //        string Fields = "non public number：\n";
                    //        string name = item.Name; //名称
                    //        object value = item.GetValue(logbase);  //值
                    //        Fields += string.Format("[Fields_name == {0}: {1} -- {2}],", name, value, value.GetType());

                    //        //if (name.Equals("score"))
                    //        //{
                    //        //    item.SetValue(logbase, 900);
                    //        //    Fields += "\n";
                    //        //    Fields += string.Format("[new][Fields_name == [{0}:{1} -- {2}]\n",
                    //        //        item.Name, item.GetValue(logbase), item.GetValue(logbase).GetType());
                    //        //}

                    //        //Fields += "\n=========================\n";
                    //        // LogDT("Fields", Fields);

                    //    }
                    //} catch {

                    //    LogDT("FieldDump err", string.Format("Date[{0}],namespace[{1}],class[{2}],hashcode[{3}]"));
                    //}

                    

                }
            } while (true);
        }

    }
}
