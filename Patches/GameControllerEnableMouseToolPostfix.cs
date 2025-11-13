using HarmonyLib;
using LinkedMovement;
using LinkedMovement.Utils;
using System.Reflection;

#nullable disable
[HarmonyPatch]
class GameControllerEnableMouseToolPostfix {
    static MethodBase TargetMethod() {
        MethodBase methodBase = (MethodBase)AccessTools.Method(typeof(GameController), "enableMouseTool");
        if (methodBase != null) {
            LMLogger.Info("GameController.enableMouseTool method found");
        } else {
            LMLogger.Info("GameController.enableMouseTool method NOT FOUND");
        }
        return methodBase;
    }

    [HarmonyPostfix]
    static void enableMouseTool(GameController __instance) {
        LMLogger.Debug("GameController.enableMouseTool Postfix");
        LMUtils.UpdateGameMouseMode(__instance.getActiveMouseTool() != null);
    }
}
