using System.Collections.Generic;

namespace RentFinder.Base.Areas
{
    public interface IArea
    {
        string Name { get; }
        string LinkPart { get; }
        List<ICity> Cities { get; }
    }
}