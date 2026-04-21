using ReactiveUI;
using TodoApp.Models;

namespace TodoApp.ViewModels;

public class TodoItemViewModel : ReactiveObject
{
    public TodoItem Model { get; } = new TodoItem();

    private bool _isCompleted;

    public bool IsCompleted
    {
        get => _isCompleted;
        set => this.RaiseAndSetIfChanged(ref _isCompleted, value, "IsCompleted");
    }
    public string Title => Model.Title;
    public int Id => Model.Id;

    public TodoItemViewModel(TodoItem model)
    {
        Model = model;
        _isCompleted = model.IsCompleted;
    }
}
