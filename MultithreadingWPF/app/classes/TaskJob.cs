using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1.app.classes
{
    class TaskJob
    {
        private int timeSpan;
        private Task task;
        public TaskJob(Action action, int timeSpan)
        {
            this.task = new Task(action);
            this.TimeSpan = timeSpan;

        }
        public string IsCompletedSuccessfully { get {
                string res = this.task.IsCompletedSuccessfully ? "" : "NOT";
                return string.Format("task {0,3} completed {1} successfully", this.task.Id, res);
            }
        }
        public string TotalResult
        {
            get
            {
                return string.Format("{0,3} times in {1,4} milliseconds total {2,4} miliseconds", this.Counter, this.TimeSpan, TotalTime);
            }
        }
        public int Start()
        {
            this.task.Start();

            return this.task.Id;
        }
        public int TimeSpan { get => timeSpan; set => timeSpan = value; }
        public int Counter { get; set; } = 0;
        public int TotalTime { get => this.TimeSpan * this.Counter; }
    }
}
