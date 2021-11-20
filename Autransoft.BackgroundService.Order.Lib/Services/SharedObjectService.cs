using System;
using System.Collections.Generic;
using Autransoft.BackgroundService.Order.Lib.Entities;
using Autransoft.BackgroundService.Order.Lib.Repositories;

namespace Autransoft.BackgroundService.Order.Lib.Services
{
    internal class SharedObjectService
    {
        private SharedObjectRepository _sharedObjectRepository;
        private WorkerRepository _repository;

        public SharedObjectService()
        {
            _sharedObjectRepository = new SharedObjectRepository();
            _repository = new WorkerRepository();
        }

        public IEnumerable<SharedObject> GetSharedObject(Type target)
        {
            if(target == null)
                return new List<SharedObject>();

            return _sharedObjectRepository.Get(target);
        }

        public void SharedObjectToAllWorkers(Type source, object sharedObject)
        { 
            if(source == null || sharedObject == null)
                return;

            foreach(var target in _repository.GetAllWorkers())
            {
                if(source == target)
                    continue;

                _sharedObjectRepository.Add(source, target, sharedObject);
            }
        }

        public void SharedObjectToSpecificWorker(Type source, Type target, object sharedObject)
        {
            if(source == null || target == null || source == target || sharedObject == null)
                return;

            _sharedObjectRepository.Add(source, target, sharedObject);
        }
    }
}