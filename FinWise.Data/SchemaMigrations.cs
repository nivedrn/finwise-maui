using FinWise.Data.Rows;
using SQLite;

namespace FinWise.Data;

/// <summary>
/// Creates and upgrades the database schema. The schema version is stored in SQLite's
/// <c>PRAGMA user_version</c> (0 for a new, empty database).
/// </summary>
/// <remarks>
/// To change the schema, add a step with the next version number and leave earlier steps alone:
/// they describe how existing databases were built. Each step runs inside the same database
/// transaction as the version update.
/// </remarks>
internal static class SchemaMigrations
{
    private static readonly (int Version, Action<SQLiteConnection, DateTimeOffset> Apply)[] Steps =
    [
        (1, CreateInitialSchema),
    ];

    public static int LatestVersion => Steps[^1].Version;

    public static int GetVersion(SQLiteConnection connection) =>
        connection.ExecuteScalar<int>("PRAGMA user_version");

    public static void Migrate(SQLiteConnection connection, DateTimeOffset now)
    {
        var current = GetVersion(connection);
        if (current > LatestVersion)
            throw new InvalidOperationException(
                $"The database has schema version {current}, but this version of FinWise only supports up to {LatestVersion}.");

        foreach (var (version, apply) in Steps.Where(s => s.Version > current))
        {
            apply(connection, now);
            connection.Execute($"PRAGMA user_version = {version}");
        }
    }

    private static void CreateInitialSchema(SQLiteConnection db, DateTimeOffset now)
    {
        // Guids are stored as text, DateTime/DateTimeOffset as ticks (UTC ticks for DateTimeOffset)
        // and bools as 0/1, matching how sqlite-net reads and writes them.
        const string common = """
            Id TEXT NOT NULL PRIMARY KEY,
            CreatedAt INTEGER NOT NULL,
            UpdatedAt INTEGER NOT NULL,
            IsDeleted INTEGER NOT NULL DEFAULT 0
            """;

        db.Execute($"""
            CREATE TABLE people (
                {common},
                Name TEXT NOT NULL,
                Email TEXT,
                PhoneNumber TEXT,
                IsCurrentUser INTEGER NOT NULL DEFAULT 0
            )
            """);

        db.Execute($"""
            CREATE TABLE categories (
                {common},
                Name TEXT NOT NULL,
                Icon TEXT,
                Color TEXT,
                SortOrder INTEGER NOT NULL DEFAULT 0,
                IsDefault INTEGER NOT NULL DEFAULT 0
            )
            """);

        db.Execute($"""
            CREATE TABLE transactions (
                {common},
                Title TEXT NOT NULL,
                Note TEXT,
                AmountE4 INTEGER NOT NULL,
                CurrencyCode TEXT NOT NULL,
                Date INTEGER NOT NULL,
                CategoryId TEXT,
                PayerId TEXT NOT NULL,
                SplitMethod INTEGER NOT NULL DEFAULT 0
            )
            """);
        db.Execute("CREATE INDEX ix_transactions_date ON transactions (Date)");
        db.Execute("CREATE INDEX ix_transactions_payer ON transactions (PayerId)");

        db.Execute($"""
            CREATE TABLE splits (
                {common},
                TransactionId TEXT NOT NULL,
                PersonId TEXT NOT NULL,
                AmountE4 INTEGER NOT NULL,
                SharesE4 INTEGER
            )
            """);
        db.Execute("CREATE INDEX ix_splits_transaction ON splits (TransactionId)");
        db.Execute("CREATE INDEX ix_splits_person ON splits (PersonId)");

        db.Execute($"""
            CREATE TABLE settlements (
                {common},
                FromPersonId TEXT NOT NULL,
                ToPersonId TEXT NOT NULL,
                AmountE4 INTEGER NOT NULL,
                CurrencyCode TEXT NOT NULL,
                Date INTEGER NOT NULL,
                Note TEXT
            )
            """);
        db.Execute("CREATE INDEX ix_settlements_from ON settlements (FromPersonId)");
        db.Execute("CREATE INDEX ix_settlements_to ON settlements (ToPersonId)");

        foreach (var category in DefaultCategories.Create(now))
        {
            db.Insert(category);
        }
    }
}
