﻿using System;
using System.Threading;

namespace Known.Runner
{
    internal class ThreadJob
    {
        private readonly Thread thread;

        public ThreadJob(IThreadJob job)
        {
            Name = job.Config.Name;
            Interval = job.Config.Interval;
            Job = job;
            thread = new Thread(Run) { Name = Name, IsBackground = true };
            thread.Start();
        }

        public string Name { get; }
        public int Interval { get; }
        public IThreadJob Job { get; }
        public bool IsRunOver { get; private set; }
        public bool IsAbort { get; set; } = false;

        public void Abort()
        {
            thread.Abort();
        }

        public void Join()
        {
            thread.Join();
        }

        private void Run()
        {
            try
            {
                while (!IsAbort)
                {
                    Logger.Info($"{Name} is running.");
                    IsRunOver = false;
                    Job.Run();
                    Logger.Info($"{Name} is runned.");
                    IsRunOver = true;
                    Thread.Sleep(Interval);
                }
            }
            catch (ThreadAbortException)
            {
                Logger.Info($"The thread {Name} is abort by manual.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
