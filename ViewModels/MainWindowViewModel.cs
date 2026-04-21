using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.ViewModels;

public partial class MainWindowViewModel : ReactiveObject
{
    private readonly DatabaseService _db = new();

    public ObservableCollection<TodoItemViewModel> Todos { get; } = new();

    private string _newTodoText = string.Empty;
    public string NewTodoText
    {
        get => _newTodoText;
        set => this.RaiseAndSetIfChanged(ref _newTodoText, value);
    }

    public ReactiveCommand<Unit, Unit> AddTodoCommand { get; }

    public ReactiveCommand<TodoItemViewModel, Unit> DeleteTodoCommand { get; }

    public ReactiveCommand<TodoItemViewModel, Unit> ToggleCompleteCommand { get; }

    public MainWindowViewModel()
    {
        var canAdd = this.WhenAnyValue(x => x.NewTodoText, t => !string.IsNullOrWhiteSpace(t));
        AddTodoCommand = ReactiveCommand.CreateFromTask(AddTodoAsync, canAdd);

        DeleteTodoCommand = ReactiveCommand.CreateFromTask<TodoItemViewModel>(DeleteTodoAsync);
        ToggleCompleteCommand = ReactiveCommand.CreateFromTask<TodoItemViewModel>(
            ToggleCompleteAsync
        );

        _ = LoadTodosAsync();
    }

    private async Task LoadTodosAsync()
    {
        var items = await _db.GetAllTodosAsync();
        Todos.Clear();

        foreach (var item in items)
        {
            Todos.Add(new TodoItemViewModel(item));
        }
    }

    private async Task AddTodoAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTodoText))
            return;

        var item = new TodoItem { Title = NewTodoText.Trim() };
        await _db.AddTodoAsync(item);
        Todos.Insert(0, new TodoItemViewModel(item));

        NewTodoText = string.Empty;
    }

    private async Task DeleteTodoAsync(TodoItemViewModel todo)
    {
        await _db.DeleteTodoAsync(todo.Model);
        Todos.Remove(todo);
    }

    private async Task ToggleCompleteAsync(TodoItemViewModel vm)
    {
        vm.IsCompleted = !vm.IsCompleted;
        vm.Model.IsCompleted = vm.IsCompleted;
        await _db.UpdateTodoAsync(vm.Model);
    }
}
