using FinWise.Data;

namespace FinWise.Tests;

/// <summary>A clock the tests can move forward.</summary>
public sealed class ManualTimeProvider(DateTimeOffset start) : TimeProvider
{
    public DateTimeOffset Now { get; set; } = start;

    public override DateTimeOffset GetUtcNow() => Now;

    public void Advance(TimeSpan by) => Now += by;
}

/// <summary>
/// Gives each test its own SQLite file in the temp directory and deletes it afterwards.
/// </summary>
public abstract class DatabaseTest : IAsyncLifetime
{
    protected DatabaseTest()
    {
        DatabasePath = Path.Combine(Path.GetTempPath(), $"finwise-test-{Guid.NewGuid():N}.db3");
        Clock = new ManualTimeProvider(new DateTimeOffset(2026, 11, 18, 9, 0, 0, TimeSpan.Zero));
        Database = new FinWiseDatabase(DatabasePath, Clock);
    }

    protected string DatabasePath { get; }

    protected ManualTimeProvider Clock { get; }

    protected FinWiseDatabase Database { get; private set; }

    /// <summary>Closes the connection and opens the same file again, as on an app restart.</summary>
    protected async Task ReopenAsync()
    {
        await Database.DisposeAsync();
        Database = new FinWiseDatabase(DatabasePath, Clock);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await Database.DisposeAsync();
        foreach (var file in new[] { DatabasePath, DatabasePath + "-journal", DatabasePath + "-wal", DatabasePath + "-shm" })
        {
            if (File.Exists(file))
                File.Delete(file);
        }
    }
}
