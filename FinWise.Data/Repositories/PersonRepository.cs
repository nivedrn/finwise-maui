using FinWise.Core.Models;
using FinWise.Core.Repositories;
using FinWise.Core.Validation;
using FinWise.Data.Rows;
using SQLite;

namespace FinWise.Data.Repositories;

public sealed class PersonRepository(FinWiseDatabase database)
    : SqliteRepository<Person, PersonRow>(database), IPersonRepository
{
    public async Task<Person?> GetCurrentUserAsync()
    {
        var connection = await Database.GetConnectionAsync().ConfigureAwait(false);
        var row = await connection.Table<PersonRow>()
            .Where(p => p.IsCurrentUser && p.IsDeleted == false)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        return row is null ? null : ToEntity(row);
    }

    protected override void Normalize(Person entity)
    {
        entity.Name = entity.Name.Trim();
        entity.Email = string.IsNullOrWhiteSpace(entity.Email) ? null : entity.Email.Trim();
        entity.PhoneNumber = string.IsNullOrWhiteSpace(entity.PhoneNumber) ? null : entity.PhoneNumber.Trim();
    }

    protected override void Validate(Person entity) => EntityValidator.EnsureValid(entity);

    protected override void AfterSave(SQLiteConnection connection, Person entity, DateTimeOffset now)
    {
        // Only one person can be the app's user.
        if (entity.IsCurrentUser)
        {
            connection.Execute(
                "UPDATE people SET IsCurrentUser = 0, UpdatedAt = ? WHERE IsCurrentUser = 1 AND Id <> ?",
                now, entity.Id);
        }
    }

    protected override IEnumerable<Person> Sort(IEnumerable<Person> entities) =>
        entities
            .OrderByDescending(p => p.IsCurrentUser)
            .ThenBy(p => p.Name, StringComparer.CurrentCultureIgnoreCase)
            .ThenBy(p => p.Id);

    protected override Person ToEntity(PersonRow row)
    {
        var person = new Person
        {
            Name = row.Name,
            Email = row.Email,
            PhoneNumber = row.PhoneNumber,
            IsCurrentUser = row.IsCurrentUser,
        };
        CopyCommon(row, person);
        return person;
    }

    protected override PersonRow ToRow(Person entity)
    {
        var row = new PersonRow
        {
            Name = entity.Name,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            IsCurrentUser = entity.IsCurrentUser,
        };
        CopyCommon(entity, row);
        return row;
    }
}
