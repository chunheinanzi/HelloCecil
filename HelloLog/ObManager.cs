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
    //对象引用注册类
    public class Register
    {
        public void RegisterUser(object obj)
        {
            ObManager.Instance.LogDT_Field(obj);
        }
    }
    //内存对象管理类
    public  sealed class ObManager :Iwork

    {
        string chunheinanzi = "zyz";
        int age = -1;
        public List<object> listitem_;
        public QueueThreadBase queueb_ = new QueueThreadBase();

        private static readonly ObManager instance = new ObManager();
       

        public static ObManager Instance
        {
            get
            {
                return instance;
            }
        }

        private ObManager() {
            listitem_ = new List<object>();
            //启动任务队列
            queueb_.OneCompleted += ShowOneDoneMsg;
            queueb_.Start( this);

        }

        public void DoWork(Task pendingValue)
        {
            FieldDump(pendingValue);
        }


        public void LogDT(String tag, String data)
        {
            Debug.Log(this.ToString() + "////" + tag + "-: " + data);
            OnObjManagerMessage (tag + "-: " + data);

        }

        //消息回调
        public event Action<string> ObjManagerMessage;
        private void OnObjManagerMessage(String msg)
        {
            if (ObjManagerMessage != null)
            {
                try
                {
                    
                    ObjManagerMessage(msg);//--一个任务开始了..
                }
                catch { }
            }

        }




        public void LogDT_Field(object obj)
        {
           
            if (obj is null)
            {
                return;
            }
            listitem_.Add(obj);
            queueb_.AddTask(new Task(obj));
                                                   //启动线程

        }

        public string getMemory(object o) // 获取引用类型的内存地址方法    
        {
            string str = "";
            int k;
            unsafe
            {
                TypedReference tr = __makeref(o);
                IntPtr ptr = **(IntPtr**)(&tr);
                

                int* j = (int*)&ptr;

                k = *j;
            }
           
            return "0x" + k.ToString("X");
        }


        public delegate void InvokeMsg1(HelloLog.Task x, CompetedEventArgs args);
        public void ShowOneDoneMsg(HelloLog.Task x, CompetedEventArgs args)
        {
            LogDT("thread Error", args.InnerException.ToString());
        }


        public void FieldDump(Task pendingValue)
        {
            do {
                LogDT("LogDateTime_NonStatic " ,"Thread Start queue cpunt :" +  queueb_.m_QueueCount);


               // foreach (object obj in listitem_)
                {

                object logbase = (object)pendingValue.object_name;

                    LogDT(" Class Info ", string.Format("Date[{0}],namespace[{1}],class[{2}],hashcode[0x{3}],addr[{4}]",
                         DateTime.Now.ToString(),
                         logbase.GetType().Namespace,
                         logbase.GetType().Name,
                         logbase.GetHashCode().ToString("X")
                    ,
                     getMemory(logbase)));
                    ;

                    //try {
                    FieldInfo[] infos = logbase.GetType().GetFields();//获取类共有中字段
                    foreach (FieldInfo item in infos)//遍历类中字段并赋值
                                                     //foreach (PropertyInfo item in properties)
                    {
                        string Fields = "public number：\r\n";
                        string name = item.Name; //名称
                        object value = item.GetValue(logbase);  //值
                  
                            
                        Fields += string.Format("[Fields_name == {0}: {1} -- type:{2}", name, value, value.GetType());
                       
                        Fields += "\r\n=========================\r\n";
                        LogDT("Fields", Fields);
                    }


                    //获取非共有成员

                    BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;

                    infos = logbase.GetType().GetFields(flag);//获取类共有中字段

                    
                    foreach (FieldInfo item in infos)//遍历类中字段并赋值
                                                     //foreach (PropertyInfo item in properties)
                    {
                        string Fields = "non public number：\r\n";
                        string name = item.Name; //名称
                        object value = item.GetValue(logbase);  //值
                        Fields += string.Format("[Fields_name == {0}: {1} -- type:{2} ", name, value, value.GetType());


                        Fields += "\r\n=========================\r\n";
                        LogDT("Fields", Fields);

                    }



                }
            } while (false);
        }

    }
}
