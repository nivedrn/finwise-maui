using FinWise.Data;
using FinWise.Data.Repositories;
using SQLite;

namespace FinWise.Tests;

public class DatabaseTests : DatabaseTest
{
    [Fact]
    public async Task First_use_creates_the_schema_and_sets_the_version()
    {
        Assert.False(File.Exists(DatabasePath) && new FileInfo(DatabasePath).Length > 0);

        Assert.Equal(FinWiseDatabase.LatestSchemaVersion, await Database.GetSchemaVersionAsync());

        var connection = await Database.GetConnectionAsync();
        var tables = await connection.QueryScalarsAsync<string>(
            "SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY name");
        Assert.Equal(["categories", "people", "settlements", "splits", "transactions"], tables);
    }

    [Fact]
    public async Task Default_categories_are_seeded_once()
    {
        var categories = new CategoryRepository(Database);

        var seeded = await categories.GetAllAsync();
        Assert.Equal(DefaultCategories.All.Select(c => c.Name), seeded.Select(c => c.Name));
        Assert.All(seeded, c => Assert.True(c.IsDefault));
        Assert.All(seeded, c => Assert.Equal(Clock.Now, c.CreatedAt));

        // Deleting a default category must not bring it back on the next start.
        Assert.True(await categories.DeleteAsync(seeded[0].Id));
        await ReopenAsync();

        var afterRestart = await new CategoryRepository(Database).GetAllAsync(includeDeleted: true);
        Assert.Equal(DefaultCategories.All.Count, afterRestart.Count);
        Assert.Single(afterRestart, c => c.IsDeleted);
    }

    [Fact]
    public async Task Default_category_ids_are_stable()
    {
        var categories = await new CategoryRepository(Database).GetAllAsync();

        Assert.Equal(DefaultCategories.All.Select(c => c.Id), categories.Select(c => c.Id));
    }

    [Fact]
    public async Task Data_survives_a_restart()
    {
        await new PersonRepository(Database).SaveAsync(new() { Name = "Alice" });
        await ReopenAsync();

        var people = await new PersonRepository(Database).GetAllAsync();
        Assert.Equal("Alice", Assert.Single(people).Name);
        Assert.Equal(FinWiseDatabase.LatestSchemaVersion, await Database.GetSchemaVersionAsync());
    }

    [Fact]
    public async Task A_database_from_a_newer_app_version_is_refused()
    {
        var connection = await Database.GetConnectionAsync();
        await connection.ExecuteAsync($"PRAGMA user_version = {FinWiseDatabase.LatestSchemaVersion + 1}");
        await ReopenAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(() => Database.GetConnectionAsync());
    }

    [Fact]
    public async Task Concurrent_first_use_initialises_once()
    {
        var versions = await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => Database.GetSchemaVersionAsync()));

        Assert.All(versions, v => Assert.Equal(FinWiseDatabase.LatestSchemaVersion, v));
        Assert.Equal(DefaultCategories.All.Count, (await new CategoryRepository(Database).GetAllAsync()).Count);
    }

    [Fact]
    public async Task Money_is_stored_as_exact_integers()
    {
        var people = new PersonRepository(Database);
        var alice = new FinWise.Core.Models.Person { Name = "Alice" };
        await people.SaveAsync(alice);
        var transaction = new FinWise.Core.Models.Transaction
        {
            Title = "Big", Amount = 12345678901234.56m, CurrencyCode = "EUR", PayerId = alice.Id,
        };
        await new TransactionRepository(Database).SaveAsync(transaction);

        var connection = await Database.GetConnectionAsync();
        var stored = await connection.ExecuteScalarAsync<long>("SELECT AmountE4 FROM transactions");
        Assert.Equal(123456789012345600L, stored);
        Assert.Equal("integer", await connection.ExecuteScalarAsync<string>("SELECT typeof(AmountE4) FROM transactions"));
    }
}
