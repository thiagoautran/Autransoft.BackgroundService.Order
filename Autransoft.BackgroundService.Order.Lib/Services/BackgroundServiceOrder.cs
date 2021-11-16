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
                Console.WriteLine("Executed");
                await Task.Delay(new TimeSpan(0, 0, 2), stoppingToken);

                if(_service.Executed(GetType()))
                    continue;

                Console.WriteLine("AllDependencyExecuted");
                await Task.Delay(new TimeSpan(0, 0, 2), stoppingToken);

                if(!_service.AllDependencyExecuted(GetType()))
                    continue;

                Console.WriteLine("BackgroundExecuteAsync");
                await Task.Delay(new TimeSpan(0, 0, 2), stoppingToken);

                if(!(await BackgroundExecuteAsync(stoppingToken)))
                    continue;

                Console.WriteLine("EndExecution");
                await Task.Delay(new TimeSpan(0, 0, 2), stoppingToken);

                _service.EndExecution();

                await Task.Delay(new TimeSpan(0, 0, _service.GetIndex(GetType())), stoppingToken);

                _service.RestartAllWorkers();
            }
        }

        protected abstract Task<bool> BackgroundExecuteAsync(CancellationToken stoppingToken);
    }
}