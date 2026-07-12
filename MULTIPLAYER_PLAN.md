# Multiplayer Compatibility — Status & Implementation Plan

Goal: make **Animate Things** fully *shared & synced* in Parkitect multiplayer — animations and
links created/edited/deleted by any player are replicated identically to every peer and survive
late-join, without ever causing a desync.

## Background (why this is possible)

Parkitect multiplayer is **deterministic lockstep with a host-ordered command stream**; a 32-bit
per-tick checksum mismatch kicks the diverging peer. Two facts make this mod tractable:

- **Animated state is not checksummed.** The `Checksums` struct (`Checksums.cs:14-65`) enumerates
  every checksummed field: park time, money, people/train/boat positions, ratings, station states,
  navmesh, and the three `RandomGenerator` streams. There is **no** field for Deco transforms,
  effect playback state, or anything the mod touches. Tweening placed scenery is invisible to the
  desync detector.
- The mod creates only plain `MonoBehaviour`s (no `SerializedMonoBehaviour` → no `objectID`
  perturbation) and used `UnityEngine.Random` (not the checksummed `RandomGenerator` streams).

So the real gap was **replication** (nothing was shared between peers), not desync-avoidance.

## Design

Every shared-state mutation is expressed as a Parkitect **`CustomCommand`** (stable built-in type id;
never perturbs the reflection-ordered protobuf command table). The UI no longer mutates directly: it
sends a command, and the registered handler performs the mutation on **every** peer (including the
sender). Objects are referenced across peers by **`SerializedMonoBehaviour.objectID` (uint)** —
allocated deterministically/identically in lockstep — resolved via
`GameController.Instance.getSerializedObject<BuildableObject>(objectID)`.

Payloads are the existing `SerializedRawObject` models round-tripped with Parkitect's own
`Serializer` + `MiniJSON.Json` to a UTF-8 `byte[]`. `addCustomCommand` runs the handler immediately
when offline and host-ordered when online, so **single-player exercises the identical
serialize→deserialize→apply path** as multiplayer.

**Boundary decision:** in multiplayer, only **`Deco` scenery** may be animated/linked. Rides, paths,
track, trains and stations feed checksummed state and animation playback is frame-time interpolated,
so allowing them would desync. Enforced only when in multiplayer mode.

**Concurrency model: last-writer-wins.** Two players may edit the same animation/link concurrently;
whoever commits later wins (the upsert handlers are idempotent replace-operations). No locking or
edit-ownership is planned — but a remote write landing on an object a player is *currently editing*
must cleanly cancel that local edit session (see Remaining work, item 3).

## What is DONE (builds clean; `MSBuild LinkedMovement.sln` green)

- **New file `Multiplayer/LMCommands.cs`** (added to `AnimateThings.csproj`):
  - Identifiers `LM.AnimUpsert / LM.AnimDelete / LM.LinkUpsert / LM.LinkDelete`.
  - `EnsureRegistered()` — registers handlers, re-registers if `CommandController` instance changes
    (new game/session), tolerant of duplicate registration.
  - `Send*` helpers (serialize payload + `addCustomCommand`).
  - `handle*` handlers (decode → resolve objectIDs → call controller apply methods).
  - `IsMultiplayerSafeTarget(BuildableObject)` — `Deco`-only in MP, anything in SP.
  - `Serialize/DeserializeRawObject` via `Serializer` + `MiniJSON` (side-effect-free; our raw objects
    have `canBeReferenced() == false`).
- **`LMController.cs`**: peer-agnostic, idempotent `applyAnimationUpsert/applyAnimationDelete/
  applyLinkUpsert/applyLinkDelete` + `removeLinkInternal`. `commitEdit` now captures final data, tears
  down local editing UI via `discardChanges`, and sends the command. `doRemoveAnimation/doRemoveLink`
  send delete commands. `EnsureRegistered()` called in `Awake` and `setupPark`.
- **`Animation/LMAnimation.cs`, `Links/LMLink.cs`**: Deco-only guard added to all three pickers
  (animation target, link parent, link target).
- **`Utils/LMUtils.cs`**: Deco-only guard in the three blueprint builders; `GetDeterministicStartDelay`
  (from `objectID`) replaces `UnityEngine.Random` for the start delay when in multiplayer.
- **`LinkedMovement.cs`**: `isMultiplayerModeCompatible() => true`,
  `isRequiredByAllPlayersInMultiplayerMode() => true`.
- Note: `LMAnimation.saveChanges`/`LMLink.saveChanges` are now unused but retained (history).

## Research findings (verified against decompiled Parkitect source)

These were open questions in the first draft of this plan; all three are now settled by source
analysis. File/line references are into `ParkitectLatest/Parkitect`.

### Late-join WILL carry mod state — CONFIRMED

- The host snapshots the park for a joiner via an ordinary `GameController.saveGame(...)` with
  `SerializationContext.Context.Multiplayer` (`PlayerJoinHandler.cs:40-54`), uploads it, and the
  joiner loads it through the standard `Loader.Instance.loadSavegame(...)` path with the same
  context (`MultiplayerController.cs:1779-1797`).
- `SerializedMonoBehaviour.customDataList` is `[Serialized]` with **no context restriction** — only
  `DontSerializeEmpty` / `DontSerializeIfValue(null)` (`SerializedMonoBehaviour.cs:32-35`). So all
  animation/link custom data is in the snapshot.
- `objectID` is `[OnlySerializeIn(Multiplayer)]` (`SerializedMonoBehaviour.cs:28-31`) and the park's
  allocator counter/hash (`Park.objectIDCount` / `objectIdHashCode`, `Park.cs:279-288`) are also
  Multiplayer-only serialized — the joiner's objects keep the **host's ids** and id allocation
  resumes in lockstep.
- The joiner's load fires `Park.eventFixedStart` through the normal `IFixedStartEvent` mechanism
  (`Park.cs:636`, `ObjectEventManager.cs:459-466`), so our `ParkEventFixedStartPostfix` → `setupPark`
  reconstruction **runs on a late-joiner** exactly like a save load.

Conclusion: no serialization work needed for late-join. The remaining late-join problem is the
**mid-tween starting-transform drift** described in Remaining work, item 2.

### Triggerable animations sync automatically — CONFIRMED (no `LM.Trigger` command needed)

- `EffectRunner` fires `IEffect.execute` from `eventFixedUpdate` (`EffectRunner.cs:135-195`), which
  `ObjectEventManager` drives from the fixed **simulation/logic tick**, gated in MP by
  `MultiplayerController.logicTicksNeedToWait` (`ObjectEventManager.cs:288-309`) — every peer runs
  the same number of logic ticks between network ticks.
- The trigger sources are all deterministic simulation events: `AttractionTrigger` subscribes to
  `Attraction.OnStartRideCycle`, `TrackSegmentTrigger` to `TrackSegment4.OnCarEnters` (train
  positions and ride cycles are checksummed state), `TimeIntervalTrigger` counts fixed ticks.
- There is **no** per-fire network command anywhere in the effects system (the effect commands are
  configuration-only: `EffectBoxChangeTriggerCommand` etc.), and the effects layer itself is treated
  as cosmetic — e.g. `EffectFireworksLauncher` uses plain `UnityEngine.Random` (line 152).

Conclusion: `LMTrigger` fires at the same logic tick on every peer with no work from us, because
the *cause* (EffectBox trigger config + simulation event) is already replicated/deterministic.
Caution for future changes: the fire path must stay purely simulation-driven — never gate a trigger
on local frame time, local `UnityEngine.Random`, or local-only UI state.

### Blueprint placement replicates fully — CONFIRMED, but exposes a new bug (item 1 below)

- `BlueprintBuildCommand.Data` carries the **entire blueprint file as `byte[]`** through the ordered
  command stream (`BlueprintBuildCommand.cs:12-46`, `BlueprintBuilder.cs:400-405`); it is an
  `OrderedCommand`, so **every peer** deserializes the identical bytes and runs the full build.
- `onAfterBuild` executes on **all** peers (BuilderImplementation\`1.cs:16-31) — only the camera
  shake/sound is gated on `isOwnCommand` (`BlueprintBuilderImplementation.cs:84-91`). So our
  `BlueprintBuilderImplementationOnAfterBuildPostfix` → `BuildLinksAndAnimationsFromBlueprint` runs
  on every peer with identical object instances, identical custom data, and identical (sequential,
  lockstep-allocated) objectIDs (`GameController.cs:1179-1195`, `Park.cs:3229-3237`).
- Blueprint *creation* is purely local (no command) — `BlueprintCreationPanel.createBlueprint`
  never touches `CommandController`.

Conclusion: blueprint-stamped animations/links replicate for free — **provided our reconstruction
is deterministic**. It currently is not (see next section).

## What REMAINS

In priority order. Items 1–4 are code changes; items 5–7 are verification/testing.

### 1. Make blueprint id regeneration deterministic (bug, must fix before MP testing)

`BuildLinksAndAnimationsFromBlueprint` regenerates link ids with `LMUtils.GetNewId()`
(= `Guid.NewGuid()`, `LMUtils.cs:117-127`) and animation ids with `animation.generateNewId()`.
Since `onAfterBuild` runs on **every** peer, each peer mints **different** GUIDs for the same
stamped link/animation. Each peer's park stays self-consistent (parent/target ids match locally,
and commands reference objectIDs, not these ids), and nothing checksums them — so this does not
desync — but peers' persisted custom data silently diverges, a late-joiner inherits the host's ids
while existing peers keep their own, and any future feature that matches by id across peers breaks.

**Fix:** derive regenerated ids deterministically from lockstep-stable inputs, e.g.
`$"bp-{parentBuildableObject.objectID}"` for the link id and `$"bp-{targetBuildableObject.objectID}"`
for the animation id. objectIDs are unique per park and identical on every peer at that point in the
command stream, so uniqueness and cross-peer identity both hold. Works identically in SP (objectIDs
are allocated there too). Keep `GetNewId()` for interactively-created animations/links — those ids
are minted on one peer and travel to the others inside the upsert payload, so they never diverge.

### 2. Fix starting-transform drift on load (bites late-join hardest)

`LMAnimation.setup()` (park load path) calls `setStartingValues(transform)`, which **unconditionally
re-captures the current transform as the animation origin** (`LMAnimationParams.cs:70-91`) — even
though the authoritative origin is already persisted in `[Serialized]`
`startingLocalPosition/Rotation/Scale`. Any park saved mid-tween therefore adopts the drifted pose
as the new origin. This is a pre-existing SP issue (saving while an animation plays), but late-join
makes it unavoidable: the join snapshot is **always** a mid-tween save, so a joiner's animations
would visibly diverge from the host's and the drift compounds across save cycles.

**Fix:** invert the direction on the load path — reset the transform *from* the persisted params
instead of re-capturing them, i.e. in `setup()` call
`LMUtils.ResetTransformLocals(transform, params.startingLocalPosition, ...)` before `buildSequence()`
(exactly what `applyAnimationUpsert` already does with wire params). Considerations:
- Keep re-capture for the flows where the object legitimately moved: blueprint stamping (object
  placed at a new location — `TryToBuildAnimationFromBlueprintObject` should keep adopting the
  placement pose) and edit-commit (already handled via the command payload).
- Version-gate if needed: params saved by old mod versions predate the forward-rotation fix; use
  `VersionMatchesOrExceedsMin` the way the `forward` handling already does if resetting old data
  proves unsafe.
- Link targets are re-parented before animations rebuild (`setupPark` order), so `localPosition`
  from params is relative to the same parent on every peer — verify order in testing.

### 3. Remote-write collision with a local edit session (hardening)

The apply methods replace/destroy `LMAnimation`/`LMLink` instances without checking whether the
local player is mid-edit on the same object. If peer B commits an upsert/delete on object X while
peer A has X open in the editor (`currentAnimation`/`currentLink` wraps it, temp copies +
highlights active, picking possibly live), A's edit session is left holding a destroyed model —
stale references, orphaned highlights, and a later commit that resurrects overwritten state.
The same hole exists when a remote player *deletes the object itself*
(`handleBuildableObjectDestruct` doesn't check the edit target either — pre-existing, but MP makes
it likely).

**Fix:** at the top of `applyAnimationUpsert/Delete`, `applyLinkUpsert/Delete`, and
`handleBuildableObjectDestruct`: if the affected object is the local edit target (compare against
`currentAnimation.targetGameObject` / `currentLink` parent+targets), call `clearEditMode()` first
and show an Info popup ("This animation/link was changed by another player; your edit was
cancelled"). Last-writer-wins stays intact; the local UI just never dangles.

### 4. Small hardening items

- **Payload versioning:** `isRequiredByAllPlayersInMultiplayerMode` guarantees every peer *has* the
  mod, not the same *version*. Add `payload["v"] = LinkedMovement.VERSION_NUMBER` to every `Send*`;
  handlers log (Info) on mismatch. Gives future payload changes a compatibility hook and makes
  mixed-version sessions diagnosable from the log.
- **Earlier handler registration on late-join:** `EnsureRegistered` currently runs at controller
  `Awake`/`setupPark` (first fixed update after load). A custom command arriving in the narrow
  window after the joiner resumes but before `eventFixedStart` would hit an unregistered handler
  and be lost on that peer. Cheap fix: also call `LMCommands.EnsureRegistered()` from
  `ParkInitializePostfix` (earliest point where `CommandController.Instance` exists). Idempotent,
  no downside.
- **Oversized payloads (verify only):** an animation with many steps serializes to a few KB of
  JSON; confirm nothing in the command transport imposes a small message limit (watch the log for
  send failures on a worst-case animation). No action expected.

### 5. Runtime verification — single player first (see Testing)

Validates serialization + apply logic on one machine; offline `addCustomCommand` runs the identical
path synchronously.

### 6. Two-client replication + desync watch

Including the concurrency scenarios (item 3) and blueprint stamping (item 1) once fixed.

### 7. Visual phase-lock of loops (optional, cosmetic)

Not a desync: PrimeTween runs on frame-time and MP catch-up scales `Time`, so long-running loops
drift out of visual phase between peers. Deterministic start delay gets same-time peers close.
Optional future work: offset each loop's initial phase from `ParkInfo.ParkTimeDouble` so a
late-joiner (and drifted peers) compute the same phase from shared park time.

## Testing

**Step 1 — single-player regression (highest value, do first).** Offline commands run the full
serialize/deserialize/apply path synchronously. With debug logging on: create/edit/delete animations
and links, save+reload, place a blueprint with an animated object. Pass = identical to pre-change
behavior, `Send* → handle* → apply*` in the log, no serialization exceptions. After item 2 lands:
save **while animations are mid-tween**, reload, confirm objects return to their authored origin
(this currently fails and is the regression test for the drift fix).

**Step 2 — two clients (lobby join).** Create/edit/delete animations and links from each peer; confirm
both directions replicate and **no desync dialog** over a multi-minute session with loops running.
Add: **stamp a blueprint containing animated/linked objects** from each peer — confirm both peers
build it, animations play on both, and (after item 1) the logged link/animation ids match on both
peers.

**Step 3 — boundary.** In MP, attempt to animate/link a ride/path/track → rejection popup; same
objects allowed in SP.

**Step 4 — concurrency.** With A editing an animation on X: B commits a different edit to X → A's
editor closes with the info popup and X plays B's animation on both peers. B deletes X's animation →
same. B deletes the object X itself → A's editor closes cleanly, no stale highlights or exceptions.
Repeat for links (B re-links a target A is using, B deletes A's link parent).

**Step 5 — late-join.** Host builds animations/links (loops running), second player joins mid-play:
confirm the joiner reconstructs everything, objects animate from the **same origin** as on the host
(item 2), and subsequent edits from either side replicate. Also late-join a park containing a
stamped blueprint (exercises inherited ids from item 1).

**Step 6 — triggers.** Wire a triggerable animation to a track-segment trigger via the Effects
system in MP; confirm it fires on both peers at the same moment (same train pass) with no command
traffic from the mod.

Log signals: desync after an action → that apply isn't deterministic (diff the two peers' logs around
the tick); change not appearing → `handle*` not firing (registration) or `couldn't resolve object`
(objectID mismatch); serialization exception → a params round-trip gap; ids differing between peers'
logs after a blueprint stamp → item 1 regressed.

## Key source references

- Mod command layer: `Multiplayer/LMCommands.cs`; apply methods: `LMController.cs` (`===== Multiplayer
  apply methods =====` section); blueprint reconstruction: `LMUtils.BuildLinksAndAnimationsFromBlueprint`.
- Parkitect commands: `CommandController.addCustomCommand/registerCustomCommand` (CustomCommand.cs,
  stable id), `CommandController.cs:82-93` (OrderedCommand routing),
  `GameController.getSerializedObject<T>(uint)`.
- objectID allocation: `GameController.cs:1179-1195` (`addSerializedObject`), `Park.cs:3229-3237`
  (`getNextObjectID`, checksummed counter at `MultiplayerController.cs:781`).
- Late-join: `PlayerJoinHandler.cs:40-66` (host snapshot, Multiplayer context),
  `MultiplayerController.cs:1765-1808` (joiner load + blind drain), `SerializedMonoBehaviour.cs:28-35`
  (objectID Multiplayer-only; customDataList unrestricted), `Park.cs:636` + `ObjectEventManager.cs:459-466`
  (`eventFixedStart` on load).
- Blueprints: `BlueprintBuildCommand.cs:12-46` (full file as `byte[]` in the command),
  `BlueprintBuilder.cs:400-405`, `BlueprintBuilderImplementation.cs:29-91` (`onAfterBuild` on all peers).
- Effects/triggers: `EffectRunner.cs:135-195` (fixed-tick fire), `ObjectEventManager.cs:288-309`
  (logic-tick gating via `logicTicksNeedToWait`), `AttractionTrigger.cs:41-59`,
  `TrackSegmentTrigger.cs:41-64`, `EffectBox.cs:80-155`.
- Checksums: `Checksums.cs:14-65`, `MultiplayerController.buildChecksum` (`MultiplayerController.cs:781-782`).
- Serialization gating: `Serializer.cs:176-201`, `OnlySerializeIn.cs:11-14`, `DontSerializeEmpty.cs:12-15`.
- Full analyses: `ParkitectLatest/MP_ANALYSIS - Claude.md`, `ParkitectLatest/MULTIPLAYER_ANALYSYS - Codex.md`.
