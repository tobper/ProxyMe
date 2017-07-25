using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProxyMe.Tests.Models;

namespace ProxyMe.Tests
{
    [TestClass]
    public class ProxyTests
    {
        [TestMethod]
        public void Proxy_Create_ShouldReturnObject()
        {
            var foo = new Foo();

            // Act
            var proxy = Proxy.Create<IFoo>(foo);

            // Assert
            proxy.Should().NotBeNull();
        }

        [TestMethod]
        public void Proxy_SetProperty_ShouldSetTargetProperty()
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
        public void Proxy_GetProperty_ShouldGetTargetProperty()
        {
            // Arrange
            var foo = new Foo { Number = 42 };

            // Act
            var proxy = Proxy.Create<IFoo>(foo);

            // Assert
            proxy.Number.Should().Be(42);
        }

        [TestMethod]
        public void Proxy_MethodCall_ShouldCallTargetMethod()
        {
            // Arrange
            var target = new Bar();
            var proxy = Proxy.Create<IBar>(target);

            // Act
            proxy.SetValue(42);

            // Assert
            proxy.GetValue().Should().Be(42);
            target.GetValue().Should().Be(42);
        }
    }
}
