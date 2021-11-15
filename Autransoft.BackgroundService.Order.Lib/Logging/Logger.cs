using System;
using System.Collections.Generic;
using System.Linq;
using Autransoft.BackgroundService.Order.Lib.Entities;

namespace Autransoft.BackgroundService.Order.Lib.Logging
{
    internal class Logger
    {
        private Guid? _logId;
        private Guid LogId
        {
            get
            {
                if(_logId == null)
                    _logId = Guid.NewGuid();

                return _logId.Value;
            }
        }

        internal void Restart(IEnumerable<WorkerEntity> workers)
        {
            _logId = Guid.NewGuid();
            
            Console.WriteLine($"Restart");

            foreach(var worker in workers)
                LogInformation(worker);
        }

        internal void LogInformation(WorkerEntity worker)
        {
            if(worker.Dependencies.Count() == 0)
                Console.WriteLine($"Id:{LogId}|Executed:{worker.Executed}|WorkerName:{worker.Type.Name}");
            else
                Console.WriteLine($"Id:{LogId}|Executed:{worker.Executed}|WorkerName:{worker.Type.Name}|Dependencies:{string.Join( ",", worker.Dependencies.Select(x => x.Type.Name))}");
        }
    }
}