namespace RentFinder.Core.RealtyTypes
{
    public class Room : BaseRealtyType
    {

        public Room() : base("Room", "arenda-komnat/")
        {
        }

        public Room WithDaylyRentalType()
        {
            LinkPart = BaseUrl + "komnaty-posutochno/";
            return this;
        }

        public Room WithLongTermRentalType()
        {
            LinkPart = BaseUrl + "dolgosrochnaya-arenda-komnat/";
            return this;
        }

        public Room OnlyBedPlace()
        {
            LinkPart = BaseUrl + "koyko-mesta/";
            return this;
        }
    }
}