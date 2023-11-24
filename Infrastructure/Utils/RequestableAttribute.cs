using System;

namespace Infrastructure.Utils
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequestableAttribute : Attribute
    {
        public RequestableAttribute()
        {

        }
    }
}