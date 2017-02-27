namespace RentFinder.Core.RealtyTypes
{
    public class Apartament : BaseRealtyType
    {
        public Apartament() : base("Apartament", "arenda-kvartir/")
        {
        }

        public Apartament WithHourlyRentalType()
        {
            LinkPart = BaseUrl + "kvartiry-s-pochasovoy-oplatoy/";
            return this;
        }

        public Apartament WithDaylyRentalType()
        {
            LinkPart = BaseUrl + "kvartiry-posutochno/";
            return this;
        }

        public Apartament WithLongTermRentalType()
        {
            LinkPart = BaseUrl + "dolgosrochnaya-arenda-kvartir/";
            return this;
        }

        public Apartament WithoutIntermediaries()
        {
            LinkPart = BaseUrl + "bez-posrednikov/";
            return this;
        }
    }
}
