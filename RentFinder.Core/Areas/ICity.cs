using System.Collections.Generic;

namespace RentFinder.Core.Areas
{
    public interface ICity
    {
        string Name { get; }
        string LinkPart { get; }
        List<IDistrict> Districts { get; }
    }
}