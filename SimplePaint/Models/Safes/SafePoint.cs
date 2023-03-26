using System;
using Avalonia;
using Avalonia.Media;

namespace SimplePaint.Models.Safes
{
    public class SafePoint : ForcePropertyChange, ISafe
    {
        private int _x, _y;
        private bool _valid = true;
        private readonly Action<object?>? _hook;
        private readonly string _separator;
        private readonly object? _inst;
        public SafePoint(int x, int y, Action<object?>? hook = null, object? inst = null, bool altSeparator = false)
        {
            _x = x; _y = y; this._hook = hook; this._inst = inst;
            _separator = altSeparator ? " " : ",";
        }
        public SafePoint(string init, Action<object?>? hook = null, object? inst = null, bool altSeparator = false)
        {
            this._hook = hook; this._inst = inst;
            _separator = altSeparator ? " " : ",";
            Set(init);
            if (!_valid) throw new FormatException("Недействующий формат инициализации SafePoint: " + init);
        }
        public Point Point { get => new(_x, _y); }
        private void Upd_valid(bool v)
        {
            _valid = v;
            _hook?.Invoke(_inst);
        }
        private void Re_check()
        {
            if (!_valid)
            {
                _valid = true;
            }
        }
        public void Set(Point p)
        {
            _x = (int)p.X;
            _y = (int)p.Y;
            _valid = true;
        }
        public void Set(int x, int y) {
            _x = x; _y = y;
            _valid = true;
        }
        public bool Valid => _valid;
        public void Set(string str)
        {
            var ss = str.TrimAll().Split(_separator);
            if (ss.Length != 2) { Upd_valid(false); return; }

            int a, b;
            try
            {
                a = int.Parse(ss[0]);
                b = int.Parse(ss[1]);
            }
            catch { Upd_valid(false); return; }

            if (Math.Abs(a) > 10000 || Math.Abs(b) > 10000) { Upd_valid(false); return; }

            _x = a; _y = b;
            Upd_valid(true);
        }
        public string Value
        {
            get { Re_check(); return _x + _separator + _y; }
            set
            {
                Set(value);
                UpdProperty(nameof(Color));
            }
        }
        public IBrush Color { get => _valid ? Brushes.Lime : Brushes.Pink; }
    }
}
