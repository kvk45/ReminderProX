namespace ReminderProX.ViewModels;

public partial class LedgerViewModel : BaseViewModel
{
    private readonly ILedgerService _ledgerService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;
    private readonly List<LedgerItemModel> _allLedgers = [];

    private const int PageSize = 10;
    private int _skipCount = 0;
    private bool _hasMoreItems = true;
    private bool _isLoadingMore = false;

    [ObservableProperty]
    public partial ObservableCollection<LedgerItemModel> Ledgers { get; set; } = [];

    [ObservableProperty]
    public partial string NewUserName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string SearchQuery { get; set; } = string.Empty;

    [ObservableProperty]
    public partial decimal TotalPaid { get; set; }

    [ObservableProperty]
    public partial decimal TotalReceived { get; set; }

    [ObservableProperty]
    public partial bool IsRefreshing { get; set; }

    public LedgerViewModel(
        ILedgerService ledgerService,
        IDialogService dialogService,
        INavigationService navigationService)
    {
        _ledgerService = ledgerService;
        _dialogService = dialogService;
        _navigationService = navigationService;
        Title = "Ledger";

        WeakReferenceMessenger.Default.Register<DataRestoredMessage>(this, async (r, m) =>
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await LoadLedgersAsync();
            });
        });
    }

    async partial void OnSearchQueryChanged(string value) => await FilterLedgersAsync();

    [RelayCommand]
    public async Task LoadLedgersAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            _skipCount = 0;
            _hasMoreItems = true;
            _allLedgers.Clear();
            Ledgers.Clear();

            var items = await _ledgerService.GetPagedLedgersAsync(_skipCount, PageSize);
            if (items != null && items.Count > 0)
            {
                _allLedgers.AddRange(items);
                _skipCount += items.Count;
                if (items.Count < PageSize)
                {
                    _hasMoreItems = false;
                }
            }
            else
            {
                _hasMoreItems = false;
            }

            await FilterLedgersAsync();
            await CalculateTotalsAsync();
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAlertAsync("Error", ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task LoadMoreLedgersAsync()
    {
        if (_isLoadingMore || !_hasMoreItems || !string.IsNullOrWhiteSpace(SearchQuery)) return;

        _isLoadingMore = true;
        try
        {
            var items = await _ledgerService.GetPagedLedgersAsync(_skipCount, PageSize);
            if (items != null && items.Count > 0)
            {
                _allLedgers.AddRange(items);
                foreach (var item in items)
                {
                    Ledgers.Add(item);
                }
                _skipCount += items.Count;
                if (items.Count < PageSize)
                {
                    _hasMoreItems = false;
                }
            }
            else
            {
                _hasMoreItems = false;
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAlertAsync("Error", ex.Message);
        }
        finally
        {
            _isLoadingMore = false;
        }
    }

    [RelayCommand]
    public async Task RefreshAsync()
    {
        IsRefreshing = true;
        SearchQuery = string.Empty;
        try
        {
            await LoadLedgersAsync();
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAlertAsync("Error", ex.Message);
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task SearchAsync() => await FilterLedgersAsync();

    private async Task FilterLedgersAsync()
    {
        Ledgers.Clear();

        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            foreach (var item in _allLedgers)
            {
                Ledgers.Add(item);
            }
        }
        else
        {
            var results = await _ledgerService.SearchLedgersAsync(SearchQuery.Trim());
            foreach (var item in results)
            {
                Ledgers.Add(item);
            }
        }
    }

    private async Task CalculateTotalsAsync()
    {
        var allData = await _ledgerService.GetAllLedgersAsync();
        TotalReceived = allData.Where(x => x.Balance > 0).Sum(x => x.Balance);
        TotalPaid = allData.Where(x => x.Balance < 0).Sum(x => Math.Abs(x.Balance));
    }

    [RelayCommand]
    private async Task AddLedgerAsync()
    {
        if (string.IsNullOrWhiteSpace(NewUserName))
        {
            await _dialogService.ShowAlertAsync("Validation", "Please enter a User Name.");
            return;
        }

        try
        {
            var newLedger = new LedgerItemModel
            {
                UserName = NewUserName.Trim(),
                CreatedDateTime = DateTime.Now,
                Balance = 0m
            };

            await _ledgerService.SaveLedgerAsync(newLedger);
            NewUserName = string.Empty;
            await LoadLedgersAsync();
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAlertAsync("Error", ex.Message);
        }
    }

    [RelayCommand]
    private async Task DeleteLedgerAsync(LedgerItemModel item)
    {
        if (item == null) return;

        bool answer = await _dialogService.ShowConfirmationAsync(
            "Confirm Delete",
            $"Are you sure you want to delete {item.UserName} from Ledger?");

        if (answer)
        {
            await _ledgerService.DeleteLedgerWithTransactionsAsync(item.Id);
            await LoadLedgersAsync();
        }
    }

    [RelayCommand]
    private async Task OpenTransactionAsync(LedgerItemModel item)
    {
        if (item == null) return;

        var navigationParameters = new Dictionary<string, object>
        {
            { "LedgerItem", item }
        };

        await _navigationService.NavigateToAsync("TransactionPage", navigationParameters);
    }
}