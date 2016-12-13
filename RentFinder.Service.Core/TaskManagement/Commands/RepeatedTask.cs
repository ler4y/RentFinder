using System;
using RentFinder.Service.Core.Tasks;

namespace RentFinder.Service.Core.TaskManagement.Commands
{
    public class RepeatedTask:BaseTask
    {
        private readonly TimeSpan _timeSpan;
        private DateTime _lastExecutedTime;

        public RepeatedTask(Action action, TimeSpan timeSpan, TimeSpan? delay = null):base(new BaseActivity(action))
        {
            if (delay.HasValue)
                _lastExecutedTime = DateTime.Now.Add(delay.Value);
            _timeSpan = timeSpan;
        }

        public RepeatedTask(IActivity action, TimeSpan timeSpan, TimeSpan? delay = null) : base(action)
        {
            if (delay.HasValue)
                _lastExecutedTime = DateTime.Now.Add(delay.Value);
           
            _timeSpan = timeSpan;
        }

        public override void Execute()
        {
            if (_lastExecutedTime == default(DateTime) || _lastExecutedTime.Add(_timeSpan) < DateTime.Now)
            {
                Activity.Run();
                _lastExecutedTime = DateTime.Now;
            }
        }
    }
}
