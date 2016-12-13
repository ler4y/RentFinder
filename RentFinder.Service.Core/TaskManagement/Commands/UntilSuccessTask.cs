using RentFinder.Service.Core.Tasks;

namespace RentFinder.Service.Core.TaskManagement.Commands
{
    public class UntilSuccessTask : BaseTask
    {
        private readonly uint _maxEfforts;

        private uint _currentEffort;

        public UntilSuccessTask(IActivity action , uint maxEfforts = 3):base(action)
        {
            _maxEfforts = maxEfforts;
        }
        public override void Execute()
        {
            try
            {
                Activity.Run();
                IsExecuted = true;
            }
            catch
            {
                IsExecuted = _currentEffort >= _maxEfforts;
            }
            _currentEffort++;
        }
    }
}
