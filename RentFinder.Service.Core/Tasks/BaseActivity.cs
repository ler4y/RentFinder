using System;

namespace RentFinder.Service.Core.Tasks
{
    public class BaseActivity : IActivity
    {
        private readonly Action _action;
        public BaseActivity(Action action)
        {
            _action = action;
        }
        public void Run()
        {
            _action();
        }
    }

    public class BaseActivity<T> : IActivity<T>
    {
        private readonly Func<T> _action;
        public BaseActivity(Func<T> action)
        {
            _action = action;
        }
        public void Run()
        {
           Result = _action();
        }
        public T Result { get; private set; }
    }
}
