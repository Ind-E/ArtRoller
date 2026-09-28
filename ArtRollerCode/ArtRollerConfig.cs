using BaseLib.Config;

namespace ArtRoller;

[ConfigHoverTipsByDefault]
internal class ArtRollerConfig : SimpleModConfig
{
    public static bool LoadPersonalEdits { get; set; } = true;

    public static bool PersonalEditMode { get; set; } = false;

    public static bool DeveloperMode { get; set; } = false;

    /// <summary>Developer Mode is a superset of Personal Edit Mode, so either one opens the editor.</summary>
    public static bool EditorEnabled => PersonalEditMode || DeveloperMode;

    /// <summary>
    /// Either editor mode forces personal rolls on: editing them while they did not render would
    /// make Save look broken.
    /// </summary>
    public static bool ApplyPersonalEdits => LoadPersonalEdits || EditorEnabled;
}
