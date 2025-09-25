using System;
using System.Collections.Generic;

namespace ProjectTools.DependencyManagement
{
    public static class Service
    {
        private static readonly Dictionary<Type, object> _instances = new();
        
        public static T Get<T>() where T : class
        {
            if (_instances.TryGetValue(typeof(T), out var instance))
            {
                return instance as T;
            }
            
            return null;
        }
        
        public static void Register<T>(T instance) where T : class
        {
            Register(typeof(T), instance);
        }
        
        public static void Register(Type type, object instance)
        {
            if (instance == null)
            {
                throw new Exception($"Dependency instance cannot be null.");
            }

            if (!_instances.TryAdd(type, instance))
            {
                throw new Exception($"{type.Name} is already registered!");
            }
        }
        
        public static void Unregister<T>(T instance) where T : class
        {
            Unregister(typeof(T), instance);
        }

        public static void Unregister(Type type, object instance)
        {
            _instances.Remove(type);
        }

        public static void ClearAll()
        {
            _instances.Clear();
        }
    }
}