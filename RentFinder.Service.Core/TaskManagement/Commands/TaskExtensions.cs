namespace RentFinder.Service.Core.TaskManagement.Commands
{
    public static class TaskExtensions
    {
        public static void ContinueWith(this ITask currentTask, ITask nextTask)
        {
            currentTask.Executed += (sender, args) => { currentTask.TaskManager.AddTask(nextTask);};
        }

    }
}
