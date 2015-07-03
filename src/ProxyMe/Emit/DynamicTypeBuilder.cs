using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ProxyMe.Caching;
using ProxyMe.Emit.Extensions;

namespace ProxyMe.Emit
{
    /// <summary>
    ///     Defines and creates new instances of classes and interfaces during run time.
    ///     Inherit from this class to add specific implementation of members.
    /// </summary>
    public class DynamicTypeBuilder
    {
        /// <summary>
        ///     Returns the reference type to define a new type for.
        /// </summary>
        protected Type ReferenceType { get; private set; }

        /// <summary>
        ///     Returns the <see cref="System.Reflection.TypeInfo"/> representation
        ///     for <see cref="DynamicTypeBuilder.ReferenceType"/>.
        /// </summary>
        protected TypeInfo ReferenceTypeInfo { get; private set; }

        /// <summary>
        ///     Creates the definition for the new <see cref="System.Type"/>.
        /// </summary>
        /// <param name="referenceType">
        ///     The reference <see cref="System.Type"/> to define an implementation for.
        /// </param>
        /// <returns>
        ///     Returns the dynamically created <see cref="System.Type"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        ///     <paramref name="referenceType"/> contains methods and no method implementation
        ///     has been defined.
        /// </exception>
        /// <exception cref="TypeLoadException">
        ///     The type cannot be loaded. For example, it contains a static method that
        ///     has the calling convention System.Reflection.CallingConventions.HasThis.
        /// </exception>
        public virtual Type CreateType(Type referenceType)
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

        /// <summary>
        ///     Extensíon point to do some cleanup. Called after the type has been created.
        /// </summary>
        protected virtual void Cleanup()
        {
        }

        /// <summary>
        ///     Deafult field implementation doesn't do anyhting. Override to provide
        ///     implementation for fields.
        /// </summary>
        /// <param name="typeBuilder">
        ///     The <see cref="System.Reflection.Emit.TypeBuilder"/> for the type being defined.
        /// </param>
        protected virtual void DefineFields(TypeBuilder typeBuilder)
        {
        }

        /// <summary>
        ///     Defines implementation for default constructor.
        ///     Override to provide implementation for other constructors.
        /// </summary>
        /// <param name="typeBuilder">
        ///     The <see cref="System.Reflection.Emit.TypeBuilder"/> for the type being defined.
        /// </param>
        protected virtual void DefineConstructors(TypeBuilder typeBuilder)
        {
            typeBuilder.DefineDefaultConstructor();
        }

        /// <summary>
        ///     Defines the members of the type being defined.
        /// </summary>
        /// <param name="typeBuilder">
        ///     The <see cref="System.Reflection.Emit.TypeBuilder"/> for the type being defined.
        /// </param>
        protected virtual void DefineMembers(TypeBuilder typeBuilder)
        {
            DefineFields(typeBuilder);
            DefineConstructors(typeBuilder);
            DefineProperties(typeBuilder);
            DefineMethods(typeBuilder);
        }

        /// <summary>
        ///     Calls <see cref="DefineProperty"/> for each property on <see cref="ReferenceType"/>.
        ///     Override to provide other implementation of properties.
        /// </summary>
        /// <param name="typeBuilder">
        ///     The <see cref="System.Reflection.Emit.TypeBuilder"/> for the type being defined.
        /// </param>
        protected virtual void DefineProperties(TypeBuilder typeBuilder)
        {
            foreach (var property in ReferenceTypeInfo.DeclaredProperties)
            {
                DefineProperty(typeBuilder, property);
            }
        }

        /// <summary>
        ///     Defines a property with a backing field for the value. Override to provide other
        ///     implemantation of the property.
        /// </summary>
        /// <param name="typeBuilder">
        ///     The <see cref="System.Reflection.Emit.TypeBuilder"/> for the type being defined.
        /// </param>
        /// <param name="property">
        ///     The <see cref="System.Reflection.PropertyInfo"/> for the property being defined.
        /// </param>
        protected virtual void DefineProperty(TypeBuilder typeBuilder, PropertyInfo property)
        {
            typeBuilder.DefinePropertyWithBackingField(property);
        }

        /// <summary>
        ///     Calls <see cref="DefineMethod"/> for each method without a special name on
        ///     <see cref="ReferenceType"/>. Override to provide other implementation of metods.
        /// </summary>
        /// <param name="typeBuilder">
        ///     The <see cref="System.Reflection.Emit.TypeBuilder"/> for the type being defined.
        /// </param>
        protected virtual void DefineMethods(TypeBuilder typeBuilder)
        {
            var methods = ReferenceTypeInfo.DeclaredMethods.Where(m => m.IsSpecialName == false);

            foreach (var method in methods)
            {
                DefineMethod(typeBuilder, method);
            }
        }

        /// <summary>
        ///     Throws a <see cref="System.NotSupportedException"/>. Override to provide
        ///     implementation of the method.
        /// </summary>
        /// <param name="typeBuilder">
        ///     The <see cref="System.Reflection.Emit.TypeBuilder"/> for the type being defined.
        /// </param>
        /// <param name="method">
        ///     The <see cref="System.Reflection.MethodInfo"/> for the method being defined.
        /// </param>
        /// <exception cref="NotSupportedException">
        ///     Methods are not supported by the default DynamicTypeBuilder.
        /// </exception>
        protected virtual void DefineMethod(TypeBuilder typeBuilder, MethodInfo method)
        {
            throw new NotSupportedException("Method support has not been implemented.");
        }

        /// <summary>
        ///     Creates the <see cref="System.Reflection.Emit.TypeBuilder"/> for a public class
        ///     based on <see cref="ReferenceType"/>.
        /// </summary>
        /// <param name="moduleBuilder"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
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

        /// <summary>
        ///     Returns the <see cref="System.Reflection.Emit.ModuleBuilder"/> where the new
        ///     type will be defined. Override to add the type to another module.
        /// </summary>
        /// <returns>
        ///     Returns the <see cref="System.Reflection.Emit.ModuleBuilder"/> the defined
        ///     type will be added to.
        /// </returns>
        protected virtual ModuleBuilder GetModuleBuilder()
        {
            return ProxyModuleBuilder.Get();
        }

        /// <summary>
        ///     Returns the name of the <see cref="System.Type"/> being defined. Default
        ///     implementation returns the name of <see cref="ReferenceType"/> suffixed with '`Proxy'.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetTypeName()
        {
            return ReferenceType.GetProxyTypeName("Proxy");
        }

        /// <summary>
        ///     Extensíon point to do some initialization. Called after <see cref="ReferenceType"/>
        ///     and <see cref="ReferenceTypeInfo"/> has been set, but before members have been
        ///     defined.
        /// </summary>
        protected virtual void Initialize()
        {
        }
    }
}