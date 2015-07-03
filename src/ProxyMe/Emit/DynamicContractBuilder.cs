using System;
using System.Linq;
using System.Reflection.Emit;
using ProxyMe.Emit.Extensions;

namespace ProxyMe.Emit
{
    public class DynamicContractBuilder : DynamicTypeBuilder
    {
        protected override void DefineConstructors(TypeBuilder typeBuilder)
        {
            typeBuilder.DefineDefaultConstructor();
            typeBuilder.DefineInitializationConstructor(ReferenceType);
        }

        protected override TypeBuilder DefineType(ModuleBuilder moduleBuilder, string typeName)
        {
            if (ReferenceType.IsInterface == false)
                throw new InvalidOperationException("A dynamic contract can only be created for interfaces.");

            if (ReferenceTypeInfo.DeclaredMethods.Any(m => m.IsSpecialName == false))
                throw new InvalidOperationException("A dynamic contract can not be created for an interface with methods.");

            return base.DefineType(moduleBuilder, typeName);
        }

        protected override string GetTypeName()
        {
            return ReferenceType.GetProxyTypeName("DynamicContract");
        }
    }
}