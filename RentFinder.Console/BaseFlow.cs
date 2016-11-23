using System;
using System.Collections.Generic;
using RentFinder.Core;
using RentFinder.Service.Core.TaskManagement;
using RentFinder.Service.Core.TaskManagement.Commands;

namespace RentFinder.Console
{
    public class BaseFlow
    {
        private readonly BlackNumberManager _blackNumberManager = new BlackNumberManager();
        private readonly TaskManager _taskManager = new TaskManager();

        public BaseFlow()
        {
           Init();
        }

        private void Init()
        {
            //TODO: Init blackNumberManager from database
            
            List<ITask> tasks = new List<ITask>();

            //tasks.AddRange(); //add tasks to check actual ads
            //tasks.AddRange(); //add tasks to check black ads
            //tasks.AddRange(); //add tasks to get actual data
            //tasks.AddRange(); //add tasks to update black number manager with actual data
            //tasks.AddRange(); //add tasks to parse actual data
            //tasks.AddRange(); //add tasks to persist updated actual ads

            _taskManager.AddTasks(tasks);
            _taskManager.DelayBettweenExecutions = TimeSpan.FromSeconds(24 *60 *60 / tasks.Count);
            _taskManager.Start();
        }
    }
}
