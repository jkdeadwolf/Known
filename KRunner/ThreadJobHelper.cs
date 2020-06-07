﻿using Known;
using System.Collections.Generic;

namespace KRunner
{
    internal class ThreadJobHelper
    {
        private static readonly Dictionary<string, ThreadJob> jobs = new Dictionary<string, ThreadJob>();

        public static void StartJob(IJob job)
        {
            if (job == null || job.Config == null || string.IsNullOrWhiteSpace(job.Config.Name))
                return;

            if (!jobs.ContainsKey(job.Config.Name))
            {
                var tjob = new ThreadJob(job);
                jobs.Add(job.Config.Name, tjob);
            }
        }

        public static void AbortJob(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;

            if (!jobs.ContainsKey(name))
                return;

            jobs[name].IsAbort = true;
        }

        public static void StopJob(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;

            if (!jobs.ContainsKey(name))
                return;

            var job = jobs[name];
            if (job.IsRunOver)
                job.Abort();
            else
                job.Join();

            jobs.Remove(name);
        }
    }
}
