using System;
using System.Collections.Generic;
using System.Linq;
using Autransoft.BackgroundService.Order.Lib.Entities;

namespace Autransoft.BackgroundService.Order.Lib.Repositories
{
    internal class WorkerRepository
    {
        private static List<WorkerEntity> _database;

        public static List<WorkerEntity> Database
        {
            get
            {
                if(_database == null)
                    _database = new List<WorkerEntity>();

                return _database;
            }
        }

        public bool AllDependencyExecuted(Type worker)
        {
            var key = GetWorker(worker);
            if(key == null)
                return false;
                
            return !key.Dependencies.Any(dependency => !dependency.Executed);
        }

        public void Add(Type worker, Type dependency)
        {
            Add(worker);
            Add(dependency);
            AddDependency(worker, dependency);
        }

        public void UpdateWorker(Type worker, bool executed)
        {
            var key = GetWorker(worker);
            if(key == null)
                return;

            key.Executed = executed;
        }

        private WorkerEntity GetWorker(Type worker)
        {
            if(worker == null)
                return null;

            return Database.FirstOrDefault(key => key.Type == worker);
        } 

        private void Add(Type worker)
        {
            if(worker == null)
                return;

            if(!Database.Any(database => database.Type == worker))
                Database.Add(new WorkerEntity(worker));
        }

        private void AddDependency(Type worker, Type dependency)
        {
            if(worker == null || dependency == null || worker == dependency)
                return;

            var key = Database.FirstOrDefault(database => database.Type == worker);
            if(key == null)
                return;

            var referenceWorker = Database.FirstOrDefault(database => database.Type == dependency);
            if(referenceWorker == null)
                return;

            key.Dependencies.Add(referenceWorker);
        }
    }
}