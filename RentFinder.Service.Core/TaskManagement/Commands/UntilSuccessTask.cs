﻿using System;

namespace RentFinder.Service.Core.TaskManagement.Commands
{
    public class UntilSuccessTask : BaseTask
    {
        private readonly Func<bool> _func;
        private readonly uint _maxEfforts;

        private uint _currentEffort;

        public UntilSuccessTask(ITaskManager taskManager, Func<bool> func, uint maxEfforts = 3):base(taskManager)
        {
            _func = func;
            _maxEfforts = maxEfforts;
        }
        public override void Execute()
        {
            IsExecuted = _func() || _currentEffort >= _maxEfforts;
            _currentEffort++;
        }
    }
}