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
            var date = DateTime.Now;
            
            log.AppendLine($"Id:{LogId}|{date.ToString("dd/MM/yyyy hh:mm:ss")}|Restart started");

            foreach(var worker in workers)
            {
                if(worker.Dependencies.Count() == 0)
                    log.AppendLine($"Id:{LogId}|{date.ToString("dd/MM/yyyy hh:mm:ss")}|Executed:{worker.Executed}|WorkerName:{worker.Type.Name}");
                else
                    log.AppendLine($"Id:{LogId}|{date.ToString("dd/MM/yyyy hh:mm:ss")}|Executed:{worker.Executed}|WorkerName:{worker.Type.Name}|Dependencies:{string.Join( ",", worker.Dependencies.Select(x => x.Type.Name))}");
            }

            log.Append($"Id:{LogId}|{date.ToString("dd/MM/yyyy hh:mm:ss")}|Restart finished");

            Console.WriteLine(log.ToString());
        }

        internal void LogStatusWorkerOrder(WorkerEntity worker)
        {
            var log = string.Empty;

            if(worker.Dependencies.Count() == 0)
                log = $"Id:{LogId}|{DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")}|Executed:{worker.Executed}|WorkerName:{worker.Type.Name}";
            else
                log = $"Id:{LogId}|{DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")}|Executed:{worker.Executed}|WorkerName:{worker.Type.Name}|Dependencies:{string.Join( ",", worker.Dependencies.Select(x => x.Type.Name))}";

            Console.WriteLine(log);
        }
    }
}