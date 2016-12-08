using System;

namespace RentFinder.Service.Core.Tasks
{
    public interface IActivity<T>
    {
        Func<T> Func { get; }
    }
}