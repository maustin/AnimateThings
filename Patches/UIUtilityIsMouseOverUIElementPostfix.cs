using HarmonyLib;
using LinkedMovement;
using System.Reflection;
using UnityEngine;

#nullable disable
[HarmonyPatch]
class UIUtilityIsMouseOverUIElementPostfix {
    static MethodBase TargetMethod() {
        MethodBase methodBase = (MethodBase)AccessTools.Method(typeof(UIUtility), "isMouseOverUIElement", new[] { typeof(GameObject).MakeByRefType() });
        if (methodBase != null) {
            LMLogger.Info("UIUtility.isMouseOverUIElement method found");
        } else {
            LMLogger.Info("UIUtility.isMouseOverUIElement method NOT FOUND");
        }
        return methodBase;
    }

    // Patches the out-param overload that all callers (including the no-arg wrapper) ultimately invoke, so the game treats any mod window under the cursor as a UI element. This blocks camera scroll-wheel zoom (CameraController) and game-world click-through while the mouse is over mod UI.
    [HarmonyPostfix]
    static void isMouseOverUIElement(ref bool __result, ref GameObject hoveredObject) {
        if (__result) return;
        if (!LinkedMovement.LinkedMovement.HasLMController()) return;
        var windowManager = LinkedMovement.LinkedMovement.GetLMController().windowManager;
        if (windowManager == null || !windowManager.IsAnyWindowUnderMouse) return;
        __result = true;
        hoveredObject = null; // no uGUI object, so CameraController's NonBlockingUIElement zoom bypass can't fire
    }
}
