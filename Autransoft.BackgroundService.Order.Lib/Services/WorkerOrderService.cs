using System;
using System.Linq;
using Autransoft.BackgroundService.Order.Lib.Attributes;
using Autransoft.BackgroundService.Order.Lib.Entities;
using Autransoft.BackgroundService.Order.Lib.Logging;
using Autransoft.BackgroundService.Order.Lib.Repositories;

namespace Autransoft.BackgroundService.Order.Lib.Services
{
    internal class WorkerOrderService
    {
        private WorkerRepository _repository;
        private Logger _logger;

        public WorkerOrderService()
        {
            _repository = new WorkerRepository();
            _logger = new Logger();
        }

        public void EndExecution()
        {
            var worker = _repository.UpdateWorker(GetType(), true);
            _logger.LogInformation(worker);
        }

        public void RestartAllWorkers()
        {
            var workers = _repository.Get();
            if(workers.Any(worker => !worker.Executed))
                return;
            
            foreach(var worker in workers)
                _repository.UpdateWorker(worker.Type, false);

            _logger.Restart(workers);
        }

        public void Save(Type type)
        {
            var dependencies = Attribute.GetCustomAttributes(type)
                .Where(attribute => attribute is DependencyWorkerAttribute)
                .Select(attribute => (attribute as DependencyWorkerAttribute).Type);

            WorkerEntity worker = null;
            if(dependencies == null || dependencies.Count() == 0)
                worker = _repository.Add(type);
            else
                foreach(var dependency in dependencies)
                    _repository.Add(type, dependency);

            _logger.LogInformation(worker);
        }

        public int GetIndex(Type type) => _repository.GetIndex(type);
        
        public bool Executed(Type type) => _repository.Get(type).Executed;

        public bool AllDependencyExecuted(Type type) => _repository.AllDependencyExecuted(type);
    }
}