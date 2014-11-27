namespace ProxyMe.Tests.Models
{
    public interface IValues
    {
        bool Bool { get; set; }
        byte Byte { get; set; }
        sbyte ShortByte { get; set; }
        char Char { get; set; }
        decimal Decimal { get; set; }
        double Double { get; set; }
        float Float { get; set; }
        int Integer { get; set; }
        uint UnsignedInteger { get; set; }
        long Long { get; set; }
        ulong UnsignedLong { get; set; }
        object Object { get; set; }
        short Short { get; set; }
        ushort UnsignedShort { get; set; }
        string String { get; set; }
        Choice Choice { get; set; }
        Struct Struct { get; set; }
        IFoo Foo { get; set; }
    }
}