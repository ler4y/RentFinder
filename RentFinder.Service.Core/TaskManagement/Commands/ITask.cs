using System;

namespace RentFinder.Service.Core.TaskManagement.Commands
{
    public interface ITask
    {
        ITaskManager TaskManager { get; }
        void Execute();
        bool IsExecuted { get; }
        event EventHandler Executed;
    }
    public interface ITask<out T>:ITask
    {
        T ExecuteResult();
        T Result { get; }
    }
}