using System;
using MoreLinq;
using System.Collections.Generic;
using System.Linq;

namespace RentFinder.Service.Core.TaskManagement.Commands
{
    public static class TaskExtensions
    {
        public static void ContinueWith(this ITask currentTask, ITask nextTask)
        {
            currentTask.Executed += (sender, args) => { currentTask.TaskManager.AddTask(nextTask);};
        }

        public static void ContinueWith(this ITask currentTask, IEnumerable<ITask> nextTasks)
        {
            currentTask.Executed += (sender, args) => { currentTask.TaskManager.AddTasks(nextTasks); };
        }

        public static void ContinueWith(this ITask currentTask, Func<ITask> getNextTask)
        {
            currentTask.Executed += (sender, args) => { currentTask.TaskManager.AddTask(getNextTask()); };
        }

        public static void ContinueWith(this ITask currentTask, Func<IEnumerable<ITask>> getNextTasks)
        {
            currentTask.Executed += (sender, args) => { currentTask.TaskManager.AddTasks(getNextTasks()); };
        }
        public static void ContinueWith(this IEnumerable<ITask> currentTasks, ITask nextTask)
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
        }

    }
}
