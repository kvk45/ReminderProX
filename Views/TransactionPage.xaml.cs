namespace ReminderProX.Views;

public partial class TransactionPage : ContentPage
{
    private readonly TransactionViewModel _viewModel;

    public TransactionPage(TransactionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadTransactionsAsync();
    }
}