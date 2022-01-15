namespace Awesome.Api.Data;

public interface IRepository<TModel>
    where TModel : class, IIdentifiable
{
    Task<List<TModel>> GetListAsync();

    Task<TModel> GetSingleAsync(Guid id);

    Task InsertAsync(TModel model);

    Task UpdateAsync(Guid id, TModel model);

    Task DeleteAsync(Guid id);
}
