using System.Collections.Generic;
using RentFinder.Core.Areas.DniproArea;

namespace RentFinder.Core.Areas
{
    public interface IArea
    {
        string Name { get; }
        string LinkPart { get; }
        List<ICity> Cities { get; }
    }
}