using Avalonia.ReactiveUI;
using TodoApp.ViewModels;

namespace TodoApp.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
    }
}
