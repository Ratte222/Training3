using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace AuxiliaryLib.Extensions
{
    public static class TypeUtilities
    {
        public static List<T> GetAllPublicConstantValues<T>(this Type type)
        {
            return type
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(T))
                .Select(x => (T)x.GetRawConstantValue())
                .ToList();
        }

        public static List<T> GetAllPublicProperty<T>(this Type type)
        {
            return type
                .GetFields(BindingFlags.Public | BindingFlags.GetProperty)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(T))
                .Select(x => (T)x.GetRawConstantValue())
                .ToList();
        }

        public static List<string> GetAllPublicInstance(this Type type)
        {
            return type
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(fi => fi.IsPublic && !fi.IsConstructor && !fi.IsAbstract && !fi.IsAbstract)
                .Select(x => x.Name)
                .ToList();
        }

        public static List<string> GetAllPublicProperty(this Type type)
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(x => x.Name)
                .ToList();
        }

        public static List<PropertyInfo> GetAllPublicCollection(this Type type)
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.Instance)
                .Where(fi => fi.PropertyType.Name == typeof(ICollection<>).Name)
                .Select(x => x)
                .ToList();
        }

        public static List<PropertyInfo> GetAllPublicObject(this Type type)
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.Instance)
                .Where(fi => fi.PropertyType.Name == typeof(ICollection<>).Name)
                .Select(x => x)
                .ToList();
        }
    }
}
