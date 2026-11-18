using SQLite;

namespace FinWise.Data;

/// <summary>
/// Owns the SQLite connection. The schema is created or upgraded the first time the connection is
/// used. Register one instance per database file (a singleton in the app).
/// </summary>
public sealed class FinWiseDatabase : IAsyncDisposable
{
    private const SQLiteOpenFlags OpenFlags =
        SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex;

    private readonly SQLiteAsyncConnection _connection;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _initialized;

    public FinWiseDatabase(string databasePath, TimeProvider? timeProvider = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);
        DatabasePath = databasePath;
        Clock = timeProvider ?? TimeProvider.System;
        _connection = new SQLiteAsyncConnection(databasePath, OpenFlags, storeDateTimeAsTicks: true);
    }

    public string DatabasePath { get; }

    /// <summary>The clock used for CreatedAt/UpdatedAt.</summary>
    public TimeProvider Clock { get; }

    /// <summary>The schema version this build of the app creates.</summary>
    public static int LatestSchemaVersion => SchemaMigrations.LatestVersion;

    /// <summary>Returns the connection, creating or upgrading the schema first if needed.</summary>
    public async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_initialized)
            return _connection;

        await _initLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (!_initialized)
            {
                var now = Clock.GetUtcNow();
                await _connection.RunInTransactionAsync(c => SchemaMigrations.Migrate(c, now)).ConfigureAwait(false);
                _initialized = true;
            }
        }
        finally
        {
            _initLock.Release();
        }

        return _connection;
    }

    /// <summary>The schema version currently stored in the database file.</summary>
    public async Task<int> GetSchemaVersionAsync()
    {
        var connection = await GetConnectionAsync().ConfigureAwait(false);
        return await connection.ExecuteScalarAsync<int>("PRAGMA user_version").ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.CloseAsync().ConfigureAwait(false);
        _initLock.Dispose();
    }
}
