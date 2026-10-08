namespace ReminderProX.Controls;

public partial class TransactionCardView : ContentView
{
    public static readonly BindableProperty ItemProperty =
        BindableProperty.Create(nameof(Item), typeof(TransactionModel), typeof(TransactionCardView), null);

    public static readonly BindableProperty DeleteCommandProperty =
        BindableProperty.Create(nameof(DeleteCommand), typeof(ICommand), typeof(TransactionCardView), null);

    public TransactionModel? Item
    {
        get => (TransactionModel?)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public ICommand? DeleteCommand
    {
        get => (ICommand?)GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }

    public TransactionCardView()
    {
        InitializeComponent();
    }
}