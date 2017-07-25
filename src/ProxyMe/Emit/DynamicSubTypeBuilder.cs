using System;
using System.Reflection.Emit;
using ProxyMe.Emit.Extensions;

namespace ProxyMe.Emit
{
    public class DynamicSubTypeBuilder : DynamicTypeBuilder
    {
        protected override TypeBuilder DefineType(ModuleBuilder moduleBuilder, string typeName)
        {
            if (ReferenceTypeInfo.IsInterface)
                throw new InvalidOperationException("A dynamic sub type can only be created for classes.");

            return base.DefineType(moduleBuilder, typeName);
        }

        protected override string GetTypeName()
        {
            return ReferenceType.GetProxyTypeName("DynamicSubType");
        }
    }
}