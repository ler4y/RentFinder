using System;
using RentFinder.Service.Core.Tasks;

namespace RentFinder.Service.Core.TaskManagement.Commands
{
    public class ScheduledTask:BaseTask
    {
        private readonly DateTime _invokationDateTime;

        public ScheduledTask(Action action, DateTime invocationDateTime):base(new BaseActivity(action))
        {
            _invokationDateTime = invocationDateTime;
        }

        public override void Execute()
        {
            if (DateTime.Now > _invokationDateTime)
            {
                Activity.Run();
                IsExecuted = true;
            }
        }
    }
}
