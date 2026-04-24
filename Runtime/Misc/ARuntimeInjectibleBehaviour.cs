using System;
using UnityEngine;

namespace JakubKrizanovsky.DependencyInjection
{
    public abstract class ARuntimeInjectibleBehaviour : MonoBehaviour
    {
        protected virtual void Awake() {
            DependencyInjector.Inject(this);

            // Also check if this is a service and register it if so
            Type type = this.GetType();
            if(Attribute.IsDefined(type, typeof(ServiceAttribute))) {
                DependencyInjector.RegisterService(this);
            }
        }
    }
}