using HarmonyLib;
using System.Reflection;

#nullable disable
[HarmonyPatch]
class DecoInitializePostfix {
    static MethodBase TargetMethod() {
        MethodBase methodBase = (MethodBase)AccessTools.Method(typeof(Deco), "Initialize");
        if (methodBase != null) {
            LinkedMovement.LinkedMovement.Log("Deco.Initialize method found");
        } else {
            LinkedMovement.LinkedMovement.Log("Deco.Initialize method NOT FOUND");
        }
        return methodBase;
    }

    [HarmonyPostfix]
    static void Initialize(Deco __instance) {
        if (__instance.isPreview) return;

        var name = __instance.getName();
        if (name.Contains(LinkedMovement.LinkedMovement.HELPER_OBJECT_NAME)) {
            LinkedMovement.LinkedMovement.GetLMController().addAnimationHelper(__instance);
        }
    }
}
