using System;
using AngleSharp;
using AngleSharp.Network.Default;

namespace RentFinder.Base
{
    public class BrowsingContextFactory
    {
        public IBrowsingContext GetNew()
        {
            var requester = new HttpRequester();
            requester.Headers["User-Agent"] = "Mozilla / 5.0(Windows; U; Windows NT 6.1; en - US; rv: 1.9.0.9) Gecko / 2009042410 Firefox / 3.0.9 Wyzo / 3.0.3" + Guid.NewGuid().ToString().Replace("-", "");
            var configuration = Configuration.Default.WithDefaultLoader(requesters: new[] { requester });
            var brContext = BrowsingContext.New(configuration);
            return brContext;
        }
    }
}
