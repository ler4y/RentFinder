using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using RentFinder.Service.Core.TaskManagement.Commands;

namespace RentFinder.Service.Core.TaskManagement
{
    public class TaskManager
    {
        private CancellationTokenSource _cancellationToken;
        private Thread _executorThread;
        private readonly ConcurrentBag<ITask> _tasks = new ConcurrentBag<ITask>();
        private TimeSpan _delayTimeSpan = new TimeSpan(0,0,1);

        public TimeSpan DelayBettweenExecutions
        {
            get { return _delayTimeSpan; }
            set
            {
                Stop();
                _executorThread.Join();
                _delayTimeSpan = value;
                Start();
            }
        }

        public void Start()
        {
            _cancellationToken = new CancellationTokenSource();
            _executorThread = new Thread(Work);
            _executorThread.Start();
        }

        private void Work()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                ITask task;
                if (_tasks.TryTake(out task))
                {
                    task.Execute();
                    if (!task.IsExecuted)
                        _tasks.Add(task);
                }
                Thread.Sleep(DelayBettweenExecutions);
            }
        }

        public void Stop()
        {
            _cancellationToken.Cancel();
        }

        public void AddTask(ITask task)
        {
            _tasks.Add(task);
        }

        public void AddTasks(IEnumerable<ITask> tasks)
        {
            foreach (var task in tasks)
            {
                _tasks.Add(task);
            }
        }
    }
}
