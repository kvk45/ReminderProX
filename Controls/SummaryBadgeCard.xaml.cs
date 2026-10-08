namespace ReminderProX.Controls;

public partial class SummaryBadgeCard : ContentView
{
    public static readonly BindableProperty TitleTextProperty =
        BindableProperty.Create(nameof(TitleText), typeof(string), typeof(SummaryBadgeCard), string.Empty);

    public static readonly BindableProperty AmountTextProperty =
        BindableProperty.Create(nameof(AmountText), typeof(string), typeof(SummaryBadgeCard), "₹0");

    public static readonly BindableProperty CardBackgroundColorProperty =
        BindableProperty.Create(nameof(CardBackgroundColor), typeof(Color), typeof(SummaryBadgeCard), Colors.Transparent);

    public static readonly BindableProperty CardBorderColorProperty =
        BindableProperty.Create(nameof(CardBorderColor), typeof(Color), typeof(SummaryBadgeCard), Colors.Transparent);

    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(SummaryBadgeCard), Colors.Black);

    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    public string AmountText
    {
        get => (string)GetValue(AmountTextProperty);
        set => SetValue(AmountTextProperty, value);
    }

    public Color CardBackgroundColor
    {
        get => (Color)GetValue(CardBackgroundColorProperty);
        set => SetValue(CardBackgroundColorProperty, value);
    }

    public Color CardBorderColor
    {
        get => (Color)GetValue(CardBorderColorProperty);
        set => SetValue(CardBorderColorProperty, value);
    }

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public SummaryBadgeCard()
    {
        InitializeComponent();
    }
}