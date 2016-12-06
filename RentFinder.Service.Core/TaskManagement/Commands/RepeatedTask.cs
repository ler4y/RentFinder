using System;

namespace RentFinder.Service.Core.TaskManagement.Commands
{
    public class RepeatedTask:BaseTask
    {
        private readonly Action _action;
        private readonly TimeSpan _timeSpan;
        private DateTime _lastExecutedTime;

        public RepeatedTask(ITaskManager taskManager, Action action, TimeSpan timeSpan, TimeSpan? delay = null):base(taskManager)
        {
            if (delay.HasValue)
                _lastExecutedTime = DateTime.Now.Add(delay.Value);
            _action = action;
            _timeSpan = timeSpan;
        }

        public override void Execute()
        {
            if (_lastExecutedTime == default(DateTime) || _lastExecutedTime.Add(_timeSpan) < DateTime.Now)
            {
                _action();
                _lastExecutedTime = DateTime.Now;
            }
        }
    }
}
