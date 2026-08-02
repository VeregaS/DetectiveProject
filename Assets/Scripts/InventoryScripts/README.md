# Resident Evil-style inventory setup

The runtime code provides a rectangular grid, automatic placement, drag-and-drop,
rotation, world pickup and a small item-description panel. It does not yet provide
stacking, combining, splitting, persistence or item-use actions.

## Scene setup

1. Keep one enabled `SettingsManager` in the bootstrap scene. The generated Input
   Actions asset already has `Player/Inventory` (T), `Interact`, `InspectTake`
   and `InspectPutBack` actions.
2. Under a screen-space Canvas create an inactive `InventoryRoot`. Add a child
   `GridRoot` with a top-left pivot `(0, 1)`. Do not add a Grid Layout Group: the
   view positions multi-cell items itself.
3. Add `InventoryGridView` to a convenient UI object. Assign `GridRoot`,
   `ItemUIPrefab`, cell size and spacing. `ItemUIPrefab` must contain
   `InventoryItemUI`; its existing prefab is compatible.
4. Add `InventoryManager` to an always-active scene object (not to the inactive
   `InventoryRoot`). Assign `InventoryRoot`, the grid view and the player. Choose
   grid width/height, for example 10 x 8.
5. Ensure the Canvas has a `GraphicRaycaster` and the scene has exactly one
   `EventSystem` with `InputSystemUIInputModule`; otherwise dragging and clicks do
   not arrive.
6. Optionally add `InventoryInspectorUI` to a panel and assign its panel, title and
   description TMP fields. Left-clicking an item opens it; right-clicking rotates.

## Creating and placing an item

1. Use **Assets > Create > Node Zero > Inventory Item**. Set its display name,
   description, icon, width, height and whether it can rotate.
2. Put `ItemPickup` on a world object with a Collider on the layer used by the
   player's `Interactor`. Assign the item data and the scene `InventoryManager`.
3. If **Inspect Before Pickup** is enabled, also configure `InspectionManager` with
   its UI, text, inspect point and player. Interaction opens the 3D inspection;
   `InspectTake` moves it into the grid and `InspectPutBack` restores its original
   transform.
4. Enter Play mode: interact with the object, take it, press T, then drag it.
   Invalid drops return to the previous position; right-click rotates when space
   permits.

## Current limitations

This is the spatial inventory foundation, not a complete Resident Evil game-item
system. Production work will normally add item commands (use/examine/combine/
discard), stack counts, save/load, controller navigation, audio/animation and
explicit feedback for invalid placement.
