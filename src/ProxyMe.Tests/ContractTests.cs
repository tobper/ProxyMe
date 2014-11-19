using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProxyMe.Tests.Models;

namespace ProxyMe.Tests
{
    [TestClass]
    public class ContractTests
    {
        [TestMethod]
        public void ContractShouldBeCreated()
        {
            // Act
            var foo = Proxy.CreateContract<IFoo>();

            // Assert
            foo.Should().NotBeNull();
        }

        [TestMethod]
        public void ContractShouldBeCreatedForGenerics()
        {
            // Act
            var foo = Proxy.CreateContract<IFoo<int>>();

            // Assert
            foo.Should().NotBeNull();
        }

        [TestMethod]
        public void ContractShouldBeCreatedWithInitializedValues()
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
        public void ContractShouldBeCreatedForGenericWithInitializedValues()
        {
            // Act
            var foo = Proxy.CreateContract<IFoo<int>>(c =>
            {
                c.Number = 42;
            });

            // Assert
            foo.Should().NotBeNull();
            foo.Number.Should().Be(42);
        }

        [TestMethod]
        public void ContractShouldSupportModifyingProperties()
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