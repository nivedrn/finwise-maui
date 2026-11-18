using FinWise.Core.Models;
using FinWise.Core.Repositories;
using FinWise.Core.Validation;
using FinWise.Data.Rows;
using SQLite;

namespace FinWise.Data.Repositories;

public sealed class TransactionRepository(FinWiseDatabase database)
    : SqliteRepository<Transaction, TransactionRow>(database), ITransactionRepository
{
    // Stay well below SQLite's limit on the number of parameters in one statement.
    private const int MaxIdsPerQuery = 500;

    public async Task<IReadOnlyList<Transaction>> GetForPersonAsync(Guid personId)
    {
        var connection = await Database.GetConnectionAsync().ConfigureAwait(false);
        var rows = await connection.QueryAsync<TransactionRow>(
            """
            SELECT * FROM transactions
            WHERE IsDeleted = 0
              AND (PayerId = ? OR Id IN (SELECT TransactionId FROM splits WHERE PersonId = ? AND IsDeleted = 0))
            """,
            personId, personId).ConfigureAwait(false);
        return Sort(await MaterializeAsync(connection, rows).ConfigureAwait(false)).ToList();
    }

    public async Task<IReadOnlyList<Transaction>> GetBetweenAsync(DateTime from, DateTime to)
    {
        var connection = await Database.GetConnectionAsync().ConfigureAwait(false);
        var rows = await connection.Table<TransactionRow>()
            .Where(t => t.IsDeleted == false && t.Date >= from && t.Date < to)
            .ToListAsync()
            .ConfigureAwait(false);
        return Sort(await MaterializeAsync(connection, rows).ConfigureAwait(false)).ToList();
    }

    protected override void Normalize(Transaction entity)
    {
        entity.Title = entity.Title.Trim();
        entity.Note = string.IsNullOrWhiteSpace(entity.Note) ? null : entity.Note.Trim();
        entity.CurrencyCode = Currency.Normalize(entity.CurrencyCode);
    }

    protected override void Validate(Transaction entity) => EntityValidator.EnsureValid(entity);

    protected override IEnumerable<Transaction> Sort(IEnumerable<Transaction> entities) =>
        entities
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.CreatedAt)
            .ThenBy(t => t.Id);

    protected override async Task<List<Transaction>> MaterializeAsync(
        SQLiteAsyncConnection connection,
        IReadOnlyList<TransactionRow> rows)
    {
        var transactions = rows.Select(ToEntity).ToList();
        if (transactions.Count == 0)
            return transactions;

        var splitRows = new List<SplitRow>();
        foreach (var chunk in transactions.Select(t => t.Id).Chunk(MaxIdsPerQuery))
        {
            var placeholders = string.Join(", ", Enumerable.Repeat("?", chunk.Length));
            splitRows.AddRange(await connection.QueryAsync<SplitRow>(
                $"SELECT * FROM splits WHERE IsDeleted = 0 AND TransactionId IN ({placeholders})",
                chunk.Cast<object>().ToArray()).ConfigureAwait(false));
        }

        var splitsByTransaction = splitRows
            .OrderBy(s => s.CreatedAt)
            .ThenBy(s => s.Id)
            .ToLookup(s => s.TransactionId);

        foreach (var transaction in transactions)
        {
            transaction.Splits = splitsByTransaction[transaction.Id].Select(ToSplit).ToList();
        }

        return transactions;
    }

    /// <summary>
    /// Writes the transaction's splits. A person's existing split row is updated in place (same id),
    /// so editing an amount does not create a new record; rows for people who are no longer part
    /// of the transaction are soft-deleted.
    /// </summary>
    protected override void AfterSave(SQLiteConnection connection, Transaction entity, DateTimeOffset now)
    {
        var existing = connection.Table<SplitRow>()
            .Where(s => s.TransactionId == entity.Id)
            .ToList();
        var existingByPerson = existing
            .GroupBy(s => s.PersonId)
            .ToDictionary(g => g.Key, g => g.OrderBy(s => s.IsDeleted).ThenByDescending(s => s.UpdatedAt).First());

        var active = entity.Splits.Where(s => !s.IsDeleted).ToList();
        var kept = new HashSet<Guid>();
        foreach (var split in active)
        {
            split.TransactionId = entity.Id;
            if (existingByPerson.TryGetValue(split.PersonId, out var row))
            {
                split.Id = row.Id;
                split.CreatedAt = row.CreatedAt;
            }
            else
            {
                if (split.Id == Guid.Empty)
                    split.Id = Guid.CreateVersion7();
                split.CreatedAt = now;
            }

            split.UpdatedAt = now;
            connection.InsertOrReplace(ToSplitRow(split));
            kept.Add(split.Id);
        }

        foreach (var row in existing.Where(r => !r.IsDeleted && !kept.Contains(r.Id)))
        {
            row.IsDeleted = true;
            row.UpdatedAt = now;
            connection.Update(row);
        }

        entity.Splits = active;
    }

    protected override void AfterDelete(SQLiteConnection connection, Guid id, DateTimeOffset now)
    {
        connection.Execute(
            "UPDATE splits SET IsDeleted = 1, UpdatedAt = ? WHERE TransactionId = ? AND IsDeleted = 0",
            now, id);
    }

    protected override Transaction ToEntity(TransactionRow row)
    {
        var transaction = new Transaction
        {
            Title = row.Title,
            Note = row.Note,
            Amount = Money.FromStorage(row.AmountE4),
            CurrencyCode = row.CurrencyCode,
            Date = row.Date,
            CategoryId = row.CategoryId,
            PayerId = row.PayerId,
            SplitMethod = (SplitMethod)row.SplitMethod,
        };
        CopyCommon(row, transaction);
        return transaction;
    }

    protected override TransactionRow ToRow(Transaction entity)
    {
        var row = new TransactionRow
        {
            Title = entity.Title,
            Note = entity.Note,
            AmountE4 = Money.ToStorage(entity.Amount),
            CurrencyCode = entity.CurrencyCode,
            Date = entity.Date,
            CategoryId = entity.CategoryId,
            PayerId = entity.PayerId,
            SplitMethod = (int)entity.SplitMethod,
        };
        CopyCommon(entity, row);
        return row;
    }

    private static Split ToSplit(SplitRow row)
    {
        var split = new Split
        {
            TransactionId = row.TransactionId,
            PersonId = row.PersonId,
            Amount = Money.FromStorage(row.AmountE4),
            Shares = Money.FromStorage(row.SharesE4),
        };
        CopyCommon(row, split);
        return split;
    }

    private static SplitRow ToSplitRow(Split split)
    {
        var row = new SplitRow
        {
            TransactionId = split.TransactionId,
            PersonId = split.PersonId,
            AmountE4 = Money.ToStorage(split.Amount),
            SharesE4 = Money.ToStorage(split.Shares),
        };
        CopyCommon(split, row);
        return row;
    }
}
