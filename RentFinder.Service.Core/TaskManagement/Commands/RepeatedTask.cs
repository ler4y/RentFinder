using System;

namespace RentFinder.Service.Core.TaskManagement.Commands
{
    public class RepeatedTask:ITask
    {
        private readonly Action _action;
        private readonly TimeSpan _timeSpan;
        private DateTime _lastExecutedTime;

        public RepeatedTask(Action action, TimeSpan timeSpan, TimeSpan? delay = null)
        {
            if (delay.HasValue)
                _lastExecutedTime = DateTime.Now.Add(delay.Value);
            _action = action;
            _timeSpan = timeSpan;
        }

        public void Execute()
        {
            if (_lastExecutedTime == default(DateTime) || _lastExecutedTime.Add(_timeSpan) < DateTime.Now)
            {
                _action();
                _lastExecutedTime = DateTime.Now;
            }  
        }

        public bool IsExecuted { get; } = false;
    }
}
