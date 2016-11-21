using System;

namespace RentFinder.Service.Core.TaskManagement.Commands
{
    public class SimpleTask:ITask
    {
        private readonly Action _action;
        public SimpleTask(Action action)
        {
            _action = action;
        }

        public void Execute()
        {
            _action();
            IsExecuted = true;
        }

        public bool IsExecuted { get; private set; }
    }
}
