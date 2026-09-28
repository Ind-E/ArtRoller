using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Screens;

namespace ArtRoller.Editor;

/// <summary>
/// Keeps the editor on the card the inspect screen is showing, and asks before anything that
/// leaves that card (closing, or the arrow buttons) throws away unsaved edits.
/// </summary>
[HarmonyPatch(typeof(NInspectCardScreen))]
internal static class InspectScreenPatches
{
    /// <summary>Set while running an action the player already confirmed, so it is not asked again.</summary>
    private static bool _confirmed;
    private static bool _asking;

    [HarmonyPatch("SetCard")]
    [HarmonyPostfix]
    static void CardChanged(List<CardModel>? ____cards, int ____index)
    {
        if (____cards is { Count: > 0 }) ArtRollerEditorPatch.OnInspectedCardChanged(____cards[____index]);
    }

    [HarmonyPatch("UpdateCardDisplay")]
    [HarmonyPostfix]
    static void CardRedrawn(List<CardModel>? ____cards, int ____index)
    {
        if (____cards is { Count: > 0 }) ArtRollerEditorPatch.OnInspectedCardRedrawn(____cards[____index]);
    }

    // Runs before Open's own SetCard, so the editor knows whether this viewing is the library's.
    [HarmonyPatch(nameof(NInspectCardScreen.Open))]
    [HarmonyPrefix]
    static void BeforeOpen() => ArtRollerEditorPatch.OnInspectScreenOpening();

    // Close is also what Esc and Back are bound to, so this one guard covers all three ways out.
    [HarmonyPatch(nameof(NInspectCardScreen.Close))]
    [HarmonyPrefix]
    static bool BeforeClose(NInspectCardScreen __instance) =>
        ProceedOrAsk(__instance.Close);

    [HarmonyPatch("OnLeftButtonReleased")]
    [HarmonyPrefix]
    static bool BeforeLeft(NInspectCardScreen __instance) =>
        ProceedOrAsk(() => AccessTools.Method(typeof(NInspectCardScreen), "OnLeftButtonReleased").Invoke(__instance, null));

    [HarmonyPatch("OnRightButtonReleased")]
    [HarmonyPrefix]
    static bool BeforeRight(NInspectCardScreen __instance) =>
        ProceedOrAsk(() => AccessTools.Method(typeof(NInspectCardScreen), "OnRightButtonReleased").Invoke(__instance, null));

    /// <summary>
    /// True to let the original run now. Otherwise shows the discard prompt and returns false; the
    /// original is re-run through <paramref name="retry"/> if the player chooses to discard.
    /// </summary>
    private static bool ProceedOrAsk(Action retry)
    {
        if (_confirmed || !ArtRollerEditorPatch.HasUnsavedChanges) return true;
        if (!_asking) TaskHelper.RunSafely(AskThenRetry(retry));
        return false;
    }

    private static async Task AskThenRetry(Action retry)
    {
        var popup = NGenericPopup.Create();
        if (popup == null || NModalContainer.Instance == null) return;

        _asking = true;
        bool discard;
        try
        {
            NModalContainer.Instance.Add(popup);
            discard = await popup.WaitForConfirmation(
                body: new LocString("settings_ui", "ARTROLLER-EDITOR_UNSAVED.body"),
                header: new LocString("settings_ui", "ARTROLLER-EDITOR_UNSAVED.header"),
                noButton: new LocString("settings_ui", "ARTROLLER-EDITOR_UNSAVED.keep"),
                yesButton: new LocString("settings_ui", "ARTROLLER-EDITOR_UNSAVED.discard"));
        }
        finally
        {
            _asking = false;
        }

        if (!discard) return;

        _confirmed = true;
        try
        {
            retry();
        }
        finally
        {
            _confirmed = false;
        }
    }
}
