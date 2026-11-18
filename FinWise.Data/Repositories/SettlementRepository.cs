using FinWise.Core.Models;
using FinWise.Core.Repositories;
using FinWise.Core.Validation;
using FinWise.Data.Rows;

namespace FinWise.Data.Repositories;

public sealed class SettlementRepository(FinWiseDatabase database)
    : SqliteRepository<Settlement, SettlementRow>(database), ISettlementRepository
{
    public async Task<IReadOnlyList<Settlement>> GetForPersonAsync(Guid personId)
    {
        var connection = await Database.GetConnectionAsync().ConfigureAwait(false);
        var rows = await connection.Table<SettlementRow>()
            .Where(s => s.IsDeleted == false && (s.FromPersonId == personId || s.ToPersonId == personId))
            .ToListAsync()
            .ConfigureAwait(false);
        return Sort(rows.Select(ToEntity)).ToList();
    }

    protected override void Normalize(Settlement entity)
    {
        entity.CurrencyCode = Currency.Normalize(entity.CurrencyCode);
        entity.Note = string.IsNullOrWhiteSpace(entity.Note) ? null : entity.Note.Trim();
    }

    protected override void Validate(Settlement entity) => EntityValidator.EnsureValid(entity);

    protected override IEnumerable<Settlement> Sort(IEnumerable<Settlement> entities) =>
        entities
            .OrderByDescending(s => s.Date)
            .ThenByDescending(s => s.CreatedAt)
            .ThenBy(s => s.Id);

    protected override Settlement ToEntity(SettlementRow row)
    {
        var settlement = new Settlement
        {
            FromPersonId = row.FromPersonId,
            ToPersonId = row.ToPersonId,
            Amount = Money.FromStorage(row.AmountE4),
            CurrencyCode = row.CurrencyCode,
            Date = row.Date,
            Note = row.Note,
        };
        CopyCommon(row, settlement);
        return settlement;
    }

    protected override SettlementRow ToRow(Settlement entity)
    {
        var row = new SettlementRow
        {
            FromPersonId = entity.FromPersonId,
            ToPersonId = entity.ToPersonId,
            AmountE4 = Money.ToStorage(entity.Amount),
            CurrencyCode = entity.CurrencyCode,
            Date = entity.Date,
            Note = entity.Note,
        };
        CopyCommon(entity, row);
        return row;
    }
}
