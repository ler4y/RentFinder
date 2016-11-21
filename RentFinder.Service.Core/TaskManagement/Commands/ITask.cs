using System;

namespace RentFinder.Service.Core.TaskManagement.Commands
{
    public interface ITask
    {
        void Execute();
        bool IsExecuted { get; }
    }
}