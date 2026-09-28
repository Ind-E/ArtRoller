using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace ArtRoller.Patches;

[HarmonyPatch(typeof(NCard), "Reload")]
public static class NCardPatch
{
    /// <summary>
    /// NCard nodes are reused and the base game never resets FlipH or FlipV, so without this a card
    /// with no roll inherits the flips of whatever the node showed last. It must stay unconditional.
    /// </summary>
    internal static void Prefix(NCard __instance)
    {
        if (Portrait(__instance) is not { } portrait) return;
        portrait.FlipH = false;
        portrait.FlipV = false;
    }

    [HarmonyPriority(Priority.Last)]
    internal static void Postfix(NCard __instance)
    {
        // Do not filter to "our" card types here. A reprint is still the base game's type, so such a
        // test skips exactly the cards scoped rolls exist for (it once did, in Into the Spireverse).
        // Resolve's lookups are exact-key, so a card with no roll of its own already gets null.
        //
        // A null roll must still be applied rather than returned early on: it resets the shader to
        // neutral, for the same node-reuse reason as the Prefix.
        if (Portrait(__instance) is not { } portrait) return;
        CardShaderHelper.ApplyToPortrait(portrait, CardArtRoller.Resolve(__instance.Model));
    }

    private static TextureRect? Portrait(NCard card)
    {
        if (!card.IsNodeReady() || card.Model is not { } model) return null;
        return card.GetNodeOrNull<TextureRect>(model.Rarity == CardRarity.Ancient ? "%AncientPortrait" : "%Portrait");
    }
}
