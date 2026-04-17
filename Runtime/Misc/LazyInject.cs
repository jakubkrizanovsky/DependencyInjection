using UnityEngine;

namespace JakubKrizanovsky.DependencyInjection
{
    public class LazyInject<T>
    {
        private T _value;
        private readonly MonoBehaviour _injectable;

        public LazyInject(MonoBehaviour injectable) {
            _injectable = injectable;
        }

        // Resolve when accessed
        public T Value => _value ??= DependencyInjector.Resolve<T>(_injectable);

        // Implicit conversion
        public static implicit operator T(LazyInject<T> lazy) => lazy.Value;
    }
}