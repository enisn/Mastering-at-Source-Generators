using Awesome.Api.Data;
using Awesome.Generators;

namespace Awesome.Api;

[GenerateService("RepositoryController.txt")]
public class Todo : IIdentifiable
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public bool IsCompleted { get; set; }
}
