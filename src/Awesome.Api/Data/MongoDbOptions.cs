using System.Reflection;

namespace Awesome.Api.Data;

public class MongoDbOptions
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017/" + Assembly.GetEntryAssembly().GetName().Name.Replace(".", "_");
}
