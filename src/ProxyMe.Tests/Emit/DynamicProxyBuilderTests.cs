using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProxyMe.Emit;
using ProxyMe.Tests.Models;

namespace ProxyMe.Tests.Emit
{
    [TestClass]
    public class DynamicProxyBuilderTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "A dynamic contract can only be created for interfaces.")]
        public void DynamicProxyBuilder_CreateType_ShouldThrowException_ForClasses()
        {
            // Arrange
            var builder = new DynamicProxyBuilder();

            // Act
            builder.CreateType(typeof(Foo));
        }
    }
}