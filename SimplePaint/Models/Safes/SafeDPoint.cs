using System;
using Avalonia;
using Avalonia.Media;

namespace SimplePaint.Models.Safes
{
    public class SafeDPoint : ForcePropertyChange, ISafe
    {
        private double _x, _y;
        private bool _valid = true;
        private readonly Action<object?>? _hook;
        private readonly object? _inst;
        private readonly string _separator;
        public SafeDPoint(double x, double y, Action<object?>? hook = null, object? inst = null,
            bool altSeparator = false)
        {
            _x = x;
            _y = y;
            this._hook = hook;
            this._inst = inst;
            _separator = altSeparator ? " " : ",";
        }
        public SafeDPoint(string init, Action<object?>? hook = null, object? inst = null, bool altSeparator = false)
        {
            this._hook = hook;
            this._inst = inst;
            _separator = altSeparator ? " " : ",";
            Set(init);
            if (!_valid) throw new FormatException("Невалидный формат инициализации SafeDPoint: " + init);
        }
        public Point Point
        {
            get => new(_x, _y);
        }
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
            _x = p.X;
            _y = p.Y;
            _valid = true;
        }
        public void Set(double x, double y)
        {
            _x = x;
            _y = y;
            _valid = true;
        }
        public bool Valid => _valid;
        public void Set(string str)
        {
            var ss = str.TrimAll().Replace('.', ',').Split(_separator);
            if (ss.Length != 2)
            {
                Upd_valid(false);
                return;
            }
            double a, b;
            try
            {
                a = double.Parse(ss[0]);
                b = double.Parse(ss[1]);
            }
            catch
            {
                Upd_valid(false);
                return;
            }
            if (Math.Abs(a) > 10000 || Math.Abs(b) > 10000)
            {
                Upd_valid(false);
                return;
            }

            _x = a;
            _y = b;
            Upd_valid(true);
        }
        public string Value
        {
            get
            {
                Re_check();
                return _x + _separator + _y;
            }
            set
            {
                Set(value);
                UpdProperty(nameof(Color));
            }
        }
        public IBrush Color
        {
            get => _valid ? Brushes.Lime : Brushes.Pink;
        }
    }
}