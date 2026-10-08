namespace ReminderProX.Controls;

public partial class RechargeCardView : ContentView
{
    public static readonly BindableProperty ItemProperty =
        BindableProperty.Create(nameof(Item), typeof(ReminderCardModel), typeof(RechargeCardView), null);

    public static readonly BindableProperty EditCommandProperty =
        BindableProperty.Create(nameof(EditCommand), typeof(ICommand), typeof(RechargeCardView), null);

    public static readonly BindableProperty DeleteCommandProperty =
        BindableProperty.Create(nameof(DeleteCommand), typeof(ICommand), typeof(RechargeCardView), null);

    public static readonly BindableProperty CallCommandProperty =
        BindableProperty.Create(nameof(CallCommand), typeof(ICommand), typeof(RechargeCardView), null);

    public static readonly BindableProperty CopyCommandProperty =
        BindableProperty.Create(nameof(CopyCommand), typeof(ICommand), typeof(RechargeCardView), null);

    public static readonly BindableProperty WhatsAppCommandProperty =
        BindableProperty.Create(nameof(WhatsAppCommand), typeof(ICommand), typeof(RechargeCardView), null);

    public ReminderCardModel? Item
    {
        get => (ReminderCardModel?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public ICommand? EditCommand
    {
        get => (ICommand?)GetValue(EditCommandProperty);
        set => SetValue(EditCommandProperty, value);
    }

    public ICommand? DeleteCommand
    {
        get => (ICommand?)GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }

    public ICommand? CallCommand
    {
        get => (ICommand?)GetValue(CallCommandProperty);
        set => SetValue(CallCommandProperty, value);
    }

    public ICommand? CopyCommand
    {
        get => (ICommand?)GetValue(CopyCommandProperty);
        set => SetValue(CopyCommandProperty, value);
    }

    public ICommand? WhatsAppCommand
    {
        get => (ICommand?)GetValue(WhatsAppCommandProperty);
        set => SetValue(WhatsAppCommandProperty, value);
    }

    public RechargeCardView()
    {
        InitializeComponent();
    }
}