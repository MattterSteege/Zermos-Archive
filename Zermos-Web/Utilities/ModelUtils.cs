using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Zermos_Web.Utilities
{
    public static class ModelUtils
    {
        public static T ReturnEmptyModel<T>() where T : new()
        {
            return (T)CreateEmptyObject(typeof(T));
        }

        private static object CreateEmptyObject(Type type)
        {
            if (type == typeof(string))
                return string.Empty;

            if (type == typeof(DateTime))
                return default(DateTime);

            if (type.IsValueType)
                return Activator.CreateInstance(type);

            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                var array = Array.CreateInstance(elementType, 0);
                return array;
            }

            if (typeof(IEnumerable).IsAssignableFrom(type) && type.IsGenericType)
            {
                var listType = typeof(List<>).MakeGenericType(type.GetGenericArguments()[0]);
                var list = (IList)Activator.CreateInstance(listType);
                list.Add(CreateEmptyObject(type.GetGenericArguments()[0]));
                return list;
            }

            var instance = Activator.CreateInstance(type);
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (prop.CanWrite)
                {
                    prop.SetValue(instance, CreateEmptyObject(prop.PropertyType));
                }
            }

            return instance;
        }
    }
}