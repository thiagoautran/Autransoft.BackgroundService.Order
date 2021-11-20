using System;

namespace Autransoft.BackgroundService.Order.Lib.Entities
{
    public class SharedObject
    {
        public object Data { get; set; }

        public Type WorkerType { get; set; }

        public SharedObject(Type workerType, object data) => 
            (WorkerType, Data) = (workerType, data);
    }
}