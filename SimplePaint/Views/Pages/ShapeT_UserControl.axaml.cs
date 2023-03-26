using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SimplePaint.Views.Pages;

public partial class ShapeT_UserControl : UserControl
{
    public ShapeT_UserControl()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}