using System;
using System.Threading;
using System.Threading.Tasks;
using Autransoft.BackgroundService.Order.Lib.Attributes;
using Autransoft.BackgroundService.Order.Lib.Extensions;

namespace Autransoft.BackgroundService.Order.Lib.Services
{
    public abstract class BackgroundServiceOrder : Microsoft.Extensions.Hosting.BackgroundService
    {
        private TimeSettingAttribute _timeSetting;
        private WorkerOrderService _service;

        public BackgroundServiceOrder() => _service = new WorkerOrderService();

        public async override Task StartAsync(CancellationToken cancellationToken)
        {
            _timeSetting = GetType().GetTimeSetting();

            _service.Save(GetType());

            await BackgroundStartAsync(cancellationToken);

            await base.StartAsync(cancellationToken);
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (_timeSetting != null && !stoppingToken.IsCancellationRequested)
            {
                if(!_service.Executed(GetType()))
                    continue;

                if(!_service.AllDependencyExecuted(GetType()))
                    continue;

                if(!(await BackgroundExecuteAsync(stoppingToken)))
                {
                    await Task.Delay(_timeSetting.TimeBetweenExecution, stoppingToken);
                    continue;
                }

                _service.EndExecution();

                await Task.Delay(new TimeSpan(0, 0, _service.GetIndex(GetType())), stoppingToken);

                _service.RestartAllWorkers();

                await Task.Delay(_timeSetting.TimeBetweenExecution, stoppingToken);
            }
        }

        public async override Task StopAsync(CancellationToken cancellationToken)
        {
            await BackgroundStopAsync(cancellationToken);
            
            await base.StopAsync(cancellationToken);
        }

        public virtual async Task BackgroundStartAsync(CancellationToken cancellationToken) => await Task.CompletedTask;

        public virtual async Task BackgroundStopAsync(CancellationToken cancellationToken) => await Task.CompletedTask;

        protected abstract Task<bool> BackgroundExecuteAsync(CancellationToken stoppingToken);
    }
}