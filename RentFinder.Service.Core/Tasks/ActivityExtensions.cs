
using RentFinder.Service.Core.TaskManagement;
using RentFinder.Service.Core.TaskManagement.Commands;

namespace RentFinder.Service.Core.Tasks
{
    public static class ActivityExtensions
    {
        public static ITask<T> AsSimpleTask<T>(this IActivity<T> activity, ITaskManager taskManager)
        {
            return new SimpleTask<T>(taskManager, activity.Func);
        }
    }
}
