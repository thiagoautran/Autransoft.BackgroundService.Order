using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autransoft.BackgroundService.Order.Lib.Entities;

namespace Autransoft.BackgroundService.Order.Lib.Loggings
{
    internal class Logging
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

        internal void LogRestart(IEnumerable<WorkerEntity> workers)
        {
            var log = new StringBuilder();
            _logId = Guid.NewGuid();
            
            log.AppendLine("Restart");

            foreach(var worker in workers)
            {
                if(worker.Dependencies.Count() == 0)
                    log.AppendLine($"Id:{LogId}|Executed:{worker.Executed}|WorkerName:{worker.Type.Name}");
                else
                    log.AppendLine($"Id:{LogId}|Executed:{worker.Executed}|WorkerName:{worker.Type.Name}|Dependencies:{string.Join( ",", worker.Dependencies.Select(x => x.Type.Name))}");
            }

            Console.WriteLine(log.ToString());
        }

        internal void LogStatusWorkerOrder(WorkerEntity worker)
        {
            var log = new StringBuilder();

            if(worker.Dependencies.Count() == 0)
                log.AppendLine($"Id:{LogId}|Executed:{worker.Executed}|WorkerName:{worker.Type.Name}");
            else
                log.AppendLine($"Id:{LogId}|Executed:{worker.Executed}|WorkerName:{worker.Type.Name}|Dependencies:{string.Join( ",", worker.Dependencies.Select(x => x.Type.Name))}");

            Console.WriteLine(log.ToString());
        }
    }
}