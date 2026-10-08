namespace ReminderProX.Views;

public partial class HomePage : ContentPage
{
    private readonly HomePageViewModel _viewModel;

    public HomePage(HomePageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // ஏற்கனவே கார்டுகள் லோட் ஆகியிருந்தால் மீண்டும் அனாவசியமாக லோட் செய்யாமல் தடுக்கும்
        if (_viewModel.AllReminders.Count == 0)
        {
            await _viewModel.LoadRemindersAsync();
        }
    }

    private async void OnAddButtonClicked(object? sender, EventArgs e)
    {
        if (sender is ImageButton btn)
        {
            await btn.RotateToAsync(45, 120, Easing.CubicOut);
            if (_viewModel.OpenAddOptionsCommand.CanExecute(null))
            {
                _viewModel.OpenAddOptionsCommand.Execute(null);
            }
            await btn.RotateToAsync(0, 120, Easing.CubicIn);
        }
    }
}