using HarmonyLib;
using LinkedMovement;
using System.Reflection;
using UnityEngine;

#nullable disable
[HarmonyPatch]
class EffectEntryGetEffectPostfix {
    static MethodBase TargetMethod() {
        MethodBase methodBase = (MethodBase)AccessTools.Method(typeof(EffectEntry), "getEffect");
        if (methodBase != null) {
            LMLogger.Info("EffectEntry.getEffect method found");
        } else {
            LMLogger.Info("EffectEntry.getEffect method NOT FOUND");
        }
        return methodBase;
    }

    // EffectEntry only persists its target object, so on park load initialize() re-resolves to the first IEffect on the
    // object, which is the built-in effect. Swap in the animation trigger at read time so the override holds regardless
    // of whether the entry resolved before or after this mod re-created its triggers (load order is not guaranteed).
    [HarmonyPostfix]
    static void getEffect(EffectEntry __instance, ref IEffect __result) {
        if (__result == null) return;

        if (__result is LinkedMovement.LMTrigger trigger) {
            if (trigger != null) return;
            // The trigger was destroyed (animation removed or no longer triggerable), fall back to the object's built-in effect
            __result = null;
            var target = __instance.getTarget();
            if (target == null) return;
            foreach (var candidate in target.GetComponents<IEffect>()) {
                if (candidate is LinkedMovement.LMTrigger) continue;
                __result = candidate;
                return;
            }
            return;
        }

        if (!(__result is Component component) || component == null) return;

        var overridingTrigger = component.GetComponent<LinkedMovement.LMTrigger>();
        if (overridingTrigger != null) __result = overridingTrigger;
    }
}
