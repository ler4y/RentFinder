using System.Collections.Generic;
using RentFinder.Service.Core.TaskManagement.Commands;

namespace RentFinder.Service.Core.TaskManagement
{
    public interface ITaskManager
    {
        void Start();
        void Stop();
        void AddTask(ITask task);
        void AddTasks(IEnumerable<ITask> tasks);
    }
}