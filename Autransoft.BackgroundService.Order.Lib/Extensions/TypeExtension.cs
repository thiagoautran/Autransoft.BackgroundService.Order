using System;
using System.Linq;
using Autransoft.BackgroundService.Order.Lib.Attributes;

namespace Autransoft.BackgroundService.Order.Lib.Extensions
{
    internal static class TypeExtension
    {
        public static TimeSettingAttribute GetTimeSetting(this Type type) =>
            Attribute.GetCustomAttributes(type)
                .Where(attribute => attribute is TimeSettingAttribute)
                .Select(attribute => (attribute as TimeSettingAttribute))
                .FirstOrDefault();
    }
}