using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ProxyMe.Caching;
using ProxyMe.Emit.Extensions;

namespace ProxyMe.Emit
{
    public class DynamicTypeBuilder
    {
        /// <summary>
        ///     Returns the reference type to define a new type for.
        /// </summary>
        protected Type ReferenceType { get; private set; }

        /// <summary>
        /// Returns the <see cref="System.Reflection.TypeInfo"/> representation of the specified type.
        /// </summary>
        protected TypeInfo ReferenceTypeInfo { get; private set; }

        public virtual Type CreateType(Type type)
        {
            ReferenceType = referenceType;
            ReferenceTypeInfo = referenceType.GetTypeInfo();

            try
            {
                Initialize();

                var typeName = GetTypeName();
                var moduleBuilder = GetModuleBuilder();
                var typeBuilder = DefineType(moduleBuilder, typeName);

                DefineMembers(typeBuilder);

                return typeBuilder.CreateType();
            }
            finally
            {
                Cleanup();
            }
        }

        protected virtual void Cleanup()
        {
        }

        protected virtual void DefineFields(TypeBuilder typeBuilder)
        {
        }

        protected virtual void DefineConstructors(TypeBuilder typeBuilder)
        {
            typeBuilder.DefineDefaultConstructor();
        }

        protected virtual void DefineMembers(TypeBuilder typeBuilder)
        {
            DefineFields(typeBuilder);
            DefineConstructors(typeBuilder);
            DefineProperties(typeBuilder);
            DefineMethods(typeBuilder);
        }

        protected virtual void DefineProperties(TypeBuilder typeBuilder)
        {
            foreach (var property in ReferenceTypeInfo.DeclaredProperties)
            {
                DefineProperty(typeBuilder, property);
            }
        }

        protected virtual void DefineProperty(TypeBuilder typeBuilder, PropertyInfo property)
        {
            typeBuilder.DefinePropertyWithBackingField(property);
        }

        protected virtual void DefineMethods(TypeBuilder typeBuilder)
        {
            var methods = ReferenceTypeInfo.DeclaredMethods.Where(m => m.IsSpecialName == false);

            foreach (var method in methods)
            {
                DefineMethod(typeBuilder, method);
            }
        }

        protected virtual void DefineMethod(TypeBuilder typeBuilder, MethodInfo method)
        {
            throw new NotImplementedException("Method support has not been implemented.");
        }

        protected virtual TypeBuilder DefineType(ModuleBuilder moduleBuilder, string typeName)
        {
            var parent = (Type)null;
            var contracts = (Type[])null;

            if (ReferenceType.IsInterface)
                contracts = new[] { ReferenceType };
            else
                parent = ReferenceType;

            return moduleBuilder.DefineType(
                typeName,
                TypeAttributes.Class | TypeAttributes.Public,
                parent,
                contracts);
        }

        protected virtual ModuleBuilder GetModuleBuilder()
        {
            return ProxyModuleBuilder.Get();
        }

        protected virtual string GetTypeName()
        {
            return ReferenceType.GetProxyTypeName("Proxy");
        }

        protected virtual void Initialize()
        {
        }
    }
}