namespace ProxyMe.Tests.Models
{
    public class Bar : IBar
    {
        private int _value;

        public int GetValue()
        {
            return _value;
        }

        public void SetValue(int value)
        {
            _value = value;
        }
    }
}