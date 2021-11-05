using System;

namespace Autransoft.BackgroundService.Order.Lib.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DependencyWorkerAttribute : Attribute
    {
        public Type Type { get; set; }

        public DependencyWorkerAttribute(Type type) => Type = type;
    }
}