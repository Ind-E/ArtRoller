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
        CardArtRoller.LoadUserRolls();
        new Harmony(ModId).PatchAll();
        ModConfigRegistry.Register(ModId, new ArtRollerConfig());
    }
}
