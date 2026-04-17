using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace JakubKrizanovsky.DependencyInjection
{
    internal static class InjectableMembersCache
    {
        private const BindingFlags FLAGS = BindingFlags.Instance 
                | BindingFlags.Public 
                | BindingFlags.NonPublic
                | BindingFlags.DeclaredOnly;

        private static readonly Dictionary<Type, InjectableMembers> _cache = new();

        public static InjectableMembers GetInjectableMembers(Type type) {
            if(_cache.TryGetValue(type, out InjectableMembers members)) {
                return members;
            }

            List<FieldInfo> fields = new();
            List<PropertyInfo> properties = new();

            // Walk the class hierarchy and find any members in this class or parents
            Type currentType = type;
            while(currentType != null && currentType != typeof(MonoBehaviour)
                    && currentType != typeof(object)) {
                // Find all fields with [Inject] attribute
                fields.AddRange(currentType.GetFields(FLAGS)
                        .Where(f => Attribute.IsDefined(f, typeof(InjectAttribute))));

                // Find all properties with [Inject] attribute
                properties.AddRange(currentType.GetProperties(FLAGS)
                        .Where(p => Attribute.IsDefined(p, typeof(InjectAttribute)) && p.CanWrite));

                currentType = currentType.BaseType;
            }            

            members = new InjectableMembers { 
                HasAny = fields.Any() || properties.Any(),
                Fields = fields, 
                Properties = properties 
            };

            _cache[type] = members;
            
            return members;
        }
    }
}