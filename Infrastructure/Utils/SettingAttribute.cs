using System;

namespace Infrastructure.Utils
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SettingAttribute : Attribute
    {
        public SettingAttribute()
        {

        }
    }
}