using System;

namespace RentFinder.Service.Core.TaskManagement.Commands
{
    public class ScheduledTask:BaseTask
    {
        private readonly Action _action;
        private readonly DateTime _invokationDateTime;

        public ScheduledTask(ITaskManager taskManager, Action action, DateTime invocationDateTime):base(taskManager)
        {
            _invokationDateTime = invocationDateTime;
            _action = action;
        }

        public override void Execute()
        {
            if (DateTime.Now > _invokationDateTime)
            {
                _action();
                IsExecuted = true;
            }
        }
    }
}
