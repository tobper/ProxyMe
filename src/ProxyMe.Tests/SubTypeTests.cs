using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProxyMe.Tests.Models;

namespace ProxyMe.Tests
{
    [TestClass]
    public class SubTypeTests
    {
        [TestMethod]
        public void SubType_Create_ShouldNotReturnNull()
        {
            // Act
            var proxy = Proxy.CreateSubType<Foo>();

            // Assert
            proxy.Should().NotBeNull();
        }
    }
}
