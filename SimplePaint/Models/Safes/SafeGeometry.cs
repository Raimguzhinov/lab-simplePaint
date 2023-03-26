using System;
using Avalonia.Media;

namespace SimplePaint.Models.Safes
{
    public class SafeGeometry : ForcePropertyChange, ISafe
    {
        private Geometry _geom = Geometry.Parse("");
        private string _geomStr = "";
        private bool _valid = true;
        private readonly Action<object?>? _hook;
        private readonly object? _inst;
        public SafeGeometry(string init, Action<object?>? hook = null, object? inst = null)
        {
            this._hook = hook; this._inst = inst;
            Set(init);
            if (!_valid) throw new FormatException("Недействующий формат инициализации SafeGeometry: " + init);
        }
        public Geometry Geometry => _geom;
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
        public bool Valid => _valid;
        public void Set(string str)
        {
            Geometry data;
            try
            {
                data = Geometry.Parse(str);
            }
            catch { Upd_valid(false); return; }

            _geom = data;
            _geomStr = str;
            Upd_valid(true);
        }
        public string Value
        {
            get { Re_check(); return _geomStr; }
            set
            {
                Set(value);
                UpdProperty(nameof(Color));
            }
        }
        public IBrush Color { get => _valid ? Brushes.Lime : Brushes.Pink; }
    }
}
