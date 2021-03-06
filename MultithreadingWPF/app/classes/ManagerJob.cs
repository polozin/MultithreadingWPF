using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1.app.classes
{
    delegate void AddMessageDelegate();

    class ManagerJob
    {
        const int TIME_SPAN_MIN = 10;
        const int TIME_SPAN_MAX = 1000;
        const int TIME_TO_SUPPLIER_DATA = 1000;

        public static AddMessageDelegate addMessage;
        private static EventWaitHandle waitHandleStartJob = new AutoResetEvent(true);
        private static EventWaitHandle waitHandleUpdateMessage = new AutoResetEvent(true);
        private Task manager;
        private static Hashtable listJobs = new Hashtable();
        public static StringBuilder payloadString = new StringBuilder();
        private static Queue<string> buffer = new Queue<string>();
        private static bool trueToDO = true;
        private static int CountJobs { set; get; }
        public ManagerJob(int countJobs)
        {
            CountJobs = countJobs;
            manager = new Task(ManagerTask);
        }
        /// <summary>
        /// ManagerTask method to main task named ManagerTask
        /// 1. Create work tasks. 
        /// 2. Read from queue buffer and run external code (addMessage()) to update data.
        /// 3. Waiting for the end of work all tasks.
        /// </summary>
        private static void ManagerTask()
        {
            buffer.Enqueue("Manager task is started.");
            Random random = new Random();
            for (int i = 0; i < CountJobs; i++)
            {
                int timeSpan = random.Next(TIME_SPAN_MIN, TIME_SPAN_MAX);
                TaskJob job = new TaskJob(ToDoJob, timeSpan);
                listJobs.Add(job.Start(), job);
            }

            while (CountJobs > 0)
            {
                Thread.Sleep(TIME_TO_SUPPLIER_DATA);
                getData();

            }
            foreach (DictionaryEntry pair in listJobs)
            {
                TaskJob job = (TaskJob)pair.Value;
                buffer.Enqueue(string.Format("{0} {1}", job.IsCompletedSuccessfully, job.TotalResult));
            }
            buffer.Enqueue("Manager task finished work");
            getData();
            
        }
        private static void getData()
        {
            while (buffer.Count > 0)
            {
                payloadString.AppendFormat("{0}{1}", buffer.Dequeue(), Environment.NewLine);
            }
            addMessage();
            payloadString.Clear();
        }
        /// <summary>
        /// ToDoJob method to  tasks TaskJob
        /// 1. To start job waiting to find job in class 'waitHandleStartJob.WaitOne();' first task(thread) into 
        /// 2. job(sleep)
        /// 3. white to update data 'waitHandleUpdateMessage.WaitOne();'
        /// </summary>
        private static void ToDoJob()
        {
            do
            {
                // First task enter to code. The others are waiting.
                waitHandleStartJob.WaitOne();
                TaskJob jobStarted = null;
                if (listJobs.ContainsKey(Task.CurrentId))
                {
                    jobStarted = (TaskJob)listJobs[Task.CurrentId];
                }
                else
                {
                    break;
                }
                jobStarted.Counter++;
                // First task switch waitHandleStartJob to enter next thread.
                waitHandleStartJob.Set();

                // Job (all tasks sleep time is defined in the constructor) 
                Thread.Sleep(jobStarted.TimeSpan);

                // Task which woke up first enter to code 
                waitHandleUpdateMessage.WaitOne();
                TaskJob jobFinish = null;
                if (listJobs.ContainsKey(Task.CurrentId))
                {
                    jobFinish = (TaskJob)listJobs[Task.CurrentId];
                }
                else
                {
                    break;
                }

                DateTime date = DateTime.Now;
                buffer.Enqueue( string.Format("job number {0,3} completed job in {1:HH:mm:ss.fff} after {2,4} miliseconds (total {3,8}).", Task.CurrentId, date, jobFinish.TimeSpan, jobFinish.TotalTime));

                //Task switch waitHandleUpdateMessage to next task.
                waitHandleUpdateMessage.Set();
            } while (trueToDO);
            CountJobs--;
        }
        public void Start()
        {
            this.manager.Start();
        }
        public void Stop()
        {
            // boolen var trueToDO = false  => means stop infinite loop for TaskJob tasks
            trueToDO = false;
        }

    }
}
