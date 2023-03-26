using Avalonia.Media;

namespace SimplePaint.Models.Safes
{
    internal interface ISafe
    {
        public bool Valid { get; }
        public void Set(string str);
        public string Value { get; set; }
        public IBrush Color { get; }
    }
}
