using System;

namespace Autransoft.BackgroundService.Order.Lib.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TimeSettingAttribute : Attribute
    {
        public TimeSpan TimeBetweenExecution { get; set; }
    }
}