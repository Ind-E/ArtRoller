using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace ArtRoller.Patches;

[HarmonyPatch(typeof(CardModel))]
public class CardModelPortraitPatch
{
    private static bool _bypass;

    /// <summary>The card's own art, ignoring any roll's portrait override.</summary>
    public static string OriginalPortraitPath(CardModel card)
    {
        _bypass = true;
        try
        {
            return card.PortraitPath;
        }
        finally
        {
            _bypass = false;
        }
    }

    [HarmonyPatch(nameof(CardModel.PortraitPath), MethodType.Getter)]
    [HarmonyPrefix]
    static bool OverridePortraitPath(CardModel __instance, ref string __result)
    {
        if (_bypass) return true;

        var hsv = CardArtRoller.Resolve(__instance);
        if (hsv != null && !string.IsNullOrWhiteSpace(hsv.PortraitPath))
        {
            __result = hsv.PortraitPath;
            return false;
        }

        return true;
    }
}
