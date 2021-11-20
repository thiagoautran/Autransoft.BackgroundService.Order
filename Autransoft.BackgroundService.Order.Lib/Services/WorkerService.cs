using System;
using System.Linq;
using Autransoft.BackgroundService.Order.Lib.Attributes;
using Autransoft.BackgroundService.Order.Lib.Loggings;
using Autransoft.BackgroundService.Order.Lib.Repositories;

namespace Autransoft.BackgroundService.Order.Lib.Services
{
    internal class WorkerService
    {
        private SharedObjectRepository _sharedObjectRepository;
        private WorkerRepository _repository;
        private Logging _logger;

        private static bool _isRestartAllWorkersRunnig;

        public WorkerService()
        {
            _sharedObjectRepository = new SharedObjectRepository();
            _repository = new WorkerRepository();
            _logger = new Logging();

            _isRestartAllWorkersRunnig = false;
        }

        public void EndExecution(Type type)
        {
            var worker = _repository.UpdateWorker(type, true);

            _logger.LogStatusWorkerOrder(worker);
        }

        public void RestartAllWorkers()
        {
            if(_isRestartAllWorkersRunnig)
                return;

            _isRestartAllWorkersRunnig = true;
            _sharedObjectRepository.Clean();

            try
            {
                var workers = _repository.Get();
                if(workers.Any(worker => !worker.Executed))
                    return;
                
                foreach(var worker in workers)
                    _repository.UpdateWorker(worker.Type, false);

                _logger.LogRestart(workers);
            }
            finally
            {
                _isRestartAllWorkersRunnig = false;
            }
        }

        public void Save(Type type)
        {
            var dependencies = Attribute.GetCustomAttributes(type)
                .Where(attribute => attribute is DependencyWorkerAttribute)
                .Select(attribute => (attribute as DependencyWorkerAttribute).Type);

            var worker = _repository.Add(type);
            if(dependencies != null)
                foreach(var dependency in dependencies)
                {
                    _repository.Add(dependency);
                    _repository.AddDependency(type, dependency);
                }

            _logger.LogStatusWorkerOrder(worker);
        }
        
        public bool? Executed(Type type)
        {
            if(_isRestartAllWorkersRunnig)
                return true;

            return _repository.Get(type)?.Executed;
        } 

        public bool AllDependencyExecuted(Type type) 
        { 
            if(_isRestartAllWorkersRunnig)
                return false;

            var key = _repository.GetWorker(type);
            if(key == null)
                return false;

            if(key.Dependencies.Count() == 0)
                return true;
                
            return !key.Dependencies.Any(dependency => !dependency.Executed);
        }
    }
}