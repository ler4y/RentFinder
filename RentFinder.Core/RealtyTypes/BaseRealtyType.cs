

namespace RentFinder.Core.RealtyTypes
{
    public abstract class BaseRealtyType : IRealtyType
    {
        protected readonly string BaseUrl;

        protected BaseRealtyType(string name, string baseUrl)
        {
            BaseUrl = baseUrl;
            LinkPart = BaseUrl;
            Name = name;
        }

        public string Name { get; protected set; }
        public string LinkPart { get; protected set; }
    }
}