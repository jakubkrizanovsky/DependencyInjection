using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JakubKrizanovsky.DependencyInjection
{
    public static class DependencyInjector
    {
        private static readonly List<ADependencyInjectorContext> _injectorContexts = new();
        private static readonly Dictionary<Scene, ADependencyInjectorContext> _injectorContextsByScene = new();

        public static void RegisterContext(ADependencyInjectorContext context) {
            _injectorContexts.Add(context);
            _injectorContextsByScene.Add(context.gameObject.scene, context);
        }

        public static void UnregisterContext(ADependencyInjectorContext context) {
            _injectorContexts.Remove(context);
            _injectorContextsByScene.Remove(context.gameObject.scene);
        }

        public static void Inject(MonoBehaviour injectable) {
            Type type = injectable.GetType();
            InjectableMembers injectableMembers = InjectableMembersCache.GetInjectableMembers(type);

            // Inject fields
            foreach(FieldInfo field in injectableMembers.Fields) {
                if(TryHandleLazyInject(field, injectable)) continue;

                object service = Resolve(field.FieldType, injectable);
                if(service != null) {
                    field.SetValue(injectable, service);
                }
            }

            // Inject Properties
            foreach(PropertyInfo property in injectableMembers.Properties) {
                object service = Resolve(property.PropertyType, injectable);
                if(service != null) {
                    property.SetValue(injectable, service);
                }
            }
        }

        private static bool TryHandleLazyInject(FieldInfo field, MonoBehaviour injectable) {
            if(field.FieldType.IsGenericType && 
                field.FieldType.GetGenericTypeDefinition() == typeof(LazyInject<>))
            {
                object lazyInstance = Activator.CreateInstance(field.FieldType, new object[] {injectable});
                field.SetValue(injectable, lazyInstance);
                return true;
            }
            return false;
        }

        public static T Resolve<T>(MonoBehaviour injectable = null) {
            object service = Resolve(typeof(T), injectable);
            return service != null ? (T) service : default;
        }

        public static object Resolve(Type type, MonoBehaviour injectable){
			if(TryResolve(type, injectable, out object service)) {
                return service;
			}

			// Failed to find
			Debug.LogError($"[DependencyInjector] Could not resolve dependency of type {type.Name}, " 
                    + $"dependency resolution intiated from {injectable}");
            return null;
        }

        public static bool TryResolve<T>(out T service, MonoBehaviour injectable = null) {
            if(TryResolve(typeof(T), injectable, out object serviceObject)) {
                service = (T) serviceObject;
                return true;
            }

            service = default;
            return false;
        }

        public static bool TryResolve(Type type, MonoBehaviour injectable, out object service){
            // Search in context of the same scene first
            ADependencyInjectorContext sameSceneContext = null;
            if(injectable != null && _injectorContextsByScene.TryGetValue(
                    injectable.gameObject.scene, out sameSceneContext)) 
            {
                if(sameSceneContext.TryResolve(type, out service)) {
                    return true;
                }
            }

            // Not found in the context of the same scene, try searching in contexts of all other scenes
            foreach(ADependencyInjectorContext sceneContext in _injectorContexts) {
                if(sceneContext == sameSceneContext) continue; // Do not search the same scene again

                if(sceneContext.TryResolve(type, out service)) {
                    return true;
                }
            }

            // Failed to find
            service = null;
            return false;
        }
    }
}
