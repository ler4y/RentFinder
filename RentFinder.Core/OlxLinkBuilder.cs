using RentFinder.Core.Areas.DniproArea;
using RentFinder.Core.RealtyTypes;

namespace RentFinder.Core
{
    public class OlxLinkBuilder
    {
        public const string BaseUrl = "https://www.olx.ua/";
        public const string Realty = "nedvizhimost/";
        public const string LongTermRental = "dolgosrochnaya-arenda-domov/";
        public const string DailyHourlyRental = "doma-posutochno-pochasovo/";

        public string GetLink(ICity city = null, IRealtyType realtyType = null)
        {
            return BaseUrl + Realty + realtyType?.LinkPart + city?.LinkPart;
        }
    }
}
