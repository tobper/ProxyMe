using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProxyMe.Tests.Models;

namespace ProxyMe.Tests
{
    [TestClass]
    public class NamespaceTests
    {
        [TestMethod]
        public void NamespaceShouldBeSameAsContractType()
        {
            // Act
            var proxy = Proxy.CreateContract<IFoo>();

            // Assert
            var proxyType = proxy.GetType();
            var contractType = typeof(IFoo);
            var actualNamespace = proxyType.Namespace;
            var expectedNamespace = contractType.Namespace;

            actualNamespace.Should().Be(expectedNamespace);
        }

        [TestMethod]
        public void NamespaceShouldBeSameAsProxyType()
        {
            var foo = new Foo();

            // Act
            var proxy = Proxy.Create<IFoo>(foo);

            // Assert
            var proxyType = proxy.GetType();
            var actualNamespace = proxyType.Namespace;
            var expectedNamespace = foo.GetType().Namespace;

            actualNamespace.Should().Be(expectedNamespace);
        }
    }
}