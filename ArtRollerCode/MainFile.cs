using BaseLib.Config;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace ArtRoller;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "ArtRoller";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        // Registers this mod's Node subclasses with Godot, so their overrides such as _Input run.
        Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(typeof(MainFile).Assembly);
        CardArtRoller.LoadUserRolls();
        new Harmony(ModId).PatchAll();
        ModConfigRegistry.Register(ModId, new ArtRollerConfig());
    }
}
