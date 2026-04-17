using UnityEngine;

namespace JakubKrizanovsky.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static T InstantiateAndInject<T>(this MonoBehaviour context, T prefab) 
                where T : MonoBehaviour 
        {
            // Temporary parent to instantiate the prefab inactive
            GameObject parent = new("InstantiateContainer");
            parent.SetActive(false);
            
            // Instantiate and inject
            T instance = Object.Instantiate(prefab, parent.transform);
            DependencyInjector.Inject(instance);

            // Clean up temp parent
            instance.transform.SetParent(null, true);
            Object.Destroy(parent);

            return instance;
        }

        public static T InstantiateAndInjectAll<T>(this MonoBehaviour context, T prefab) 
                where T : Object 
        {
            // Temporary parent to instantiate the prefab inactive
            GameObject parent = new("InstantiateContainer");
            parent.SetActive(false);
            
            T instance = Object.Instantiate(prefab);

            MonoBehaviour[] components = {};
            Transform transform = null; 
            if(instance is GameObject instanceGO) {
                components = instanceGO.GetComponentsInChildren<MonoBehaviour>(true);
                transform = instanceGO.transform;
            } else if(instance is Component instanceComponent) {
                components = instanceComponent.GetComponentsInChildren<MonoBehaviour>(true);
                transform = instanceComponent.transform;
            }
            
            foreach(MonoBehaviour component in components) {
                DependencyInjector.Inject(component);
            }

            // Clean up temp parent
            transform.SetParent(null, true);
            Object.Destroy(parent);

            return instance;
        }
    }
}
