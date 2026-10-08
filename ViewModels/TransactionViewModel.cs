namespace ReminderProX.ViewModels;

[QueryProperty(nameof(CurrentLedger), "LedgerItem")]
public partial class TransactionViewModel : BaseViewModel
{
    private readonly ILedgerService _ledgerService;
    private readonly ITransactionService _transactionService;
    private readonly IDialogService _dialogService;

    private const int PageSize = 10;
    private int _skipCount = 0;
    private bool _hasMoreItems = true;
    private bool _isLoadingMore = false;

    [ObservableProperty]
    public partial LedgerItemModel? CurrentLedger { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<TransactionModel> Transactions { get; set; } = [];

    [ObservableProperty]
    public partial string Amount { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Note { get; set; } = string.Empty;

    [ObservableProperty]
    public partial decimal CurrentBalance { get; set; }

    public TransactionViewModel(
        ILedgerService ledgerService,
        ITransactionService transactionService,
        IDialogService dialogService)
    {
        _ledgerService = ledgerService;
        _transactionService = transactionService;
        _dialogService = dialogService;
        Title = "Transactions";

        WeakReferenceMessenger.Default.Register<DataRestoredMessage>(this, async (r, m) =>
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (CurrentLedger != null)
                {
                    await LoadTransactionsAsync();
                }
            });
        });
    }

    async partial void OnCurrentLedgerChanged(LedgerItemModel? value)
    {
        if (value != null)
        {
            Title = $"{value.UserName}'s Transactions";
            await LoadTransactionsAsync();
        }
    }

    [RelayCommand]
    public async Task LoadTransactionsAsync()
    {
        if (CurrentLedger == null || IsBusy) return;
        IsBusy = true;

        try
        {
            _skipCount = 0;
            _hasMoreItems = true;
            Transactions.Clear();

            var userTxns = await _transactionService.GetPagedTransactionsByLedgerIdAsync(CurrentLedger.Id, _skipCount, PageSize);
            if (userTxns != null && userTxns.Count > 0)
            {
                foreach (var t in userTxns)
                {
                    Transactions.Add(t);
                }
                _skipCount += userTxns.Count;
                if (userTxns.Count < PageSize)
                {
                    _hasMoreItems = false;
                }
            }
            else
            {
                _hasMoreItems = false;
            }

            await UpdateBalanceSummaryAsync();
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
    public async Task LoadMoreTransactionsAsync()
    {
        if (CurrentLedger == null || _isLoadingMore || !_hasMoreItems) return;

        _isLoadingMore = true;
        try
        {
            var userTxns = await _transactionService.GetPagedTransactionsByLedgerIdAsync(CurrentLedger.Id, _skipCount, PageSize);
            if (userTxns != null && userTxns.Count > 0)
            {
                foreach (var t in userTxns)
                {
                    Transactions.Add(t);
                }
                _skipCount += userTxns.Count;
                if (userTxns.Count < PageSize)
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

    private async Task UpdateBalanceSummaryAsync()
    {
        if (CurrentLedger == null) return;

        var allTxns = await _transactionService.GetTransactionsByLedgerIdAsync(CurrentLedger.Id);
        decimal totalDebit = allTxns.Where(x => x.TransactionType.Equals("Debit", StringComparison.OrdinalIgnoreCase)).Sum(x => x.Amount);
        decimal totalCredit = allTxns.Where(x => x.TransactionType.Equals("Credit", StringComparison.OrdinalIgnoreCase)).Sum(x => x.Amount);
        CurrentBalance = totalDebit - totalCredit;

        if (CurrentLedger.Balance != CurrentBalance)
        {
            CurrentLedger.Balance = CurrentBalance;
            await _ledgerService.SaveLedgerAsync(CurrentLedger);
        }
    }

    [RelayCommand]
    private async Task AddDebitAsync() => await AddTransactionInternalAsync("Debit");

    [RelayCommand]
    private async Task AddCreditAsync() => await AddTransactionInternalAsync("Credit");

    private async Task AddTransactionInternalAsync(string type)
    {
        if (CurrentLedger == null) return;

        if (!decimal.TryParse(Amount, out decimal amountVal) || amountVal <= 0)
        {
            await _dialogService.ShowAlertAsync("Validation", "Please enter a valid amount.");
            return;
        }

        try
        {
            var txn = new TransactionModel
            {
                Id = 0,
                LedgerId = CurrentLedger.Id,
                Amount = amountVal,
                PreviousBalance = CurrentBalance,
                TransactionType = type,
                Note = Note?.Trim() ?? string.Empty,
                CreatedDateTime = DateTime.Now
            };

            await _transactionService.SaveTransactionAsync(txn);
            Amount = string.Empty;
            Note = string.Empty;
            await LoadTransactionsAsync();
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAlertAsync("Error Saving Transaction", ex.Message);
        }
    }

    [RelayCommand]
    private async Task DeleteTransactionAsync(TransactionModel item)
    {
        if (item == null) return;

        bool confirm = await _dialogService.ShowConfirmationAsync(
            "Confirm Delete",
            "Are you sure you want to delete this transaction?");

        if (confirm)
        {
            await _transactionService.DeleteTransactionAsync(item);
            await LoadTransactionsAsync();
        }
    }
}