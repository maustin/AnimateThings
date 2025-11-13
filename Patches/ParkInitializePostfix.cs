using HarmonyLib;
using LinkedMovement;
using System.Reflection;

#nullable disable
[HarmonyPatch]
class ParkInitializePostfix {
    static MethodBase TargetMethod() {
        MethodBase methodBase = (MethodBase)AccessTools.Method(typeof(Park), "Initialize");
        if (methodBase != null) {
            LMLogger.Info("Park.Initialize method found");
        }
        else {
            LMLogger.Info("Park.Initialize method NOT FOUND");
        }
        return methodBase;
    }

    // This ensures the controller is created at the start of park load
    // TODO: Is this needed? If the controller is always created elsewhere, can eliminate this patch.
    [HarmonyPostfix]
    static void Initialize() {
        LMLogger.Debug("Park.Initialize Postfix");
        // Ensure Controller has been created
        LinkedMovement.LinkedMovement.GetLMController();
    }
}
