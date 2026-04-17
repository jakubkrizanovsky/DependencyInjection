using System;
using UnityEngine;

namespace JakubKrizanovsky.DependencyInjection
{
    public abstract class ARuntimeInjectibleBehaviour : MonoBehaviour
    {
        [Inject]
        private LazyInject<IDependencyInjectorContext> _injectorContext;

        protected virtual void Awake() {
            DependencyInjector.Inject(this);

            // Also check if this is a service and register it if so
            Type type = this.GetType();
            if(Attribute.IsDefined(type, typeof(ServiceAttribute))) {
                _injectorContext.Value.RegisterService(type, this);
            }
        }
    }
}