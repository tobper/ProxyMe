using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProxyMe.Emit;
using ProxyMe.Tests.Models;

namespace ProxyMe.Tests.Emit
{
    [TestClass]
    public class DynamicContractBuilderTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "A dynamic contract can only be created for interfaces.")]
        public void DynamicContractBuilder_CreateType_ShouldThrowException_ForClasses()
        {
            // Arrange
            var builder = new DynamicContractBuilder();

            // Act
            builder.CreateType(typeof(Foo));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "A dynamic contract can not be created for an interface with methods.")]
        public void DynamicContractBuilder_CreateType_ShouldThrowException_ForInterfacesWithMethods()
        {
            // Arrange
            var builder = new DynamicContractBuilder();

            // Act
            builder.CreateType(typeof(IBar));
        }
    }
}