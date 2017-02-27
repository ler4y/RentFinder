using System.Collections.Generic;
using RentFinder.Core.Areas;

namespace RentFinder.Base.Areas
{
    public interface IArea
    {
        string Name { get; }
        string LinkPart { get; }
        List<ICity> Cities { get; }
    }
}