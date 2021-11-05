using System;
using System.Threading;
using System.Threading.Tasks;
using Autransoft.BackgroundService.Order.Lib.Repositories;

namespace Autransoft.BackgroundService.Order.Lib.Services
{
    public abstract class BackgroundServiceOrder : Microsoft.Extensions.Hosting.BackgroundService
    {
        internal WorkerRepository _workerRepository;

        internal WorkerRepository Repository
        {
            get
            {
                if(_workerRepository == null)
                    _workerRepository = new WorkerRepository();

                return _workerRepository;
            }
        }

        public async override Task StartAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if(await BackgroundExecuteAsync(stoppingToken))
                    await EndAsync(stoppingToken);
            }

            throw new System.NotImplementedException();
        }

        public abstract Task<bool> BackgroundExecuteAsync(CancellationToken stoppingToken);

        public async Task EndAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(new TimeSpan(0, 0, 15), stoppingToken);
        }
    }
}