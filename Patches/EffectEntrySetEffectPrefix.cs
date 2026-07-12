using HarmonyLib;
using LinkedMovement;
using System.Reflection;
using UnityEngine;

#nullable disable
[HarmonyPatch]
class EffectEntrySetEffectPrefix {
    static MethodBase TargetMethod() {
        MethodBase methodBase = (MethodBase)AccessTools.Method(typeof(EffectEntry), "setEffect");
        if (methodBase != null) {
            LMLogger.Info("EffectEntry.setEffect method found");
        } else {
            LMLogger.Info("EffectEntry.setEffect method NOT FOUND");
        }
        return methodBase;
    }

    // If the assigned object also carries a triggerable animation, the animation overrides the object's built-in effect (lights, doors).
    // Swapping here (rather than only at getEffect) makes initializeOnFirstAssignment run on the trigger, so the entry gets the animation's duration.
    [HarmonyPrefix]
    static void setEffect(ref IEffect effect) {
        if (effect == null || effect is LinkedMovement.LMTrigger) return;
        if (!(effect is Component component)) return;

        var trigger = component.GetComponent<LinkedMovement.LMTrigger>();
        if (trigger == null) return;

        LMLogger.Debug($"EffectEntry.setEffect: overriding built-in effect with triggerable animation '{trigger.animationParams.name}'");
        effect = trigger;
    }
}
