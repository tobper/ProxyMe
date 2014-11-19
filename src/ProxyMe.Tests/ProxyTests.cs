using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProxyMe.Tests.Models;

namespace ProxyMe.Tests
{
    [TestClass]
    public class ProxyTests
    {
        [TestMethod]
        public void ProxyShouldBeCreated()
        {
            var foo = new Foo();

            // Act
            var proxy = Proxy.Create<IFoo>(foo);

            // Assert
            proxy.Should().NotBeNull();
        }

        [TestMethod]
        public void ProxiedPropertyShouldSetTargetValue()
        {
            // Arrange
            var foo = new Foo();
            var proxy = Proxy.Create<IFoo>(foo);

            // Act
            proxy.Number = 42;

            // Assert
            foo.Number.Should().Be(42);
        }

        [TestMethod]
        public void ProxiedPropertyShouldReadTargetValue()
        {
            // Arrange
            var foo = new Foo { Number = 42 };

            // Act
            var proxy = Proxy.Create<IFoo>(foo);

            // Assert
            proxy.Number.Should().Be(42);
        }
    }
}
