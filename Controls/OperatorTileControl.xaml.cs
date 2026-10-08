namespace ReminderProX.Controls;

public partial class OperatorTileControl : ContentView
{
    public static readonly BindableProperty TitleTextProperty =
        BindableProperty.Create(nameof(TitleText), typeof(string), typeof(OperatorTileControl), string.Empty);

    public static readonly BindableProperty ImageSourceProperty =
        BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(OperatorTileControl), null);

    public static readonly BindableProperty IsSelectedProperty =
        BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(OperatorTileControl), false, propertyChanged: OnSelectionChanged);

    public static readonly BindableProperty SelectCommandProperty =
        BindableProperty.Create(nameof(SelectCommand), typeof(ICommand), typeof(OperatorTileControl), null);

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(OperatorTileControl), null);

    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    public ImageSource? ImageSource
    {
        get => (ImageSource?)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public ICommand? SelectCommand
    {
        get => (ICommand?)GetValue(SelectCommandProperty);
        set => SetValue(SelectCommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public Color BorderColor => IsSelected ? Color.FromArgb("#2196F3") : Colors.Transparent;
    public double TileOpacity => IsSelected ? 1.0 : 0.4;

    private static void OnSelectionChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is OperatorTileControl control)
        {
            control.OnPropertyChanged(nameof(BorderColor));
            control.OnPropertyChanged(nameof(TileOpacity));

            if (newValue is bool isSelected && isSelected)
            {
                control.AnimateSelection();
            }
        }
    }

    private async void AnimateSelection()
    {
        await this.ScaleToAsync(1.08, 100, Easing.CubicOut);
        await this.ScaleToAsync(1.0, 100, Easing.CubicIn);
    }

    public OperatorTileControl()
    {
        InitializeComponent();
    }
}