using System;
using System.Collections.Generic;

namespace RentFinder.Service.Core.TaskManagement.Commands
{
    public static class TaskExtensions
    {
        public static ITask ContinueWith(this ITask currentTask, ITask nextTask)
        {
            return currentTask.ContinueWith(nextTask);
        }

        public static ITask ContinueWith(this ITask currentTask, IEnumerable<ITask> nextTasks)
        {
            return currentTask.ContinueWith(nextTasks.AsCompositeTask());
        }

        public static ITask ContinueWith(this ITask currentTask, Func<ITask> getNextTask)
        {
            return currentTask.ContinueWith(getNextTask());
        }

        public static ITask ContinueWith(this ITask currentTask, Func<IEnumerable<ITask>> getNextTasks)
        {
            return currentTask.ContinueWith(getNextTasks().AsCompositeTask());
        }

        public static ITask AsCompositeTask(this IEnumerable<ITask> source)
        {
            return new ScheduledTask(()=>
            {
            }, DateTime.Now
        );
        }

        public static ITask<T> AsCompositeTask<T>(this IEnumerable<ITask<T>> source)
        {
            return new SimpleTask<T>(() => default(T));
        }
    }
}
