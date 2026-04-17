using System;

namespace JakubKrizanovsky.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class InjectAttribute : Attribute
    {
        
    }
}