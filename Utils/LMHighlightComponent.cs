using System;
using System.Collections.Generic;
using UnityEngine;

namespace LinkedMovement.Utils {

    [Flags]
    public enum HighlightType {
        None = 0,
        MouseOver = 1 << 0,
        AnimationTarget = 1 << 1,
        LinkParent = 1 << 2,
        LinkTarget = 1 << 3,
    }

    // TODO: Flag colors

    public class LMHighlightComponent : MonoBehaviour {
        // X-ray tint: selected objects get a per-object translucent color fill so the selection state is readable inside dense clusters
        private const string TINT_TAG = "LMSelectionVis";
        private const float TINT_ALPHA = 0.3f;
        private static readonly Dictionary<Color, Material> tintMaterialCache = new Dictionary<Color, Material>();

        private HighlightType currentFlags = HighlightType.None;
        private HighlightOverlayController.HighlightHandle highlightHandle;
        private Color? currentTintColor = null;

        // The tint is only helpful while picking Link targets; anywhere else it gets in the way, so it is gated globally
        private static bool tintPickingActive = false;
        public static void SetTintPickingActive(bool active) {
            if (tintPickingActive == active) return;
            LMLogger.Debug("LMHighlightComponent.SetTintPickingActive " + active);
            tintPickingActive = active;
            foreach (var highlightComponent in FindObjectsOfType<LMHighlightComponent>()) {
                highlightComponent.rebuildTint();
            }
        }

        public void addHighlightFlag(HighlightType flag) {
            LMLogger.Debug("LMHighlightComponent.addHighlightFlag " + flag.ToString());
            HighlightType oldFlags = currentFlags;
            currentFlags |= flag;

            if (oldFlags != currentFlags) {
                rebuildHighlight();
            }
        }

        public void removeHighlightFlag(HighlightType flag) {
            LMLogger.Debug("LMHighlightComponent.removeHighlightFlag " + flag.ToString());
            HighlightType oldFlags = currentFlags;
            currentFlags &= ~flag;

            if (oldFlags != currentFlags) {
                rebuildHighlight();
            }
        }

        public bool hasNoHighlights() {
            var highlightIsNone = currentFlags == HighlightType.None;
            //LinkedMovement.Log("LMHighlightComponent.hasNoHighlights: " + highlightIsNone);
            return highlightIsNone;
        }

        //public void clearHighlight() {
        //    LinkedMovement.Log("LMHighlightComponent.clearHighlight");
        //    currentFlags = HighlightType.None;
        //    rebuildHighlight();
        //}

        private bool hasFlag(HighlightType flag) {
            return (currentFlags & flag) == flag;
        }

        // TODO: This can be simplified. Many of these combinations are not possible with current UI flow.
        private void rebuildHighlight() {
            LMLogger.Debug("LMHighlightComponent.rebuildHighlight");
            rebuildTint();

            if (highlightHandle != null) {
                LMLogger.Debug("Remove existing");
                highlightHandle.remove();
                highlightHandle = null;
            }

            if (hasFlag(HighlightType.MouseOver)) {
                //LinkedMovement.Log("MouseOver");
                //buildHighlightWithColor(Color.grey);
                buildHighlightWithColor(Color.white);
                return;
            }
            if (hasFlag(HighlightType.AnimationTarget) && hasFlag(HighlightType.LinkParent) && hasFlag(HighlightType.LinkTarget)) {
                //LinkedMovement.Log("AnimationTarget, LinkParent, LinkTarget");
                buildHighlightWithColor(Color.white);
                return;
            }
            if (hasFlag(HighlightType.AnimationTarget) && hasFlag(HighlightType.LinkParent)) {
                //LinkedMovement.Log("AnimationTarget, LinkParent");
                buildHighlightWithColor(Color.magenta);
                return;
            }
            if (hasFlag(HighlightType.AnimationTarget) && hasFlag(HighlightType.LinkTarget)) {
                //LinkedMovement.Log("AnimationTarget, LinkTarget");
                buildHighlightWithColor(Color.cyan);
                return;
            }
            if (hasFlag(HighlightType.LinkParent) && hasFlag(HighlightType.LinkTarget)) {
                //LinkedMovement.Log("LinkParent, LinkTarget");
                buildHighlightWithColor(Color.yellow);
                return;
            }
            if (hasFlag(HighlightType.AnimationTarget)) {
                //LinkedMovement.Log("AnimationTarget");
                buildHighlightWithColor(Color.blue);
                return;
            }
            if (hasFlag(HighlightType.LinkParent)) {
                //LinkedMovement.Log("LinkParent");
                buildHighlightWithColor(Color.red);
                return;
            }
            if (hasFlag(HighlightType.LinkTarget)) {
                //LinkedMovement.Log("LinkTarget");
                buildHighlightWithColor(Color.green);
                return;
            }
            //LinkedMovement.Log("NONE! " + currentFlags.ToString());
        }

        private void buildHighlightWithColor(Color color) {
            LMLogger.Debug("LMHighlightComponent.buildHighlightWithColor " + color.ToString());
            var buildableObject = LMUtils.GetBuildableObjectFromGameObject(this.gameObject);
            List<Renderer> renderers = new List<Renderer>();
            buildableObject.retrieveRenderersToHighlight(renderers);
            highlightHandle = HighlightOverlayController.Instance.add(renderers, -1, color);
        }

        // MouseOver is outline-only; the tint reflects persistent selection state so a hover doesn't flicker the fill
        private Color? getTintColor() {
            if (!tintPickingActive) return null;
            var flags = currentFlags & ~HighlightType.MouseOver;
            if (flags == HighlightType.None) return null;
            if (flags == (HighlightType.AnimationTarget | HighlightType.LinkParent | HighlightType.LinkTarget)) return Color.white;
            if (flags == (HighlightType.AnimationTarget | HighlightType.LinkParent)) return Color.magenta;
            if (flags == (HighlightType.AnimationTarget | HighlightType.LinkTarget)) return Color.cyan;
            if (flags == (HighlightType.LinkParent | HighlightType.LinkTarget)) return Color.yellow;
            if (flags == HighlightType.AnimationTarget) return Color.blue;
            if (flags == HighlightType.LinkParent) return Color.red;
            if (flags == HighlightType.LinkTarget) return Color.green;
            return null;
        }

        private void rebuildTint() {
            Color? tintColor = getTintColor();
            if (currentTintColor == tintColor) return;

            LMLogger.Debug($"LMHighlightComponent.rebuildTint color: {tintColor}");
            var buildableObject = LMUtils.GetBuildableObjectFromGameObject(this.gameObject);
            if (buildableObject == null) return;

            if (currentTintColor != null) {
                Utility.destroyMaterialManagerByObject(buildableObject, TINT_TAG);
            }
            if (tintColor != null) {
                Utility.attachMaterialManagerByObject(buildableObject, TINT_TAG, GetTintMaterial(tintColor.Value), null);
            }
            currentTintColor = tintColor;
        }

        private static Material GetTintMaterial(Color color) {
            Material material;
            if (!tintMaterialCache.TryGetValue(color, out material)) {
                material = new Material(AssetManager.Instance.seeThroughMaterial);
                var tintColor = color;
                tintColor.a = TINT_ALPHA;
                material.color = tintColor;
                tintMaterialCache.Add(color, material);
            }
            return material;
        }

        // Safety net: if the component (or its object) dies without flags being cleared, release the tint and outline
        private void OnDestroy() {
            if (currentTintColor != null) {
                var buildableObject = LMUtils.GetBuildableObjectFromGameObject(this.gameObject);
                if (buildableObject != null) {
                    Utility.destroyMaterialManagerByObject(buildableObject, TINT_TAG);
                }
                currentTintColor = null;
            }
            if (highlightHandle != null) {
                highlightHandle.remove();
                highlightHandle = null;
            }
        }
    }
}
