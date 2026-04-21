using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApp.Models;

namespace TodoApp.Data;

public interface IDatabaseService
{
    public Task<List<TodoItem>> GetAllTodosAsync();

    public Task<TodoItem?> GetTodoAsync(int id);
    public Task<int> AddTodoAsync(TodoItem todo);
    public Task<int> UpdateTodoAsync(TodoItem todo);
    public Task<int> DeleteTodoAsync(TodoItem todo);
}
