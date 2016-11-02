using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
