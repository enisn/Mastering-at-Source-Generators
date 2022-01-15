using Awesome.Generators;

namespace Awesome.Api;

[GenerateService]
public class Todo
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public bool IsCompleted { get; set; }
}
