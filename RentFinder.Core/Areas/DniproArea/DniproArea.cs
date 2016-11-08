using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentFinder.Core.Areas.DniproArea
{
    public class DniproArea : IArea
    {
        private const string AreaName = "Dnipro";
        private const string Link = "dnp/";
        private readonly List<ICity> _cities = new List<ICity>();

        public DniproArea()
        {
            InitCities();
        }
        public List<ICity> Cities => _cities;

        public string LinkPart => Link;

        public string Name => AreaName;

        private void InitCities()
        {
            _cities.Add(new Dnipro());
        }
    }
}
