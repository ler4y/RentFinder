using System;

namespace RentFinder.Service.Core.TaskManagement.Commands
{
    public class SimpleTask:BaseTask
    {
        private readonly Action _action;
        public SimpleTask(ITaskManager taskManager, Action action):base(taskManager)
        {
            _action = action;
        }

        public override void Execute()
        {
            _action();
            IsExecuted = true;
        }
    }
}
