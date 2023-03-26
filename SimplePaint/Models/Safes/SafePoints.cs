using System;
using System.Linq;
using Avalonia;
using Avalonia.Media;

namespace SimplePaint.Models.Safes
{
    public class SafePoints : ForcePropertyChange, ISafe
    {
        private Points _points = new();
        private bool _valid = true;
        private readonly Action<object?>? _hook;
        private readonly object? _inst;
        public SafePoints(string init, Action<object?>? hook = null, object? inst = null)
        {
            this._hook = hook; this._inst = inst;
            Set(init);
            if (!_valid) throw new FormatException("Недействующий формат инициализации SafePoints: " + init);
        }
        public Points Points => _points;
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
        public void Set(Points ps)
        {
            _points = ps;
            _valid = true;
        }
        public bool Valid => _valid;
        public void Set(string str)
        {
            Points list = new();
            foreach (var p in str.Split())
            {
                if (p.Length == 0) continue;
                var ss = p.Split(",");
                if (ss.Length != 2) { Upd_valid(false); return; }
                int a, b;
                try
                {
                    a = int.Parse(ss[0]);
                    b = int.Parse(ss[1]);
                }
                catch { Upd_valid(false); return; }

                if (Math.Abs(a) > 10000 || Math.Abs(b) > 10000) { Upd_valid(false); return; }
                list.Add(new Point(a, b));
            }
            _points = list;
            Upd_valid(true);
        }

        public string Value
        {
            get { Re_check(); return String.Join(" ", _points.Select(p => p.X + "," + p.Y)); }
            set
            {
                Set(value);
                UpdProperty(nameof(Color));
            }
        }

        public IBrush Color { get => _valid ? Brushes.Lime : Brushes.Pink; }
    }
}
