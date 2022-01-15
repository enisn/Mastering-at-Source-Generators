using Humanizer;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Awesome.Api.Data;

public class MongoDbRepository<TModel> : IRepository<TModel>
    where TModel : class, IIdentifiable
{
    protected MongoDbOptions Options { get; init; }

    protected IMongoCollection<TModel> Collection { get; init; }

    public MongoDbRepository(IOptions<MongoDbOptions> options)
    {
        Options = options.Value;
        var connectionUri = new Uri(options.Value.ConnectionString);
        var client = new MongoClient(Options.ConnectionString);
        Collection = client
            .GetDatabase(connectionUri.AbsolutePath.Trim('/'))
            .GetCollection<TModel>(typeof(TModel).Name.Pluralize());
    }

    public Task DeleteAsync(Guid id)
    {
        return Collection.DeleteOneAsync(x => x.Id == id);
    }

    public Task<List<TModel>> GetListAsync()
    {
        return Collection.AsQueryable().ToListAsync();
    }

    public Task<TModel> GetSingleAsync(Guid id)
    {
        return Collection.AsQueryable().Where(x => x.Id == id).SingleOrDefaultAsync();
    }

    public Task InsertAsync(TModel model)
    {
        model.Id = Guid.NewGuid();
        return Collection.InsertOneAsync(model);
    }

    public Task UpdateAsync(Guid id, TModel model)
    {
        model.Id = id;
        return Collection.ReplaceOneAsync(x => x.Id == id, model);
    }
}
