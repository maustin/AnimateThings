using HarmonyLib;
using LinkedMovement;
using System.Reflection;

#nullable disable
[HarmonyPatch]
class ParkEventFixedStartPostfix {
    static MethodBase TargetMethod() {
        MethodBase methodBase = (MethodBase)AccessTools.Method(typeof(Park), "eventFixedStart");
        if (methodBase != null) {
            LMLogger.Info("Park.eventFixedStart method found");
        }
        else {
            LMLogger.Info("Park.eventFixedStart method NOT FOUND");
        }
        return methodBase;
    }

    // Post-park load, all objects should be created and we can now find pairings
    [HarmonyPostfix]
    static void eventFixedStart() {
        LMLogger.Debug("Park.eventFixedStart Postfix");
        // Ensure LMController has been created
        LinkedMovement.LinkedMovement.GetLMController();

        var sos = GameController.Instance.getSerializedObjects();
        LMLogger.Debug("SerializedObjects count: " + sos.Count);

        LinkedMovement.LinkedMovement.GetLMController().setupPark(sos);
    }
}
