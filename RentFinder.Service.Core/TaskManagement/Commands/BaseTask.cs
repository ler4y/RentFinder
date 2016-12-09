using System;

namespace RentFinder.Service.Core.TaskManagement.Commands
{
    public abstract class BaseTask : ITask
    {
        private bool _isExecuted;
        public ITaskManager TaskManager { get; private set; }

        public void Execute(ITaskManager taskManager)
        {
            if (taskManager == null) throw new ArgumentNullException(nameof(taskManager));
            TaskManager = taskManager;
            Execute();
        }

        public abstract void Execute();

        public bool IsExecuted
        {
            get { return _isExecuted; }
            set
            {
                _isExecuted = value;
                Executed?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler Executed;
    }
}
