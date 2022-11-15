using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Threading.Tasks;

namespace ClassifiedConsole
{
    public class CDebugThreading
    {
        public int threadMaxHandleTaskCount = 1000;
        public Queue<ThreadTask> taskQueue;

        private Thread thread;
        public CDebugThreading()
        {
            this.taskQueue = new Queue<ThreadTask>();
            this.threadTaskList = new List<ThreadTask>();
            this.thread = new Thread(RuntimeThreading);
            this.thread.IsBackground = true;
            this.thread.Start();

            this.ThreadLoop();
        }

        private async void ThreadLoop()
        {
            while (true)
            {
                if (this.isRunningThread == false)
                {
                    foreach (var task in this.threadTaskList)
                    {
                        this.OnTaskComplete(task);
                    }
                    this.threadTaskList.Clear();

                    var addedTaskCount = threadMaxHandleTaskCount;
                    while (this.taskQueue.Count > 0)
                    {
                        addedTaskCount--;
                        if (addedTaskCount < 0)
                        {
                            break;
                        }
                        this.threadTaskList.Add(this.taskQueue.Dequeue());
                    }
                    if (this.threadTaskList.Count > 0)
                    {
                        this.isRunningThread = true;
                    }
                }
                await Task.Delay(1);
            }
        }

        private bool isRunningThread = false;

        private List<ThreadTask> threadTaskList;
        private ThreadTask RunningTask;
        private void RuntimeThreading()
        {
            while (true)
            {
                if (this.isRunningThread)
                {
                    foreach (var task in this.threadTaskList)
                    {
                        task.result = task.Task?.Invoke();
                    }
                    this.isRunningThread = false;
                }

                Thread.Sleep(1);
            }
        }

        public void AddTaskToQueue(ThreadTask task)
        {
            this.taskQueue.Enqueue(task);
        }

        private void OnTaskComplete(ThreadTask task)
        {
            task.callBack?.Invoke(task);
        }
    }

    public class ThreadTask
    {
        public Func<object> Task;
        public object result;
        public Action<ThreadTask> callBack;
    }
}
