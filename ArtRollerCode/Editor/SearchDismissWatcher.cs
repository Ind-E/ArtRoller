using Godot;

namespace ArtRoller.Editor;

/// <summary>
/// Cancels an open portrait search on a click outside the box and its results, or on Esc. Watches
/// <c>_Input</c>, which sees every click before the UI does, and leaves the click unhandled so
/// whatever was clicked still responds.
/// </summary>
public partial class SearchDismissWatcher : Node
{
    private readonly Control? _results;
    private readonly Control? _box;
    private readonly Action? _cancel;

    // Godot needs a parameterless constructor for every script class.
    public SearchDismissWatcher() { }

    public SearchDismissWatcher(Control results, Control box, Action cancel)
    {
        _results = results;
        _box = box;
        _cancel = cancel;
    }

    public override void _Input(InputEvent @event)
    {
        if (_results is not { Visible: true } || _box == null) return;

        if (@event is InputEventMouseButton { Pressed: true } click)
        {
            if (!_results.GetGlobalRect().HasPoint(click.Position) && !_box.GetGlobalRect().HasPoint(click.Position))
                _cancel?.Invoke();
        }
        else if (@event.IsActionPressed("ui_cancel"))
        {
            _cancel?.Invoke();
            GetViewport().SetInputAsHandled();
        }
    }
}
