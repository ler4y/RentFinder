using System.Collections.Generic;
using RentFinder.Core.Areas;

namespace RentFinder.Base.Areas
{
    public interface ICity
    {
        string Name { get; }
        string LinkPart { get; }
        List<IDistrict> Districts { get; }
    }
}