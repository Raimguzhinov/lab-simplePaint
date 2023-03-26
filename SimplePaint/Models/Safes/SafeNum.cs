using System;
using Avalonia.Media;

namespace SimplePaint.Models.Safes
{
    public class SafeNum : ForcePropertyChange, ISafe
    {
        private int _num;
        private bool _valid = true;
        private readonly Action<object?>? _hook;
        private readonly object? _inst;
        public SafeNum(int num, Action<object?>? hook = null, object? inst = null)
        {
            this._num = num; this._hook = hook; this._inst = inst;
        }
        public SafeNum(string init, Action<object?>? hook = null, object? inst = null)
        {
            this._hook = hook; this._inst = inst;
            Set(init);
            if (!_valid) throw new FormatException("Недействующий формат инициализации SafeNum: " + init);
        }
        public int Num => _num;
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
        public void Set(int n)
        {
            _num = n;
            _valid = true;
        }
        public bool Valid => _valid;
        public void Set(string str)
        {
            int a;
            try
            {
                a = int.Parse(str);
            }
            catch { Upd_valid(false); return; }
            if (Math.Abs(a) > 10000) { Upd_valid(false); return; }
            _num = a;
            Upd_valid(true);
        }
        public string Value
        {
            get { Re_check(); return _num.ToString(); }
            set
            {
                Set(value);
                UpdProperty(nameof(Color));
            }
        }
        public IBrush Color { get => _valid ? Brushes.Lime : Brushes.Pink; }
    }
}
