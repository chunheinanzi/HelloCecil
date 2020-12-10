using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
//任务队列
namespace HelloLog
{
    public interface Iwork
    {
         void  DoWork(Task pendingValue);
    }

    public class Task
    {
        //存储对象引用
        public object object_name { get; set; }
        public Task(object str) {
            object_name = str;
        }

    }
    


    public class QueueThreadBase
    {
        #region 变量&属性

        /// 待处理结果

        private class PendingResult
        {
            /// 待处理值
            public Task PendingValue { get; set; }

            /// 是否有值
            public bool IsHad { get; set; }
        }

        /// 线程数
        public int ThreadCount
        {
            get { return this.m_ThreadCount; }
            set { this.m_ThreadCount = value; }
        }
        private int m_ThreadCount = 1;

        /// 取消=True
        public bool Cancel { get; set; }

        /// 线程列表
        List<Thread> m_ThreadList;

        /// 完成队列个数
        private volatile int m_CompletedCount = 0;

        /// 队列总数
        public int m_QueueCount = 0;

        /// 全部完成锁
        private object m_AllCompletedLock = new object();

        /// 完成的线程数

        private int m_CompetedCount = 0;

        /// 队列锁

        private object m_PendingQueueLock = new object();
        private ConcurrentQueue<Task> m_InnerQueue; //--内部队列..
                                               
        #endregion

        #region 事件相关
        //---开始一个任务的事件...
        public event Action<Task, CompetedEventArgs> OneJobStart;
        private void OnOneJobStart(Task pendingValue, CompetedEventArgs arg)
        {
            if (OneJobStart != null)
            {
                try
                {
                    //MessageBox.Show("所有任务完成！");
                    OneJobStart(pendingValue, arg);//--一个任务开始了..
                }
                catch { }
            }

        }



        /// 全部完成事件
        public event Action<CompetedEventArgs> AllCompleted;
        /// 单个完成事件
        public event Action<Task, CompetedEventArgs> OneCompleted;
        /// 引发全部完成事件
        private void OnAllCompleted(CompetedEventArgs args)
        {
            if (AllCompleted != null)
            {
                try
                {
                    //MessageBox.Show("所有任务完成！");
                    AllCompleted(args);//全部完成事件
                }
                catch { }
            }
        }

        /// 引发单个完成事件
        private void OnOneCompleted(Task pendingValue, CompetedEventArgs args)
        {
            if (OneCompleted != null)
            {
                try
                {
                    //MessageBox.Show("单个任务完成！");
                    OneCompleted(pendingValue, args);
                }
                catch { }
            }
        }
        #endregion

        #region 构造

     

        public QueueThreadBase(IEnumerable<Task> collection)
        {
            m_InnerQueue = new ConcurrentQueue<Task>(collection);
            this.m_QueueCount = m_InnerQueue.Count;
        }

        //--- 无参数的构造函数，需要向队列中填充元素...
        public QueueThreadBase()
        {
            m_InnerQueue = new ConcurrentQueue<Task>();
            this.m_QueueCount = m_InnerQueue.Count;
        }

        public void AddTask(Task task)
        {
            
                m_InnerQueue.Enqueue(task);

                this.m_QueueCount = m_InnerQueue.Count;
        }




        #endregion

        #region 主体
        Iwork manager_;
        /// 初始化线程
        private void InitThread(object manager)
        {

            m_ThreadList = new List<Thread>();
            manager_ = (Iwork)manager;
            for (int i = 0; i < ThreadCount; i++)
            {
                Thread t = new Thread(new ThreadStart(InnerDoWork));
                m_ThreadList.Add(t);
                t.IsBackground = true;
                t.Start();
            }
        }

        /// 开始
        public void Start(object manager)
        {
            InitThread( manager);
        }

        /// 线程工作
        private void InnerDoWork()
        {
            try
            {
                Exception doWorkEx = null;
                DoWorkResult doworkResult = DoWorkResult.ContinueThread;
               
                while (!this.Cancel )
                {
                    var t = CurrentPendingQueue;
                    if (t.PendingValue is null)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    OnOneJobStart(t.PendingValue, new CompetedEventArgs() { CompetedPrecent = 0, InnerException = doWorkEx, id = Thread.CurrentThread.GetHashCode() });
                    
                    try
                    {
                        doworkResult = DoWork(t.PendingValue);
                    }
                    catch (Exception ex)
                    {
                        doWorkEx = ex;
                        Debug.Log(ex.ToString());
                    }

                    m_CompletedCount++;
                    int precent = m_CompletedCount * 100 / m_QueueCount;
                    OnOneCompleted(t.PendingValue, new CompetedEventArgs() { CompetedPrecent = precent, InnerException = doWorkEx, id = Thread.CurrentThread.GetHashCode() });

                    if (doworkResult == DoWorkResult.AbortAllThread)
                    {
                        this.Cancel = true;
                        break;
                    }
                    else if (doworkResult == DoWorkResult.AbortCurrentThread)
                    {
                        break;
                    }

                   
                }

                lock (m_AllCompletedLock)
                {
                    m_CompetedCount++;

                    if (m_CompetedCount == m_ThreadList.Count)
                    {
                        OnAllCompleted(new CompetedEventArgs() { CompetedPrecent = 100 });
                    }
                }
                Thread.Sleep(300);
            }
            catch
            {
                throw;
            }
        }


        protected DoWorkResult DoWork(Task pendingValue)
        {
            try
            {
                manager_.DoWork(pendingValue);
                //do somthing
                //dump class info


                //..........多线程处理....
                return DoWorkResult.ContinueThread;//没有异常让线程继续跑..
            }
            catch (Exception ex)
            {
                return DoWorkResult.AbortCurrentThread;//有异常,可以终止当前线程.当然.也可以继续,
                                                       //return  DoWorkResult.AbortAllThread; //特殊情况下 ,有异常终止所有的线程...
            }
        }

        /// 获取当前结果
        private PendingResult CurrentPendingQueue
        {
            get
            {
            
                    PendingResult t = new PendingResult();

                    if (m_InnerQueue.Count != 0)
                    {
                        Task task = null;
                        if(!m_InnerQueue.TryDequeue(out task))
                        {
                            m_InnerQueue.TryDequeue(out task);
                        }
                        t.PendingValue = task;
                        t.IsHad = true;
                    }
                    else
                    {
                        t.PendingValue = null;
                        t.IsHad = false;
                    }

                    return t;
                
            }
        }

        #endregion


    }

    #region 相关类&枚举

    /// dowork结果枚举
    public enum DoWorkResult
    {
        /// 继续运行，默认
        ContinueThread = 0,
        /// 终止当前线程
        AbortCurrentThread = 1,
        /// 终止全部线程
        AbortAllThread = 2
    }

    /// 完成事件数据
    public class CompetedEventArgs : EventArgs
    {
        public CompetedEventArgs()
        {
        }
        /// 完成百分率
        public int CompetedPrecent { get; set; }
        /// 异常信息
        public Exception InnerException { get; set; }

        public int id { get; set; }
    }
}
#endregion