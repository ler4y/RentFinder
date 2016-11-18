using System.Collections.Concurrent;
using System.Threading;
using RentFinder.Service.Core.TaskManagement.Commands;

namespace RentFinder.Service.Core.TaskManagement
{
    public class TaskManager
    {
        private static CancellationToken _cancellationToken;
        private Thread _executorThread;
        private ConcurrentBag<ITask> _tasks = new ConcurrentBag<ITask>();

        public void Start()
        {
           _executorThread = new Thread(Work);
        }

        public static void Work()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                
            }
        }
    }
}
