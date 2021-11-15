using System;
using System.Threading;
using System.Threading.Tasks;

namespace Autransoft.BackgroundService.Order.Lib.Services
{
    public abstract class BackgroundServiceOrder : Microsoft.Extensions.Hosting.BackgroundService
    {
        private WorkerOrderService _service;

        public BackgroundServiceOrder() : base() => _service = new WorkerOrderService();

        public async override Task StartAsync(CancellationToken cancellationToken)
        {
            _service.Save(GetType());

            await BackgroundStartAsync(cancellationToken);
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if(_service.Executed(GetType()))
                    continue;

                if(!_service.AllDependencyExecuted(GetType()))
                    continue;

                if(!(await BackgroundExecuteAsync(stoppingToken)))
                    continue;

                _service.EndExecution();

                await Task.Delay(new TimeSpan(0, 0, _service.GetIndex(GetType())), stoppingToken);

                _service.RestartAllWorkers();
            }
        }

        public async override Task StopAsync(CancellationToken cancellationToken) => await BackgroundStopAsync(cancellationToken);

        public virtual Task BackgroundStartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public virtual Task BackgroundStopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        protected abstract Task<bool> BackgroundExecuteAsync(CancellationToken stoppingToken);
    }
}