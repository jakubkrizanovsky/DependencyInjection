using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JakubKrizanovsky.DependencyInjection
{
    [DefaultExecutionOrder(-1000)]
    public class ADependencyInjectorContext : MonoBehaviour
    {
        private readonly Dictionary<Type, object> _services = new();

        protected virtual void Awake() {
            DependencyInjector.RegisterContext(this);
            DiscoverAndInject(gameObject.scene);
        }

        private void OnDestroy() {
            DependencyInjector.UnregisterContext(this);
        }

        protected void DiscoverAndInject(Scene scene) {
            List<MonoBehaviour> injectables = new();

            // Scan scene hierarchy for services and injectables
            GameObject[] rootGOs = scene.GetRootGameObjects();
            foreach(GameObject rootGO in rootGOs) {
                MonoBehaviour[] components = rootGO.GetComponentsInChildren<MonoBehaviour>(true);
                foreach(MonoBehaviour component in components) {
                    Type type = component.GetType();
                    if(Attribute.IsDefined(type, typeof(ServiceAttribute))) {
                        ServiceAttribute attribute = type.GetCustomAttribute<ServiceAttribute>();
                        if(!attribute.Persistent) {
                            RegisterService(type, component);
                        } else {
                            // Persistent services are always registered as global
                            DependencyInjector.RegisterService(component, true);
                        }
                    }

                    if(component is IServiceProvider serviceProvider) {
                        object service = serviceProvider.Service;
                        if(serviceProvider.Persistent) {
                            DependencyInjector.RegisterService(service, true);
                            MakePersistent(service);
                        } else {
                            RegisterService(service);
                        }
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

        internal void RegisterService<T>(T service) {
            RegisterService(service.GetType(), service);
        }

        internal virtual void RegisterService(Type type, object service) {
            ServiceAttribute attribute = type.GetCustomAttribute<ServiceAttribute>();

            // Make sure unique services only get registered once
            if(attribute != null && attribute.Unique && _services.ContainsKey(type)) {
                if(service is MonoBehaviour serviceMB) {
                    Destroy(serviceMB);
                }

                return;
            }

            _services[type] = service;

            foreach (Type interfaceType in type.GetInterfaces())  {
                _services[interfaceType] = service;
            }
        }

        internal bool TryResolve(Type type, out object service) {
            return _services.TryGetValue(type, out service);
        }

        protected void MakePersistent(object service) {
            if(service is MonoBehaviour serviceMB) {
                serviceMB.transform.parent = null;
                DontDestroyOnLoad(serviceMB.gameObject);
            }
        }
	}
}