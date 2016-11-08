using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentFinder.Core.Areas.DniproArea
{
    public class Dnipro:ICity
    {
        public string Name { get; } = "Dnipro";
        public string LinkPart { get; } = "dnepropetrovsk/";
        public List<IDistrict> Districts { get; } = new List<IDistrict>();
    }
}
