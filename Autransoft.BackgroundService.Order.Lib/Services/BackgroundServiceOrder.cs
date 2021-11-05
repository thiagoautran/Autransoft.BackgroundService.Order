using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autransoft.BackgroundService.Order.Lib.Attributes;
using Autransoft.BackgroundService.Order.Lib.Entities;
using Autransoft.BackgroundService.Order.Lib.Logging;
using Autransoft.BackgroundService.Order.Lib.Repositories;

namespace Autransoft.BackgroundService.Order.Lib.Services
{
    public abstract class BackgroundServiceOrder : Microsoft.Extensions.Hosting.BackgroundService
    {
        private TimeSettingAttribute _timeSetting;
        private WorkerRepository _repository;
        private Logger _logger;

        public async override Task StartAsync(CancellationToken cancellationToken)
        {
            _repository = new WorkerRepository();
            _logger = new Logger();

            _timeSetting = Attribute.GetCustomAttributes(GetType())
                .Where(attribute => attribute is TimeSettingAttribute)
                .Select(attribute => (attribute as TimeSettingAttribute))
                .FirstOrDefault();

            var dependencies = Attribute.GetCustomAttributes(GetType())
                .Where(attribute => attribute is DependencyWorkerAttribute)
                .Select(attribute => (attribute as DependencyWorkerAttribute).Type);

            WorkerEntity worker = null;
            if(dependencies == null || dependencies.Count() == 0)
                worker = _repository.Add(GetType());
            else
                foreach(var dependency in dependencies)
                    _repository.Add(GetType(), dependency);

            _logger.LogInformation(worker);
            await base.StartAsync(cancellationToken);
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (_timeSetting != null && !stoppingToken.IsCancellationRequested)
            {
                if(!_repository.Get(GetType()).Executed)
                    continue;

                if(!_repository.AllDependencyExecuted(GetType()))
                    continue;

                if(!(await BackgroundExecuteAsync(stoppingToken)))
                {
                    await Task.Delay(_timeSetting.TimeBetweenExecution, stoppingToken);
                    continue;
                }

                EndExecutionAsync();
                await RestartAllWorkersAsync(stoppingToken);
                await Task.Delay(_timeSetting.TimeBetweenExecution, stoppingToken);
            }
        }

        protected abstract Task<bool> BackgroundExecuteAsync(CancellationToken stoppingToken);

        private void EndExecutionAsync()
        {
            var worker = _repository.UpdateWorker(GetType(), true);
            _logger.LogInformation(worker);
        }

        private async Task RestartAllWorkersAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(new TimeSpan(0, 0, _repository.GetKey(GetType())), stoppingToken);

            var workers = _repository.Get();
            if(workers.Any(worker => !worker.Executed))
                return;
            
            foreach(var worker in workers)
                _repository.UpdateWorker(worker.Type, false);

            _logger.Restart(workers);
        }
    }
}