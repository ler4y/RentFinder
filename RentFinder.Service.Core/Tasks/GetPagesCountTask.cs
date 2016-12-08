using System;
using System.Linq;
using AngleSharp;

namespace RentFinder.Service.Core.Tasks
{
    public class GetPagesCountActivity : IActivity<int>
    {
        private readonly IBrowsingContext _browsingContext;
        private readonly string _link;
        public GetPagesCountActivity(string link, IBrowsingContext brContext, uint maxEfforts = 3)
        {
            _link = link;
            _browsingContext = brContext;
            Func = GetPagesCount;
        }
        public void Action()
        {
            Result = GetPagesCount();
            Completed?.Invoke(this, Result);
        }

        public int Result { get; private set; }

        public Func<int> Func { get; private set; }

        public event EventHandler<int> Completed;
        private int GetPagesCount()
        {
#if DEBUG 
            return 1;
#endif
            var doc = _browsingContext.OpenAsync(_link).Result;
            var pager = doc.QuerySelector("#body-container > div:nth-child(3) > div > div.pager.rel.clr");
            // ReSharper disable once UnusedVariable
            var spans = pager.QuerySelectorAll(".item.fleft");

            return spans.Select(s=> 
                {
                    int pageNumber;
                    if (int.TryParse(s.Children.First().Children.First().InnerHtml, out pageNumber)) return pageNumber;
                        return -1;
                }).Max();
        }

       
    }
}