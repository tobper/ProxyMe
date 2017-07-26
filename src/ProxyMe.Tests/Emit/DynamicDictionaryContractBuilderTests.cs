using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProxyMe.Emit;
using ProxyMe.Tests.Models;

namespace ProxyMe.Tests.Emit
{
    [TestClass]
    public class DynamicDictionaryContractBuilderTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "A dynamic contract can only be created for interfaces.")]
        public void DynamicDictionaryContractBuilder_CreateType_ShouldThrowException_ForClasses()
        {
            // Arrange
            var builder = new DynamicDictionaryContractBuilder();

            // Act
            builder.CreateType(typeof(Foo));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "A dynamic contract can not be created for an interface with methods.")]
        public void DynamicDictionaryContractBuilder_CreateType_ShouldThrowException_ForInterfacesWithMethods()
        {
            // Arrange
            var builder = new DynamicDictionaryContractBuilder();

            // Act
            builder.CreateType(typeof(IBar));
        }
    }
}