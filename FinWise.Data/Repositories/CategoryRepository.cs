using FinWise.Core.Models;
using FinWise.Core.Repositories;
using FinWise.Core.Validation;
using FinWise.Data.Rows;

namespace FinWise.Data.Repositories;

public sealed class CategoryRepository(FinWiseDatabase database)
    : SqliteRepository<Category, CategoryRow>(database), ICategoryRepository
{
    protected override void Normalize(Category entity) => entity.Name = entity.Name.Trim();

    protected override void Validate(Category entity) => EntityValidator.EnsureValid(entity);

    protected override IEnumerable<Category> Sort(IEnumerable<Category> entities) =>
        entities
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name, StringComparer.CurrentCultureIgnoreCase)
            .ThenBy(c => c.Id);

    protected override Category ToEntity(CategoryRow row)
    {
        var category = new Category
        {
            Name = row.Name,
            Icon = row.Icon,
            Color = row.Color,
            SortOrder = row.SortOrder,
            IsDefault = row.IsDefault,
        };
        CopyCommon(row, category);
        return category;
    }

    protected override CategoryRow ToRow(Category entity)
    {
        var row = new CategoryRow
        {
            Name = entity.Name,
            Icon = entity.Icon,
            Color = entity.Color,
            SortOrder = entity.SortOrder,
            IsDefault = entity.IsDefault,
        };
        CopyCommon(entity, row);
        return row;
    }
}
