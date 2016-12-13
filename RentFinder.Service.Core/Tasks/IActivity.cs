using System;

namespace RentFinder.Service.Core.Tasks
{

    public interface IActivity
    {
        void Run();
    }
    public interface IActivity<T>: IActivity
    {
        T Result { get; }
    }
}