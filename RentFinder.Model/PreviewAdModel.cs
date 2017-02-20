using System.Collections.Generic;

namespace RentFinder.Model
{
    public class PreviewAdModel: IAddModel
    {
        public uint AdId { get; set; }
        public string TempId { get; set; }
        public double Price { get; set; }
        public List<string> PhoneNumbers { get; private set; } = new List<string>();
    }
}
