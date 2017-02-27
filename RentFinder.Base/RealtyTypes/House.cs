namespace RentFinder.Core.RealtyTypes
{
    public class House: BaseRealtyType
    {
        public House() : base("House", "arenda-domov/")
        {
        }


        public House WithDaylyRentalType()
        {
            LinkPart = BaseUrl + "doma-posutochno-pochasovo/";
            return this;
        }

        public House WithLongTermRentalType()
        {
            LinkPart = BaseUrl + "dolgosrochnaya-arenda-kvartir/";
            return this;
        }
    }
}
