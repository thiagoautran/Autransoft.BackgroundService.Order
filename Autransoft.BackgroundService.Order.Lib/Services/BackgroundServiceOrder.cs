using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autransoft.BackgroundService.Order.Lib.Entities;

namespace Autransoft.BackgroundService.Order.Lib.Services
{
    public abstract class BackgroundServiceOrder : Microsoft.Extensions.Hosting.BackgroundService
    {
        private SharedObjectService _sharedObjectService;
        private WorkerService _workerService;

        protected BackgroundServiceOrder() : base() 
        {
            _sharedObjectService = new SharedObjectService();
            _workerService = new WorkerService();
            _workerService.Save(GetType());
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var executed = _workerService.Executed(GetType());
                if(executed != null && executed.Value)
                {
                    await Task.Delay(new TimeSpan(0, 0, 1), stoppingToken);
                    continue;
                }

                if(!_workerService.AllDependencyExecuted(GetType()))
                {
                    await Task.Delay(new TimeSpan(0, 0, 1), stoppingToken);
                    continue;
                }

                if(!(await ExecuteAsync(stoppingToken, _sharedObjectService.GetSharedObject(GetType()))))
                    continue;

                _workerService.EndExecution(GetType());

                _workerService.RestartAllWorkers();
            }
        }

        protected abstract Task<bool> ExecuteAsync(CancellationToken stoppingToken, IEnumerable<SharedObject> sharedObjects);

        protected void SharedObjectToAllWorkers(object sharedObject) => 
            _sharedObjectService.SharedObjectToAllWorkers(GetType(), sharedObject);

        protected void SharedObjectToSpecificWorker(Type workerType, object sharedObject) => 
            _sharedObjectService.SharedObjectToSpecificWorker(GetType(), workerType, sharedObject);
    }
}