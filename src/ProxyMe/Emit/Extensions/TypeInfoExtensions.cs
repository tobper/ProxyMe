using System.Linq;
using System.Reflection;

namespace ProxyMe.Emit.Extensions
{
    public static class TypeInfoExtensions
    {
        public static ConstructorInfo GetDefaultConstructor(this TypeInfo type)
        {
            return type.
                DeclaredConstructors.
                Single(c => c.GetParameters().Length == 0);
        }
    }
}