using System;
using RentFinder.Service.Core.Tasks;

namespace RentFinder.Service.Core.TaskManagement.Commands
{
    public class SimpleTask<T> : BaseTask, ITask<T>
    {
        private readonly Func<T> _func;
        public SimpleTask(Func<T> action) 
        {
            _func = action;
        }

        public override void Execute()
        {
            Result = _func();
            IsExecuted = true;
        }

        public T Result { get; private set; }
    }
}
