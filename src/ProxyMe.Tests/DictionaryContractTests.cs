using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProxyMe.Tests.Models;

namespace ProxyMe.Tests
{
    [TestClass]
    public class DictionaryContractTests
    {
        [TestMethod]
        public void DictionaryContract_Create_ShouldReturnObject_ForInterfaces()
        {
            // Act
            var properties = new Dictionary<string, object>();
            var foo = Proxy.CreateContract<IFoo>(properties);

            // Assert
            foo.Should().NotBeNull();
        }

        [TestMethod]
        public void DictionaryContract_Create_ShouldReturnObject_ForGenerics()
        {
            // Act
            var properties = new Dictionary<string, object>();
            var foo = Proxy.CreateContract<IFoo<int>>(properties);

            // Assert
            foo.Should().NotBeNull();
        }

        [TestMethod]
        public void DictionaryContract_Create_ShouldBeInitializedWithSuppliedValues()
        {
            // Act
            var properties = new Dictionary<string, object>
            {
                { "Number", 42 }
            };

            var foo = Proxy.CreateContract<IFoo>(properties);

            // Assert
            foo.Should().NotBeNull();
            foo.Number.Should().Be(42);
        }

        [TestMethod]
        public void DictionaryContract_Create_ShouldBeInitializedWithSuppliedValues_ForGenerics()
        {
            // Act
            var properties = new Dictionary<string, object>
            {
                { "Value", 42 }
            };

            var foo = Proxy.CreateContract<IFoo<int>>(properties);

            // Assert
            foo.Should().NotBeNull();
            foo.Value.Should().Be(42);
        }

        [TestMethod]
        public void DictionaryContract_Create_ShouldSupportModifyingProperties()
        {
            // Arrange
            var properties = new Dictionary<string, object>();
            var foo = Proxy.CreateContract<IFoo>(properties);

            // Act
            foo.Number = 42;

            // Assert
            properties["Number"].Should().Be(42);
        }

        [TestMethod]
        public void DictionaryContract_Create_ShouldBeInitializedWithDefaultValues()
        {
            // Arrange
            var properties = new Dictionary<string, object>();

            // Act
            Proxy.CreateContract<IValues>(properties);

            // Assert
            properties.Count.Should().Be(18);
            properties["Bool"].Should().Be(default(bool));
            properties["Byte"].Should().Be(default(byte));
            properties["ShortByte"].Should().Be(default(sbyte));
            properties["Char"].Should().Be(default(char));
            properties["Decimal"].Should().Be(default(decimal));
            properties["Double"].Should().Be(default(double));
            properties["Float"].Should().Be(default(float));
            properties["Integer"].Should().Be(default(int));
            properties["UnsignedInteger"].Should().Be(default(uint));
            properties["Long"].Should().Be(default(long));
            properties["UnsignedLong"].Should().Be(default(ulong));
            properties["Object"].Should().Be(default(object));
            properties["Short"].Should().Be(default(short));
            properties["UnsignedShort"].Should().Be(default(ushort));
            properties["String"].Should().Be(default(string));
            properties["Choice"].Should().Be(default(Choice));
            properties["Struct"].Should().Be(default(Struct));
            properties["Foo"].Should().Be(null);
        }

        [TestMethod]
        public void DictionaryContract_Create_ShouldNotBeInitializeWithDefaultValuesIfPropertiesAlreadyHaveValues()
        {
            // Arrange
            var obj = new Object();
            var str = new Struct { Number = 42 };
            var foo = new Foo();
            var properties = new Dictionary<string, object>
            {
                { "Bool", true },
                { "Byte", (byte)1 },
                { "ShortByte", (sbyte)2 },
                { "Char", (char)3 },
                { "Decimal", 4m },
                { "Double", 5d },
                { "Float", 6f },
                { "Integer", 7 },
                { "UnsignedInteger", 8u },
                { "Long", 9L },
                { "UnsignedLong", 10UL },
                { "Object", obj },
                { "Short", (short)11 },
                { "UnsignedShort", (ushort)12 },
                { "String", "Hello" },
                { "Choice", Choice.Yes },
                { "Struct", str },
                { "Foo", foo },
            };

            // Act
            Proxy.CreateContract<IValues>(properties);

            // Assert
            properties.Count.Should().Be(18);
            properties["Bool"].Should().Be(true);
            properties["Byte"].Should().Be((byte)1);
            properties["ShortByte"].Should().Be((sbyte)2);
            properties["Char"].Should().Be((char)3);
            properties["Decimal"].Should().Be(4m);
            properties["Double"].Should().Be(5d);
            properties["Float"].Should().Be(6f);
            properties["Integer"].Should().Be(7);
            properties["UnsignedInteger"].Should().Be(8u);
            properties["Long"].Should().Be(9L);
            properties["UnsignedLong"].Should().Be(10UL);
            properties["Object"].Should().BeSameAs(obj);
            properties["Short"].Should().Be((short)11);
            properties["UnsignedShort"].Should().Be((ushort)12);
            properties["String"].Should().Be("Hello");
            properties["Choice"].Should().Be(Choice.Yes);
            properties["Struct"].Should().Be(str);
            properties["Foo"].Should().BeSameAs(foo);
        }
    }
}