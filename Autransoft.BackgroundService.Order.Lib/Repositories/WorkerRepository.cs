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

        public WorkerEntity Get(Type type) => Database.FirstOrDefault(database => database.Type == type);

        public IEnumerable<Type> GetAllWorkers() => Database.Select(database => database.Type);

        public IEnumerable<WorkerEntity> Get() => Database;

        public WorkerEntity GetWorker(Type type)
        {
            if(type == null || Database.Count() == 0)
                return null;

            return Database.FirstOrDefault(key => key.Type == type);
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

        public void AddDependency(Type worker, Type dependency)
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

        public WorkerEntity UpdateWorker(Type type, bool executed)
        {
            var worker = GetWorker(type);
            if(worker == null)
                return null;

            worker.Executed = executed;

            return worker;
        }
    }
}