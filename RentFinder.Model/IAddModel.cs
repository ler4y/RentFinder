using System.Collections.Generic;

namespace RentFinder.Model
{
    public interface IAddModel
    {
        List<string> PhoneNumbers { get; }
        uint AdId { get; set; }
    }
}
