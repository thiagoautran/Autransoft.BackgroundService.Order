using System;
using System.Threading;
using System.Threading.Tasks;

namespace Autransoft.BackgroundService.Order.Lib.Services
{
    public abstract class BackgroundServiceOrder : Microsoft.Extensions.Hosting.BackgroundService
    {
        private WorkerOrderService _service;

        protected BackgroundServiceOrder() : base() 
        {
            _service = new WorkerOrderService();

            _service.Save(GetType());
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var executed = _service.Executed(GetType());
                if(executed != null && executed.Value)
                    continue;

                if(!_service.AllDependencyExecuted(GetType()))
                    continue;

                if(!(await BackgroundExecuteAsync(stoppingToken)))
                    continue;

                _service.EndExecution(GetType());

                await Task.Delay(new TimeSpan(0, 0, _service.GetIndex(GetType())), stoppingToken);

                _service.RestartAllWorkers();
            }
        }

        protected abstract Task<bool> BackgroundExecuteAsync(CancellationToken stoppingToken);
    }
}