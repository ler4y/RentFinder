using System;

namespace RentFinder.Service.Core.TaskManagement.Commands
{
    public class ScheduledTask:ITask
    {
        private readonly Action _action;
        private readonly DateTime _invokationDateTime;

        public ScheduledTask(Action action, DateTime invocationDateTime)
        {
            _invokationDateTime = invocationDateTime;
            _action = action;
        }

        public void Execute()
        {
            if (DateTime.Now > _invokationDateTime)
            {
                _action();
                IsExecuted = true;
            }
        }

        public bool IsExecuted { get; private set; }
    }
}
