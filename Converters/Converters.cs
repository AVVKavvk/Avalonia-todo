using Avalonia.Data.Converters;
using Avalonia.Media;

namespace TodoApp.Converters;

public static class Converters
{
    // Returns TextDecorations.Strikethrough when true, null when false
    public static readonly IValueConverter BoolToStrikethrough = new FuncValueConverter<
        bool,
        TextDecorationCollection?
    >(isCompleted => isCompleted ? TextDecorations.Strikethrough : null);

    // Returns 0.45 opacity when true, 1.0 when false
    public static readonly IValueConverter BoolToOpacity = new FuncValueConverter<bool, double>(
        isCompleted => isCompleted ? 0.45 : 1.0
    );
}
