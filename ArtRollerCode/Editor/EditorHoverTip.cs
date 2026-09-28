using Godot;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace ArtRoller.Editor;

/// <summary>
/// Shows the game's own hover tip over an editor control. Godot's TooltipText does nothing here:
/// the game never displays native tooltips.
///
/// Text comes from <c>settings_ui.json</c> as <c>ARTROLLER-EDITOR_{key}.title</c> and <c>.desc</c>,
/// because the game's HoverTip only accepts LocStrings.
/// </summary>
internal static class EditorHoverTip
{
    public static void Attach(Control control, string key)
    {
        var tip = new HoverTip(
            new LocString("settings_ui", $"ARTROLLER-EDITOR_{key}.title"),
            new LocString("settings_ui", $"ARTROLLER-EDITOR_{key}.desc"));

        // Labels ignore the mouse by default, so they would never report being hovered.
        if (control.MouseFilter == Control.MouseFilterEnum.Ignore)
            control.MouseFilter = Control.MouseFilterEnum.Pass;

        control.MouseEntered += () =>
        {
            // CreateAndShow registers one tip per owner and throws on a second.
            NHoverTipSet.Remove(control);
            NHoverTipSet.CreateAndShow(control, tip, HoverTipAlignment.Right);
        };
        control.MouseExited += () => NHoverTipSet.Remove(control);

        // Closing the inspect screen hides the control without a MouseExited, which would strand the tip.
        control.VisibilityChanged += () =>
        {
            if (!control.IsVisibleInTree()) NHoverTipSet.Remove(control);
        };
    }
}
