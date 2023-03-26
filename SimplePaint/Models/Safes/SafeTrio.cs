using System;
using Avalonia.Media;

namespace SimplePaint.Models.Safes {
    public class SafeTrio: ForcePropertyChange, ISafe {
        private bool _valid = true;
        private readonly Action<object?>? _hook;
        private readonly object? _inst;
        private readonly string _separator;
        public SafeTrio(int x, int y, int z, Action<object?>? hook = null, object? inst = null, bool altSeparator = false) {
            X = x; Y = y; Z = z; this._hook = hook; this._inst = inst;
            _separator = altSeparator ? " " : ",";
        }
        public SafeTrio(string init, Action<object?>? hook = null, object? inst = null, bool altSeparator = false) {
            this._hook = hook; this._inst = inst;
            _separator = altSeparator ? " " : ",";
            Set(init);
            if (!_valid) throw new FormatException("Невалидный формат инициализации SafeTrio: " + init);
        }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }
        private void Upd_valid(bool v) {
            _valid = v;
            _hook?.Invoke(_inst);
        }
        private void Re_check() {
            if (!_valid) {
                _valid = true;
            }
        }
        public bool Valid => _valid;
        public void Set(string str) {
            var ss = str.TrimAll().Split(_separator);
            if (ss.Length != 3) { Upd_valid(false); return; }
            int a, b, c;
            try {
                a = int.Parse(ss[0]);
                b = int.Parse(ss[1]);
                c = int.Parse(ss[2]);
            } catch { Upd_valid(false); return; }
            if (Math.Abs(a) > 10000 || Math.Abs(b) > 10000 || Math.Abs(c) > 10000) { Upd_valid(false); return; }
            X = a; Y = b; Z = c;
            Upd_valid(true);
        }
        public string Value {
            get { Re_check(); return X + _separator + Y + _separator + Z; }
            set {
                Set(value);
                UpdProperty(nameof(Color));
            }
        }
        public IBrush Color { get => _valid ? Brushes.Lime : Brushes.Pink; }
    }
}