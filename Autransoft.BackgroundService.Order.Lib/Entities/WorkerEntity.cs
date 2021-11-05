using System;
using System.Collections.Generic;

namespace Autransoft.BackgroundService.Order.Lib.Entities
{
    internal class WorkerEntity
    {
        public Type Type { get; set; }
        public bool Executed { get; set; }
        public List<WorkerEntity> Dependencies { get; set; }

        public WorkerEntity(Type type)
        {
            Type = type;
            Executed = false;
            Dependencies = new List<WorkerEntity>();
        }
    }
}