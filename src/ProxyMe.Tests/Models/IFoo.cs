namespace ProxyMe.Tests.Models
{
    public interface IFoo
    {
        int Number { get; set; }
    }

    public interface IFoo<T>
    {
        T Number { get; set; }
    }
}