using System;
using RentFinder.Service.Core.Tasks;

namespace RentFinder.Service.Core.TaskManagement.Commands
{
    public class SimpleTask<T> : BaseTask, ITask<T>
    {
        public SimpleTask(IActivity<T> action) :base(action)
        {
        }

        public SimpleTask(Func<T> action) : base(new BaseActivity<T>(action))
        {
        }

        public override void Execute()
        {
            Activity.Run();
            var activity = Activity as IActivity<T>;
            if (activity != null) Result = activity.Result;
            IsExecuted = true;
        }

        public T Result { get; private set; }
    }
    public class SimpleTask : BaseTask
    {
        public SimpleTask(IActivity action) : base(action)
        {
        }

        public SimpleTask(Action action) : base(new BaseActivity(action))
        {
        }

        public override void Execute()
        {
            Activity.Run();
            IsExecuted = true;
        }
    }
}
