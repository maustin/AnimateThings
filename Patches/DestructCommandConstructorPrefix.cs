using HarmonyLib;
using System;

#nullable disable
[HarmonyPatch(typeof(DestructCommand), MethodType.Constructor, new Type[] { typeof(BuildableObject) })]
class DestructCommandConstructorPrefix {
    // This handles objects destroyed via right-click
    [HarmonyPrefix]
    static bool DestructCommand(BuildableObject buildableObject) {
        //LinkedMovement.LinkedMovement.Log("DestructCommand Constructor PREFIX");
        LinkedMovement.LinkedMovement.GetLMController().handleBuildableObjectDestruct(buildableObject);

        return true;
    }
}
