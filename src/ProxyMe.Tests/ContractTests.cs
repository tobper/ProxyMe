using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProxyMe.Tests.Models;

namespace ProxyMe.Tests
{
    [TestClass]
    public class ContractTests
    {
        [TestMethod]
        public void Contract_Create_ShouldReturnObject_ForInterfaces()
        {
            // Act
            var foo = Proxy.CreateContract<IFoo>();

            // Assert
            foo.Should().NotBeNull();
        }

        [TestMethod]
        public void Contract_Create_ShouldReturnObject_ForGenerics()
        {
            // Act
            var foo = Proxy.CreateContract<IFoo<int>>();

            // Assert
            foo.Should().NotBeNull();
        }

        [TestMethod]
        public void Contract_Create_ShouldReturnObject_ForUntyped()
        {
            // Act
            var foo = Proxy.CreateContract(typeof(IFoo));

            // Assert
            foo.Should().NotBeNull();
            foo.Should().BeAssignableTo<IFoo>();
        }

        [TestMethod]
        public void Contract_Create_ShouldBeInitializedWithSuppliedValues()
        {
            // Act
            var foo = Proxy.CreateContract<IFoo>(c =>
            {
                c.Number = 42;
            });

            // Assert
            foo.Should().NotBeNull();
            foo.Number.Should().Be(42);
        }

        [TestMethod]
        public void Contract_Create_ShouldBeInitializedWithSuppliedValues_ForGenerics()
        {
            // Act
            var foo = Proxy.CreateContract<IFoo<int>>(c =>
            {
                c.Value = 42;
            });

            // Assert
            foo.Should().NotBeNull();
            foo.Value.Should().Be(42);
        }

        [TestMethod]
        public void Contract_SetProperty_ShouldUpdateTheProperty()
        {
            // Arrange
            var foo = Proxy.CreateContract<IFoo>();

            // Act
            foo.Number = 42;

            // Assert
            foo.Number.Should().Be(42);
        }
    }
}