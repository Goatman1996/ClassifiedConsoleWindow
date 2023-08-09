using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Threading.Tasks;

namespace ClassifiedConsole.Runtime
{
    public class CDebugThreading
    {
        public int threadMaxHandleTaskCount = 1000;
        public Queue<IThreadTask> taskQueue;

        private Thread thread;
        public CDebugThreading()
        {
            return;
            this.taskQueue = new Queue<IThreadTask>();
            this.threadTaskList = new List<IThreadTask>();
            this.thread = new Thread(RuntimeThreading);
            this.thread.Name = "CDebug Thread";
            this.thread.IsBackground = true;
            this.thread.Start();

            // this.ThreadLoop();

            if (UnityEngine.Application.isEditor == false)
            {
                var go = new GameObject();
                go.name = nameof(ForThreadOnGameExit);
                var exitEvt = go.AddComponent<ForThreadOnGameExit>();
                exitEvt.OnDestroyEvt += this.OnDestroy;
                GameObject.DontDestroyOnLoad(go);
            }

            {
                var go = new GameObject();
                go.name = nameof(ForThreadUpdate);
                var exitEvt = go.AddComponent<ForThreadUpdate>();
                exitEvt.OnUpdate += this.ThreadLoop;
                GameObject.DontDestroyOnLoad(go);
            }
        }

        private void OnDestroy()
        {
            if (this.thread != null)
            {
                this.thread.Abort();
            }
        }

        private void ThreadLoop()
        {
            // while (true)
            // {
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
            // await Task.Delay(1);
            // }
        }

        private bool isRunningThread = false;

        private List<IThreadTask> threadTaskList;
        private IThreadTask RunningTask;
        private void RuntimeThreading()
        {
            while (true)
            {
                if (this.isRunningThread)
                {
                    foreach (var task in this.threadTaskList)
                    {
                        // task.result = task.Task?.Invoke();
                        task.Run();
                    }
                    this.isRunningThread = false;
                }

                Thread.Sleep(1);
            }
        }

        public void AddTaskToQueue(IThreadTask task)
        {
            // this.taskQueue.Enqueue(task);
            task.Run();
            task.CallBack();
        }

        private void OnTaskComplete(IThreadTask task)
        {
            task.CallBack();
        }
    }
}
