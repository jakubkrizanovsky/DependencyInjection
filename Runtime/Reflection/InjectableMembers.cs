using System.Collections.Generic;
using System.Reflection;

namespace JakubKrizanovsky.DependencyInjection
{
    internal struct InjectableMembers {
        public bool HasAny;
        public List<FieldInfo> Fields;
        public List<PropertyInfo> Properties;
    }
}