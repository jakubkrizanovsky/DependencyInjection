using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace JakubKrizanovsky.DependencyInjection
{
    [DefaultExecutionOrder(-1000)]
    public class ADependencyInjectorContext : MonoBehaviour, IDependencyInjectorContext
    {
        private readonly Dictionary<Type, object> _services = new();

        protected virtual void Awake() {
            DependencyInjector.RegisterContext(this);
            DiscoverAndInject();
        }

        private void OnDestroy() {
            DependencyInjector.UnregisterContext(this);
        }

        private void DiscoverAndInject() {
            List<MonoBehaviour> injectables = new();

            // Scan scene hierarchy for services and injectables
            GameObject[] rootGOs = gameObject.scene.GetRootGameObjects();
            foreach(GameObject rootGO in rootGOs) {
                MonoBehaviour[] components = rootGO.GetComponentsInChildren<MonoBehaviour>(true);
                foreach(MonoBehaviour component in components) {
                    Type type = component.GetType();
                    if(Attribute.IsDefined(type, typeof(ServiceAttribute))) {
                        RegisterService(type, component);
                        HandleServicePersistence(component, type);
                    }

                    if(component is IServiceProvider serviceProvider) {
                        RegisterService(serviceProvider.Service);
                    }

                    if(InjectableMembersCache.GetInjectableMembers(type).HasAny) {
                        injectables.Add(component);
                    }
                }
            }

            // Inject all discovered injectables
            foreach(MonoBehaviour injectable in injectables) {
                DependencyInjector.Inject(injectable);
            }
        }

        public void RegisterService<T>(T service) {
            RegisterService(service.GetType(), service);
        }

        public void RegisterService(Type type, object service) {
            _services[type] = service;

            foreach (Type interfaceType in type.GetInterfaces())  {
                _services[interfaceType] = service;
            }
        }

        public bool TryResolve(Type type, out object service) {
            return _services.TryGetValue(type, out service);
        }

        private void HandleServicePersistence(MonoBehaviour service, Type type) {
            ServiceAttribute attribute = type.GetCustomAttribute<ServiceAttribute>();
            if(attribute.Persistent) {
                service.transform.parent = null;
                DontDestroyOnLoad(service.gameObject);
            }
        }
	}
}