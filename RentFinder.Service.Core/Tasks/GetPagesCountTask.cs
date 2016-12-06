using AngleSharp;
using RentFinder.Service.Core.TaskManagement;
using RentFinder.Service.Core.TaskManagement.Commands;

namespace RentFinder.Service.Core.Tasks
{
    public class GetPagesCountTask : BaseTask, ITask<int>
    {
        private readonly IBrowsingContext _browsingContext;
        private readonly string _link;
        public GetPagesCountTask(ITaskManager taskManager, string link, IBrowsingContext brContext, uint maxEfforts = 3) : base(taskManager)
        {
            _link = link;
            _browsingContext = brContext;
        }

        public int ExecuteResult()
        {
            Execute();
            return Result;
        }

        public int Result { get; private set; }

        public override void Execute()
        {
            Result = GetPagesCount();
            IsExecuted = true;
        }

        private int GetPagesCount()
        {
            var doc = _browsingContext.OpenAsync(_link).Result;
            var pager = doc.QuerySelector("#body-container > div:nth-child(3) > div > div.pager.rel.clr");
            // ReSharper disable once UnusedVariable
            var spans = pager.QuerySelectorAll(".item.fleft");
#if RELEASE
            return spans.Select(s=> 
                {
                    int pageNumber;
                    if (int.TryParse(s.Children.First().Children.First().InnerHtml, out pageNumber)) return pageNumber;
                    else return -1;
                    
                }).Max();
#else
            return 1;
#endif
        }

    }
}