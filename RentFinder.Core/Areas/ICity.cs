using System.Collections.Generic;
using System.IO;

namespace RentFinder.Core.Areas.DniproArea
{
    public interface ICity
    {
        string Name { get;}
        string LinkPart { get; }
        List<IDistrict> Districts { get; }
}