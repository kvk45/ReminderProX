namespace ReminderProX.Views;

public partial class DTHPage : ContentPage
{
    public DTHPage(DTHPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}