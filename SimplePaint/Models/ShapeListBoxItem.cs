using System.ComponentModel;
using ReactiveUI;
using System.Reactive;
using System.Runtime.CompilerServices;

namespace SimplePaint.Models
{
    public class ShapeListBoxItem
    {
        public ShapeListBoxItem(string name, Mapper map)
        {
            Name = name;
            Remove = ReactiveCommand.Create<Unit, Unit>(_ => { map.Remove(this); return new Unit(); });
        }
        public string Name { get; }
        public ReactiveCommand<Unit, Unit> Remove { get; }
    }
    public class ForcePropertyChange : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void UpdProperty([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
