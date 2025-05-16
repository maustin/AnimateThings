using HarmonyLib;
using LinkedMovement;
using System.Reflection;

#nullable disable
[HarmonyPatch]
class DecoInitializePostfix {
    [HarmonyTargetMethod]
    static MethodBase TargetMethod() {
        MethodBase methodBase = (MethodBase)AccessTools.Method(typeof(Deco), "Initialize");
        if (methodBase != null) {
            LinkedMovement.LinkedMovement.Log("Deco.Initialize method found");
        }
        else {
            LinkedMovement.LinkedMovement.Log("Deco.Initialize method NOT FOUND");
        }
        return methodBase;
    }

    [HarmonyPostfix]
    static void Initialize(Deco __instance) {
        //LinkedMovement.LinkedMovement.Log("Deco Initialize type: " + __instance.GetType().Name + ", name: " + __instance.name);
        //PairBase pairBase;
        //PairTarget pairTarget;
        //__instance.tryGetCustomData(out pairBase);
        //__instance.tryGetCustomData(out pairTarget);
        //if (pairBase != null) {
        //    LinkedMovement.LinkedMovement.Log("Deco Initialize has pairBase, " + pairBase.pairId);
        //    LinkedMovement.LinkedMovement.Log("Deco type: " + __instance.GetType().Name);

        //    // Find target object
        //    var sos = GameController.Instance.getSerializedObjects();
        //    LinkedMovement.LinkedMovement.Log("SerializedObjects count: " + sos.Count);
        //    foreach (var so in sos) {
        //        LinkedMovement.LinkedMovement.Log("SO: " + so.name + ", type: " + so.GetType().Name);
        //        PairTarget sPairTarget;
        //        so.tryGetCustomData(out sPairTarget);
        //        if (sPairTarget != null) {
        //            LinkedMovement.LinkedMovement.Log("Found a PairTarget");
        //            if (sPairTarget.pairId == pairBase.pairId) {
        //                LinkedMovement.LinkedMovement.Log("Matching pairId!");
        //                var pairing = new Pairing(__instance.gameObject, so.gameObject, pairBase.pairId);
        //                LinkedMovement.LinkedMovement.Controller.addPairing(pairing);
        //                break;
        //            }
        //        }
        //    }
        //}
        //if (pairTarget != null) {
        //    LinkedMovement.LinkedMovement.Log("Deco Initialize has pairTarget, " + pairTarget.pairId);
        //}
    }
}
