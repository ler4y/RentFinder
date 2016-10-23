using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentFinder.Model
{
    public class PreviewAdModel
    {
        public uint AdId { get; set; }
        public string TempId { get; set; }
        public double Price { get; set; }
        public List<string> PhoneNumbers { get; private set; } = new List<string>();
    }
}
