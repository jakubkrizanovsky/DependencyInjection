using System;

namespace JakubKrizanovsky.DependencyInjection
{
    public interface IDependencyInjectorContext
    {
        public void RegisterService<T>(T service);
        public void RegisterService(Type type, object service);
        public bool TryResolve(Type type, out object service);
    }
}