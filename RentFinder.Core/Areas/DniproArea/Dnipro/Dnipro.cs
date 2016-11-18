using System.Collections.Generic;

namespace RentFinder.Core.Areas.DniproArea.Dnipro
{
    public class Dnipro:ICity
    {
        public Dnipro()
        {
            InitDistricts();
        }
        public string Name { get; } = "Dnipro";
        public string LinkPart { get; } = "dnepropetrovsk/";
        public List<IDistrict> Districts { get; } = new List<IDistrict>();

        private void InitDistricts()
        {
            //TODO: Add districts for Dnipro
            //?search%5Bdistrict_id%5D=111 - where 111 district id
        }
    }
}
