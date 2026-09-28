# Art Roller

Recolour, mirror and re-art any card in Slay the Spire 2, and let mods ship their own card recolours.

Art Roller does two jobs:

- **For players:** an editor in the Card Library for changing how any card looks: colours, brightness, contrast, flips, or its art entirely. Your changes stay on your computer.
- **For mod authors:** a shared library that applies the recolours your mod ships. Many mods reuse base-game art for their own cards, and Art Roller is how those cards get a look of their own. Every mod depending on this one copy means mods no longer fight over card rendering.

Requires **BaseLib**.

## Settings

Art Roller adds three settings to its page in the mod settings:

- **Load Personal Edits** (on by default): shows the edits you have saved. Turn it off to see every card exactly as its mod ships it. Your edits are kept and come back when you turn it on again.
- **Personal Edit Mode** (off by default): adds the editor to the Card Library. See [Personal Edit Mode](#personal-edit-mode).
- **Developer Mode** (off by default): everything in Personal Edit Mode, plus the tools for making recolours to ship with a mod. See [Developer Mode](#developer-mode).

Either editor mode also turns Load Personal Edits on, so you always see what you are editing.

## Personal Edit Mode

Turn on **Personal Edit Mode**, open the Card Library, and click any card you have discovered. The editor panel appears on the left of the enlarged card, and every change previews on the card straight away.

### Standard tab

Adjustments to the whole image. Every slider reads **100** when unchanged.

- **Tint:** click the swatch to pick a colour the whole image is multiplied by. White is unchanged.
- **Hue:** rotates every colour around the colour wheel.
- **Sat:** colour intensity. 0 is greyscale.
- **Lum:** brightness.
- **Gamma:** darkens or lifts the midtones without touching pure black and white.
- **Red, Green, Blue:** multiply that colour in the art.
- **Red, Green, Blue Offset:** add or remove that colour evenly, shadows included, which the multiply sliders cannot reach.
- **Contrast:** pushes darks and lights apart, or flattens them together.
- **Flip X / Flip Y:** mirror the art left to right, or top to bottom.

Order of operations: the colour channels and offsets, then Hue, Sat and Lum, then Contrast, Gamma and Tint.

### Selective Hue tab

Changes one colour in the art and leaves everything else alone, such as turning a blue flame purple. There are three independent sets.

1. Click a set's **Target** swatch, then press the **eyedropper** button in the colour picker and click the colour you want to change on the card's own art.
2. Raise or lower **Shift** to rotate that colour to a new one, and use **Saturation** and **Brightness** to mute, intensify, darken or lighten it.
3. Adjust **Width** until the change catches all of that colour and nothing else. Only the hue is compared, so darker and paler versions of the colour are caught too.
4. Raise **Softness** if the change leaves a hard edge or a visible fringe.

**Reset** beside a set's swatch turns just that set off.

The sets apply after the whole Standard tab, in order: Set 1, then Set 2, then Set 3, each working on the art as the sets above left it. That is exactly what you see on the card, so the eyedropper always picks the colour a set will match, as long as you pick a set's target before you shift it.

### Buttons

- **Search portrait paths:** replace the card's art with any other card's. Type part of a name and pick a result.
- **Save:** keeps your changes for this card. Until you save, everything is only a preview.
- **Clear:** deletes your saved changes, so the card goes back to its normal art.
- **Copy / Paste:** copy one card's colours and flips onto another. The art itself is not copied. Pasting is a preview until you save.
- **Reset:** puts everything back to neutral and shows the card's original art, as a preview.

If you try to leave a card with unsaved changes, by clicking the background, pressing Esc, or using the arrows to move to another card, Art Roller asks before discarding them.

Saved changes apply everywhere the card appears, on your computer only, and survive restarts and mod updates. Hover over any control for a reminder of what it does.

### Reprinted cards

Some modded characters reprint base-game cards in their own colours. When you edit a card from that character's tab in the Card Library, your change applies only when that character shows the card; the original character keeps its own look. Edit it from the original character's tab to change it everywhere.

## Developer Mode

For mod authors making recolours to ship with their mod. It has everything in Personal Edit Mode, plus:

- **Save Default:** writes the card's recolour as a file to ship with your mod, into the output folder.
- **Clear Default:** deletes that file from the output folder. A copy already packed into your mod keeps showing until you rebuild it.
- **Output folder** and **...**: choose where Save Default writes. Point it at the `ArtRoller` folder you create in your mod's project; see [Workflow](#workflow). Remembered between sessions.
- **Key:** the file name the buttons write to. A key ending in `@CHARACTER` is a reprint, which applies only while that character shows the card.

### Setting up your mod

1. Add Art Roller to your manifest's dependencies:

   ```json
   "dependencies": [{"id": "BaseLib", "min_version": "3.4.7"}, {"id": "ArtRoller", "min_version": "1.0.0"}]
   ```

2. Reference Art Roller from your `.csproj` through NuGet, the same way as BaseLib:

   ```xml
   <PackageReference Include="Shadowfall.Sts2.ArtRoller" Version="1.0.0" PrivateAssets="All" ExcludeAssets="runtime" />
   ```

   `ExcludeAssets="runtime"` keeps `ArtRoller.dll` out of your build; players get it from the Art Roller mod. The package works wherever you and your collaborators installed Art Roller, mods folder or Workshop, and updates like any other package.

   You need this only if your code calls Art Roller, such as registering your own folders in step 3. A mod that keeps its recolours in the default folder can skip it.

3. Art Roller reads recolours from `res://{YourModId}/ArtRoller/` and portrait search art from `res://{YourModId}/images/card_portraits/`, with no setup. For other folders, register them in your mod initializer, which needs the reference from step 2:

   ```csharp
   using ArtRoller;

   CardArtRoller.RegisterDefaultsDirectory("res://MyMod/SomewhereElse");
   CardArtRoller.RegisterPortraitDirectory("res://MyMod/art/portraits");
   ```

   Portrait folders are searched for `big/*.png`, so the portrait search can offer art that no card uses yet.

4. Add `*.hsv` to `include_filter` in your `export_presets.cfg`. Godot does not treat `.hsv` as a resource, so without this your recolours are silently left out of your `.pck`.

### Workflow

1. In your mod's project, create a folder named `ArtRoller` inside your mod's resource folder, the one named after your mod id. That is `res://{YourModId}/ArtRoller/` from step 3, which on disk is `{YourProject}/{YourModId}/ArtRoller/`.
2. Turn on **Developer Mode**, and set the output folder to that folder with the **...** button.
3. In the Card Library, open a card, adjust it, and press **Save Default**. The change shows immediately.
4. Rebuild your mod to pack the new files.
5. From then on your mod loads the new recolours as its shipped defaults, for you and for every player once published. On your own machine, a personal edit made with **Save** still takes priority over the default for the same card. Press **Clear** on that card, or turn off **Load Personal Edits** with both editor modes off, to see the true shipped default. Personal edits are stored in the game's user data folder under `ArtRoller`, which on Windows is `%APPDATA%\SlayTheSpire2\ArtRoller`.

## Already pasted Art Roller into your mod?

We are aware of a handful of mods that copied the Art Roller code out of Into the Spireverse before it was its own mod - if this applies to you, read this section. Two copies of Art Roller patching the same card could interfere with each other, and an old copy cannot read anything made with the newer controls. Switching to this mod fixes both and is thus recommended - but we have taken precautions to ensure it is optional. Compatibility should be preserved for a mod that has the Art Roller code duplicated, in a user that also has the new Art Roller mod subscribed.

**Until you switch,** Art Roller reads your copy's recolours on its own, from `res://{YourModId}/ArtRoller/`, and personal saves from `user://card_hsv_data` and your mod's `ArtRoller` folder. Your cards keep their look for players who have both installed. The game log names each mod that still carries its own copy.

**To switch:**

1. Delete your copy of the Art Roller code. It is usually a folder with these classes:
   - `CardArtRoller` and `CardHsvData`
   - `CardShaderHelper`
   - `CardModelPortraitPatch` and `NCardPatch`
   - `NCardLibraryVerticalSlidersPatch` and `PortraitSearchBox`
   - `AltArtContext` and `CardLibraryCharacterContextPatch`, if you copied the reprint support

   Delete the copied `color_adjust.gdshader` too.
2. Remove the `CardArtRoller.RegisterAllFromDirectory(...)` call from your mod initializer.
3. Leave your `.hsv` files where they are. If they are in `res://{YourModId}/ArtRoller/`, nothing else is needed. If not, register the folder as in [Setting up your mod](#setting-up-your-mod).
4. Follow [Setting up your mod](#setting-up-your-mod): add the dependency, and the NuGet package reference if your code calls Art Roller.
5. If your own code called the copied classes, point it at `ArtRoller.CardArtRoller`. The save methods now take a whole recolour instead of a list of numbers: `SaveHsvForCard(key, data)` and `SaveDefaultHsvForCard(key, data)`.

Your existing recolours keep working unchanged. Every setting a file leaves out counts as unchanged.

## Reference

### The `.hsv` format

A recolour is a JSON file named after its key. Every number is the editor's slider value divided by 100, so for every adjustment `1` means unchanged, offsets and hue shifts included. Fields a file leaves out are neutral.

`hue`, `saturation`, `value`, `gamma`, `red`, `red_offset`, `green`, `green_offset`, `blue`, `blue_offset`, `contrast`, `tint` (hex colour), `flip_h`, `flip_v`, `portrait_path`.

`selective_1`, `selective_2` and `selective_3` are objects with `color` (hex colour), `width`, `shift`, `saturation`, `brightness` and `softness`. `width` and `softness` shape the set rather than adjust it: `width` 0 turns the set off, and `softness` runs from 0 (hard edge) to 1 (fades from the target itself), default 0.5.

### Keys and reprints

A recolour is keyed by card id, e.g. `CARD.MYMOD-FIREBALL.hsv`.

A base-game card reprinted into your character's pool keeps the base game's id, so a plain key would recolour it for everyone. When a card is shown by a character from a different mod than the card, Art Roller first looks for a scoped key, `CARD.HAVOC@MYMOD-MY_CHARACTER.hsv`, then falls back to the plain key. In a run the card's owner decides; in the Card Library the selected character tab decides. The editor saves scoped keys on its own when you edit a reprint from your character's tab.

Registered folders are searched first, in registration order, then the automatically found ones. The first match wins.