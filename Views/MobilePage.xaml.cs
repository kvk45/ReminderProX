namespace ReminderProX.Views;

public partial class MobilePage : ContentPage
{
    public MobilePage(MobilePageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}