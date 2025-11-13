using HarmonyLib;
using LinkedMovement;
using System.Reflection;

#nullable disable
[HarmonyPatch]
class DecoDestructPostfix {
    static MethodBase TargetMethod() {
        MethodBase methodBase = (MethodBase)AccessTools.Method(typeof(Deco), "destruct");
        if (methodBase != null) {
            LMLogger.Info("Deco.destruct method found");
        } else {
            LMLogger.Info("Deco.destruct method NOT FOUND");
        }
        return methodBase;
    }

    [HarmonyPostfix]
    static void destruct(Deco __instance) {
        if (__instance.isPreview) return;

        var name = __instance.getName();
        if (name.Contains(LinkedMovement.LinkedMovement.HELPER_OBJECT_NAME)) {
            LinkedMovement.LinkedMovement.GetLMController().removeAnimationHelper(__instance);
        }
    }
}
