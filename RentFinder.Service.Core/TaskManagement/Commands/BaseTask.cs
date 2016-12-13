using System;
using RentFinder.Service.Core.Tasks;

namespace RentFinder.Service.Core.TaskManagement.Commands
{
    public abstract class BaseTask : ITask
    {
        private bool _isExecuted;
        protected IActivity Activity;
        public ITaskManager TaskManager { get; private set; }

        protected BaseTask(IActivity activity)
        {
            if (activity == null) throw new ArgumentNullException();
            Activity = activity;
        }

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

        public ITask ContinueWith(ITask nextTask)
        {
           Executed += (sender, args) => { TaskManager.AddTask(nextTask); };
            return nextTask;
        }

        public ITask<T> ContinueWith<T>(ITask<T> nextTask)
        {
            Executed += (sender, args) => { TaskManager.AddTask(nextTask); };
            return nextTask;
        }

        /*  public void ContinueWith(IEnumerable<ITask> nextTasks)
          {
              Executed += (sender, args) => { TaskManager.AddTasks(nextTasks); };
          }

          public void ContinueWith(Func<ITask> getNextTask)
          {
             Executed += (sender, args) => { TaskManager.AddTask(getNextTask()); };
          }

          public void ContinueWith(Func<IEnumerable<ITask>> getNextTasks)
          {
              Executed += (sender, args) => {TaskManager.AddTasks(getNextTasks()); };
          }*/
        /* public static void ContinueWith(this IEnumerable<ITask> currentTasks, ITask nextTask)
         {
             var enumerable = currentTasks as IList<ITask> ?? currentTasks.ToList();
             enumerable.ForEach(s => s.Executed += (sender, args) =>
             {
                 if (enumerable.All(t => t.IsExecuted)) enumerable.First().TaskManager.AddTask(nextTask);
             });
         }

         public static void ContinueWith(this IEnumerable<ITask> currentTasks, IEnumerable<ITask> nextTasks)
         {
             var enumerable = currentTasks as IList<ITask> ?? currentTasks.ToList();
             enumerable.ForEach(s => s.Executed += (sender, args) =>
             {
                 if (enumerable.All(t => t.IsExecuted)) enumerable.First().TaskManager.AddTasks(nextTasks);
             });
         }

         public static void ContinueWith(this IEnumerable<ITask> currentTasks, Func<ITask> getNextTask)
         {
             var enumerable = currentTasks as IList<ITask> ?? currentTasks.ToList();
             enumerable.ForEach(s => s.Executed += (sender, args) =>
             {
                 if (enumerable.All(t => t.IsExecuted)) enumerable.First().TaskManager.AddTask(getNextTask());
             });
         }

         public static void ContinueWith(this IEnumerable<ITask> currentTasks, Func<IEnumerable<ITask>> getNextTasks)
         {
             var enumerable = currentTasks as IList<ITask> ?? currentTasks.ToList();
             enumerable.ForEach(s => s.Executed += (sender, args) =>
             {
                 if (enumerable.All(t => t.IsExecuted)) enumerable.First().TaskManager.AddTasks(getNextTasks());
             });
         }*/
    }
}
