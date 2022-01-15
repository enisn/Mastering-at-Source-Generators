using Microsoft.AspNetCore.Mvc;

namespace Awesome.Api.Controllers;

public partial class TodoController
{
    [HttpPut("mark-all-as-completed")]
    public async Task MarkAllAsCompletedAsync()
    {
        var todos = await _repository.GetListAsync();

        foreach (var todo in todos.Where(x => !x.IsCompleted))
        {
            await _repository.UpdateAsync(todo.Id, todo);
        }
    }
}
