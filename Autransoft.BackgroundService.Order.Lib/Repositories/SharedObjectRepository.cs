using System;
using System.Collections.Generic;
using System.Linq;
using Autransoft.BackgroundService.Order.Lib.Entities;

namespace Autransoft.BackgroundService.Order.Lib.Repositories
{
    internal class SharedObjectRepository
    {
        private static Dictionary<Type, IList<SharedObject>> _database;

        private static Dictionary<Type, IList<SharedObject>> Database
        {
            get
            {
                if(_database == null)
                    _database = new Dictionary<Type, IList<SharedObject>>();

                return _database;
            }
        }

        public IEnumerable<SharedObject> Get(Type target) 
        { 
            if(target == null || Database.Keys.Count() == 0)
                return new List<SharedObject>();

            var key = Database.Keys.FirstOrDefault(key => key == target);
            if(key == null)
                return new List<SharedObject>();

            return Database[key];
        }

        public void Add(Type source, Type target, object sharedObject)
        {
            if(source == null || target == null || sharedObject == null)
                return;

            if(Database.Keys.Count() == 0 || !Database.Keys.Any(key => key == target))
            {
                Database.Add(target, new List<SharedObject> { new SharedObject(source, sharedObject) });
                return;
            }

            Database[target].Add(new SharedObject(source, sharedObject));
        }

        public void Clean() => _database = new Dictionary<Type, IList<SharedObject>>();
   }
}