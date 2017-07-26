using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProxyMe.Emit;
using ProxyMe.Tests.Models;

namespace ProxyMe.Tests.Emit
{
    [TestClass]
    public class DynamicSubTypeBuilderTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "A dynamic sub type can only be created for classes.")]
        public void DynamicSubTypeBuilder_CreateType_ShouldThrowException_ForInterfaces()
        {
            // Arrange
            var builder = new DynamicSubTypeBuilder();

            // Act
            builder.CreateType(typeof(IFoo));
        }
    }
}