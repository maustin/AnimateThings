using HarmonyLib;
using LinkedMovement;
using LinkedMovement.Utils;
using System.Reflection;

#nullable disable
[HarmonyPatch]
class GameControllerRemoveMouseToolPostfix {
    static MethodBase TargetMethod() {
        MethodBase methodBase = (MethodBase)AccessTools.Method(typeof(GameController), "removeMouseTool");
        if (methodBase != null) {
            LMLogger.Info("GameController.removeMouseTool method found");
        } else {
            LMLogger.Info("GameController.removeMouseTool method NOT FOUND");
        }
        return methodBase;
    }

    [HarmonyPostfix]
    static void removeMouseTool(GameController __instance) {
        LMLogger.Debug("GameController.removeMouseTool Postfix");
        LMUtils.UpdateGameMouseMode(__instance.getActiveMouseTool() != null);
    }
}
