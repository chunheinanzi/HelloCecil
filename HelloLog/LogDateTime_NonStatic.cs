using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using System.Threading;
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

    public class LogDateTime_NonStatic

    {

        public int abc = 10;
        private int def = 90;

        public object obj_;

        public void LogDT(String tag, String data)
        {
            Debug.Log(this.ToString() + "////" + tag + "-: " + data);
        }



        public void LogDT_Field(object obj)
        {
            obj_ = obj;
            Thread thread = new Thread(new ThreadStart(FieldDump));//创建线程

            thread.Start();                                                           //启动线程

        }



        public void FieldDump()
        {

            Thread.Sleep(5*1000);
            object logbase = (object)obj_;

            LogDT(" Date", DateTime.Now.ToString());
            LogDT(" namespace", logbase.GetType().Namespace);//获取当前类名
            LogDT(" class", logbase.GetType().Name);//获取当前类名\
            String addr = String.Format("{0:X2}", obj_);

            LogDT(" addr", addr);
            FieldInfo[] infos = logbase.GetType().GetFields();//获取类中字段


            string Fields = "";

            foreach (FieldInfo item in infos)//遍历类中字段并赋值
                                             //foreach (PropertyInfo item in properties)
            {
                string name = item.Name; //名称
                object value = item.GetValue(logbase);  //值
                Fields += string.Format("[Fields_name == {0}: {1} -- {2}],", name, value, value.GetType());

                if (name.Equals("score"))
                {
                    item.SetValue(logbase, 900);
                    Fields += "\n";
                    Fields += string.Format("[new][Fields_name == [{0}:{1} -- {2}]\n", item.Name, item.GetValue(logbase), item.GetValue(logbase).GetType());
                }

                //if (value.GetType() == typeof(Int32))
                //{
                //    item.SetValue(logbase,32);
                //}
                //else if (value.GetType() == typeof(String))
                //{
                //    item.SetValue(logbase, "String");
                //}
               
                Fields += "\n=========================\n";

            }
            LogDT("Fields", Fields);
        }

    }
}
