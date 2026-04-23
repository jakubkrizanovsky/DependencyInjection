using System;

namespace JakubKrizanovsky.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ServiceAttribute : Attribute
    {
        public bool Persistent {get; set;} = false;
        private bool? _unique = null;
        public bool Unique {
            get => _unique ?? Persistent;
            set => _unique = value;
        }
    }
}