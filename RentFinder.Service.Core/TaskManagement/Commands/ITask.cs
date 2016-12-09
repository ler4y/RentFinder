using System;

namespace RentFinder.Service.Core.TaskManagement.Commands
{
    public interface ITask
    {
        ITaskManager TaskManager { get; }
        void Execute( ITaskManager taskManager);
        bool IsExecuted { get; }
        event EventHandler Executed;
    }
    public interface ITask<out T>:ITask
    {
        T Result { get; }
    }
}