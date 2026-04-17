using System;

namespace JakubKrizanovsky.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ServiceAttribute : Attribute
    {
        public bool Persistent {get; set;} = false;
    }
}