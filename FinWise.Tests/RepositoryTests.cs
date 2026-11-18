using FinWise.Core.Calculations;
using FinWise.Core.Models;
using FinWise.Core.Validation;
using FinWise.Data.Repositories;

namespace FinWise.Tests;

public class RepositoryTests : DatabaseTest
{
    private readonly PersonRepository _people;
    private readonly TransactionRepository _transactions;
    private readonly SettlementRepository _settlements;
    private readonly CategoryRepository _categories;

    public RepositoryTests()
    {
        _people = new PersonRepository(Database);
        _transactions = new TransactionRepository(Database);
        _settlements = new SettlementRepository(Database);
        _categories = new CategoryRepository(Database);
    }

    private async Task<Person> AddPersonAsync(string name, bool isCurrentUser = false)
    {
        var person = new Person { Name = name, IsCurrentUser = isCurrentUser };
        await _people.SaveAsync(person);
        return person;
    }

    private static Transaction NewExpense(Guid payer, decimal amount, params Guid[] participants)
    {
        var t = new Transaction
        {
            Title = "Dinner",
            Amount = amount,
            CurrencyCode = "EUR",
            PayerId = payer,
            Date = new DateTime(2026, 11, 10, 19, 30, 0),
        };
        if (participants.Length > 0)
            t.SetSplits(SplitMethod.Equal, SplitCalculator.Equal(amount, participants));
        return t;
    }

    [Fact]
    public async Task Person_round_trip()
    {
        var person = new Person { Name = "  Alice ", Email = "alice@example.com", PhoneNumber = " " };
        await _people.SaveAsync(person);

        var loaded = await _people.GetAsync(person.Id);

        Assert.NotNull(loaded);
        Assert.Equal("Alice", loaded.Name);
        Assert.Equal("alice@example.com", loaded.Email);
        Assert.Null(loaded.PhoneNumber);
        Assert.Equal(Clock.Now, loaded.CreatedAt);
        Assert.Equal(Clock.Now, loaded.UpdatedAt);
        Assert.False(loaded.IsDeleted);
    }

    [Fact]
    public async Task Updating_keeps_CreatedAt_and_moves_UpdatedAt()
    {
        var person = await AddPersonAsync("Alice");
        var created = Clock.Now;

        Clock.Advance(TimeSpan.FromHours(1));
        person.Name = "Alice B.";
        person.CreatedAt = default; // a stale or careless caller must not reset it
        await _people.SaveAsync(person);

        var loaded = await _people.GetAsync(person.Id);
        Assert.Equal("Alice B.", loaded!.Name);
        Assert.Equal(created, loaded.CreatedAt);
        Assert.Equal(created.AddHours(1), loaded.UpdatedAt);
        Assert.Single(await _people.GetAllAsync());
    }

    [Fact]
    public async Task Soft_delete_hides_but_keeps_the_record()
    {
        var person = await AddPersonAsync("Alice");
        Clock.Advance(TimeSpan.FromMinutes(5));

        Assert.True(await _people.DeleteAsync(person.Id));
        Assert.False(await _people.DeleteAsync(person.Id));
        Assert.False(await _people.DeleteAsync(Guid.NewGuid()));

        Assert.Null(await _people.GetAsync(person.Id));
        Assert.Empty(await _people.GetAllAsync());

        var tombstone = Assert.Single(await _people.GetAllAsync(includeDeleted: true));
        Assert.True(tombstone.IsDeleted);
        Assert.Equal(Clock.Now, tombstone.UpdatedAt);
    }

    [Fact]
    public async Task Invalid_entities_are_not_saved()
    {
        await Assert.ThrowsAsync<ValidationException>(() => _people.SaveAsync(new Person { Name = " " }));

        var alice = await AddPersonAsync("Alice");
        var bad = NewExpense(alice.Id, 10m, alice.Id);
        bad.Splits[0].Amount = 9m;
        await Assert.ThrowsAsync<ValidationException>(() => _transactions.SaveAsync(bad));

        Assert.Empty(await _transactions.GetAllAsync(includeDeleted: true));
    }

    [Fact]
    public async Task Only_one_person_is_the_current_user()
    {
        Assert.Null(await _people.GetCurrentUserAsync());

        var alice = await AddPersonAsync("Alice", isCurrentUser: true);
        Assert.Equal(alice.Id, (await _people.GetCurrentUserAsync())?.Id);

        var bob = await AddPersonAsync("Bob", isCurrentUser: true);
        Assert.Equal(bob.Id, (await _people.GetCurrentUserAsync())?.Id);
        Assert.Single(await _people.GetAllAsync(), p => p.IsCurrentUser);

        // The current user is listed first.
        await AddPersonAsync("Aaron");
        Assert.Equal(["Bob", "Aaron", "Alice"], (await _people.GetAllAsync()).Select(p => p.Name));
    }

    [Fact]
    public async Task Transaction_round_trip_with_splits()
    {
        var alice = await AddPersonAsync("Alice");
        var bob = await AddPersonAsync("Bob");
        var category = (await _categories.GetAllAsync())[0];

        var transaction = new Transaction
        {
            Title = " Dinner ",
            Note = "Pizza",
            Amount = 10m,
            CurrencyCode = "eur",
            PayerId = alice.Id,
            CategoryId = category.Id,
            Date = new DateTime(2026, 11, 10, 19, 30, 0),
        };
        transaction.SetSplits(SplitMethod.Shares, SplitCalculator.ByShares(10m, [(alice.Id, 1m), (bob.Id, 2.5m)]));
        await _transactions.SaveAsync(transaction);

        var loaded = await _transactions.GetAsync(transaction.Id);

        Assert.NotNull(loaded);
        Assert.Equal("Dinner", loaded.Title);
        Assert.Equal("Pizza", loaded.Note);
        Assert.Equal(10m, loaded.Amount);
        Assert.Equal("EUR", loaded.CurrencyCode);
        Assert.Equal(alice.Id, loaded.PayerId);
        Assert.Equal(category.Id, loaded.CategoryId);
        Assert.Equal(new DateTime(2026, 11, 10, 19, 30, 0), loaded.Date);
        Assert.Equal(SplitMethod.Shares, loaded.SplitMethod);

        Assert.Equal(2, loaded.Splits.Count);
        var aliceSplit = loaded.Splits.Single(s => s.PersonId == alice.Id);
        var bobSplit = loaded.Splits.Single(s => s.PersonId == bob.Id);
        Assert.Equal(2.86m, aliceSplit.Amount);
        Assert.Equal(7.14m, bobSplit.Amount);
        Assert.Equal(1m, aliceSplit.Shares);
        Assert.Equal(2.5m, bobSplit.Shares);
        Assert.All(loaded.Splits, s => Assert.Equal(transaction.Id, s.TransactionId));
        Assert.All(loaded.Splits, s => Assert.Equal(Clock.Now, s.CreatedAt));
    }

    [Fact]
    public async Task Transaction_without_category_or_splits()
    {
        var alice = await AddPersonAsync("Alice");
        var transaction = NewExpense(alice.Id, 4.5m);
        await _transactions.SaveAsync(transaction);

        var loaded = await _transactions.GetAsync(transaction.Id);

        Assert.Null(loaded!.CategoryId);
        Assert.Equal(SplitMethod.None, loaded.SplitMethod);
        Assert.Empty(loaded.Splits);
    }

    [Fact]
    public async Task Editing_splits_updates_in_place_and_soft_deletes_removed_people()
    {
        var alice = await AddPersonAsync("Alice");
        var bob = await AddPersonAsync("Bob");
        var carol = await AddPersonAsync("Carol");

        var transaction = NewExpense(alice.Id, 30m, alice.Id, bob.Id, carol.Id);
        await _transactions.SaveAsync(transaction);
        var original = (await _transactions.GetAsync(transaction.Id))!.Splits.ToDictionary(s => s.PersonId);

        // Drop Carol and change the amount, building brand-new split objects as a form would.
        Clock.Advance(TimeSpan.FromMinutes(10));
        transaction.Amount = 50m;
        transaction.SetSplits(SplitMethod.Equal, SplitCalculator.Equal(50m, [alice.Id, bob.Id]));
        await _transactions.SaveAsync(transaction);

        var loaded = (await _transactions.GetAsync(transaction.Id))!;
        Assert.Equal([25m, 25m], loaded.Splits.Select(s => s.Amount));
        foreach (var split in loaded.Splits)
        {
            Assert.Equal(original[split.PersonId].Id, split.Id);
            Assert.Equal(original[split.PersonId].CreatedAt, split.CreatedAt);
            Assert.Equal(Clock.Now, split.UpdatedAt);
        }

        var connection = await Database.GetConnectionAsync();
        var carolRow = await connection.Table<FinWise.Data.Rows.SplitRow>()
            .Where(s => s.PersonId == carol.Id).FirstAsync();
        Assert.True(carolRow.IsDeleted);
        Assert.Equal(original[carol.Id].Id, carolRow.Id);

        // Adding Carol back reuses her old row instead of creating another one.
        transaction.SetSplits(SplitMethod.Equal, SplitCalculator.Equal(50m, [alice.Id, bob.Id, carol.Id]));
        await _transactions.SaveAsync(transaction);
        Assert.Equal(3, await connection.Table<FinWise.Data.Rows.SplitRow>().CountAsync());
        Assert.Equal(3, (await _transactions.GetAsync(transaction.Id))!.Splits.Count);
    }

    [Fact]
    public async Task Making_a_transaction_personal_removes_its_splits()
    {
        var alice = await AddPersonAsync("Alice");
        var bob = await AddPersonAsync("Bob");
        var transaction = NewExpense(alice.Id, 30m, alice.Id, bob.Id);
        await _transactions.SaveAsync(transaction);

        transaction.ClearSplits();
        await _transactions.SaveAsync(transaction);

        Assert.Empty((await _transactions.GetAsync(transaction.Id))!.Splits);
        Assert.Empty(await _transactions.GetForPersonAsync(bob.Id));
    }

    [Fact]
    public async Task Deleting_a_transaction_deletes_its_splits()
    {
        var alice = await AddPersonAsync("Alice");
        var bob = await AddPersonAsync("Bob");
        var transaction = NewExpense(alice.Id, 30m, alice.Id, bob.Id);
        await _transactions.SaveAsync(transaction);

        Assert.True(await _transactions.DeleteAsync(transaction.Id));

        Assert.Null(await _transactions.GetAsync(transaction.Id));
        Assert.Empty(await _transactions.GetForPersonAsync(bob.Id));
        var connection = await Database.GetConnectionAsync();
        Assert.Equal(0, await connection.Table<FinWise.Data.Rows.SplitRow>().Where(s => s.IsDeleted == false).CountAsync());
        Assert.Equal(2, await connection.Table<FinWise.Data.Rows.SplitRow>().CountAsync());
    }

    [Fact]
    public async Task Transactions_for_a_person_include_paid_and_shared_ones()
    {
        var alice = await AddPersonAsync("Alice");
        var bob = await AddPersonAsync("Bob");
        var carol = await AddPersonAsync("Carol");

        var paidByBob = NewExpense(bob.Id, 10m, alice.Id);
        var sharedWithBob = NewExpense(alice.Id, 20m, alice.Id, bob.Id);
        var unrelated = NewExpense(alice.Id, 30m, alice.Id, carol.Id);
        paidByBob.Date = new DateTime(2026, 11, 1);
        sharedWithBob.Date = new DateTime(2026, 11, 5);
        foreach (var t in new[] { paidByBob, sharedWithBob, unrelated })
            await _transactions.SaveAsync(t);

        var forBob = await _transactions.GetForPersonAsync(bob.Id);

        Assert.Equal([sharedWithBob.Id, paidByBob.Id], forBob.Select(t => t.Id)); // newest first
        Assert.All(forBob, t => Assert.NotEmpty(t.Splits));
    }

    [Fact]
    public async Task Transactions_between_dates()
    {
        var alice = await AddPersonAsync("Alice");
        foreach (var day in new[] { 1, 15, 30 })
        {
            var t = NewExpense(alice.Id, day);
            t.Date = new DateTime(2026, 11, day, 12, 0, 0);
            await _transactions.SaveAsync(t);
        }

        var result = await _transactions.GetBetweenAsync(new DateTime(2026, 11, 1), new DateTime(2026, 11, 30));

        Assert.Equal([15m, 1m], result.Select(t => t.Amount));
    }

    [Fact]
    public async Task Settlement_round_trip_and_queries()
    {
        var alice = await AddPersonAsync("Alice");
        var bob = await AddPersonAsync("Bob");
        var carol = await AddPersonAsync("Carol");

        var settlement = new Settlement
        {
            FromPersonId = bob.Id,
            ToPersonId = alice.Id,
            Amount = 12.34m,
            CurrencyCode = "usd",
            Date = new DateTime(2026, 11, 12),
            Note = "Cash",
        };
        await _settlements.SaveAsync(settlement);
        await _settlements.SaveAsync(new Settlement
        {
            FromPersonId = carol.Id, ToPersonId = alice.Id, Amount = 1m, CurrencyCode = "USD", Date = new DateTime(2026, 11, 13),
        });

        var loaded = await _settlements.GetAsync(settlement.Id);
        Assert.Equal(12.34m, loaded!.Amount);
        Assert.Equal("USD", loaded.CurrencyCode);
        Assert.Equal("Cash", loaded.Note);
        Assert.Equal(new DateTime(2026, 11, 12), loaded.Date);

        Assert.Equal([settlement.Id], (await _settlements.GetForPersonAsync(bob.Id)).Select(s => s.Id));
        Assert.Equal(2, (await _settlements.GetForPersonAsync(alice.Id)).Count);

        await Assert.ThrowsAsync<ValidationException>(() => _settlements.SaveAsync(new Settlement
        {
            FromPersonId = bob.Id, ToPersonId = bob.Id, Amount = 1m, CurrencyCode = "USD",
        }));
    }

    [Fact]
    public async Task Categories_can_be_added_and_are_ordered()
    {
        await _categories.SaveAsync(new Category { Name = "Pets", SortOrder = -1 });

        var all = await _categories.GetAllAsync();

        Assert.Equal("Pets", all[0].Name);
        Assert.False(all[0].IsDefault);
    }

    [Fact]
    public async Task Balances_from_stored_data()
    {
        var alice = await AddPersonAsync("Alice");
        var bob = await AddPersonAsync("Bob");
        var carol = await AddPersonAsync("Carol");

        await _transactions.SaveAsync(NewExpense(alice.Id, 10m, alice.Id, bob.Id, carol.Id));
        var deleted = NewExpense(bob.Id, 100m, alice.Id, bob.Id);
        await _transactions.SaveAsync(deleted);
        await _transactions.DeleteAsync(deleted.Id);
        await _settlements.SaveAsync(new Settlement
        {
            FromPersonId = bob.Id, ToPersonId = alice.Id, Amount = 3.33m, CurrencyCode = "EUR",
        });

        var debts = BalanceCalculator.CalculateDebts(
            await _transactions.GetAllAsync(includeDeleted: true),
            await _settlements.GetAllAsync());

        Assert.Equal([new Debt(carol.Id, alice.Id, "EUR", 3.33m)], debts);
    }
}
