namespace ReminderProX.Data;

public class DatabaseContext
{
    private SQLiteAsyncConnection? _connection;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public const SQLiteOpenFlags Flags =
        SQLiteOpenFlags.ReadWrite |
        SQLiteOpenFlags.Create |
        SQLiteOpenFlags.SharedCache;

    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_connection is not null) return _connection;

        await _semaphore.WaitAsync();
        try
        {
            if (_connection is null)
            {
                var targetDir = FileSystem.AppDataDirectory;
                if (!System.IO.Directory.Exists(targetDir))
                {
                    System.IO.Directory.CreateDirectory(targetDir);
                }

                var databasePath = System.IO.Path.Combine(targetDir, "ReminderProX.db3");
                var connectionString = new SQLiteConnectionString(databasePath, Flags, storeDateTimeAsTicks: true);
                var connection = new SQLiteAsyncConnection(connectionString);

                await connection.EnableWriteAheadLoggingAsync();

                await connection.CreateTableAsync<ReminderCardModel>();
                await connection.CreateTableAsync<LedgerItemModel>();
                await connection.CreateTableAsync<TransactionModel>();

                _connection = connection;
            }
        }
        finally
        {
            _semaphore.Release();
        }

        return _connection;
    }
}