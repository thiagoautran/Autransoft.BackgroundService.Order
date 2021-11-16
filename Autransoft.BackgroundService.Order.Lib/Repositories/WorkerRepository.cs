using System;
using System.Collections.Generic;
using System.Linq;
using Autransoft.BackgroundService.Order.Lib.Entities;

namespace Autransoft.BackgroundService.Order.Lib.Repositories
{
    internal class WorkerRepository
    {
        private static List<WorkerEntity> _database;

        private static List<WorkerEntity> Database
        {
            get
            {
                if(_database == null)
                    _database = new List<WorkerEntity>();

                return _database;
            }
        }

        public int GetIndex(Type type)
        {
            for(var i = 0; i < Database.Count(); i++)
                if(Database.ElementAt(i).Type == type)
                    return i;

            return 0;
        }

        public bool AllDependencyExecuted(Type worker)
        {
            var key = GetWorker(worker);
            if(key == null)
                return false;

            if(key.Dependencies.Count() == 0)
                return true;
                
            return !key.Dependencies.Any(dependency => !dependency.Executed);
        }

        public void Add(Type worker, Type dependency)
        {
            Add(worker);
            Add(dependency);
            AddDependency(worker, dependency);
        }

        public WorkerEntity Get(Type type) => Database.FirstOrDefault(database => database.Type == type);

        public IEnumerable<WorkerEntity> Get() => Database;

        public WorkerEntity UpdateWorker(Type type, bool executed)
        {
            var worker = GetWorker(type);
            if(worker == null)
                return null;

            worker.Executed = executed;

            return worker;
        }

        public WorkerEntity Add(Type type)
        {
            if(type == null)
                return null;

            if(Database.Any(database => database.Type == type))
                return null;

            var worker = new WorkerEntity(type);

            Database.Add(worker);

            return worker;
        }

        private WorkerEntity GetWorker(Type type)
        {
            if(type == null)
                return null;

            return Database.FirstOrDefault(key => key.Type == type);
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