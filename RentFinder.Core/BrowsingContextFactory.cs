using System;
using AngleSharp;
using AngleSharp.Network.Default;

namespace RentFinder.Core
{
    public class BrowsingContextFactory
    {
        public IBrowsingContext GetNew()
        {
            var requester = new HttpRequester();
            requester.Headers["User-Agent"] = Guid.NewGuid().ToString().Replace("-", "");
            var configuration = Configuration.Default.WithDefaultLoader(requesters: new[] { requester });
            var brContext = BrowsingContext.New(configuration);
            return brContext;
        }
    }
}
