using HarmonyLib;
using LinkedMovement;
using LinkedMovement.Utils;
using System.Reflection;

// TODO: This should be eliminated

#nullable disable
[HarmonyPatch]
class ChunkedMeshEventStartPrefix {
    static MethodBase TargetMethod() {
        MethodBase methodBase = (MethodBase)AccessTools.Method(typeof(ChunkedMesh), "eventStart");
        if (methodBase != null) {
            LinkedMovement.LinkedMovement.Log("ChunkedMesh.eventStart method found");
        }
        else {
            LinkedMovement.LinkedMovement.Log("ChunkedMesh.eventStart method NOT FOUND");
        }
        return methodBase;
    }

    [HarmonyPrefix]
    static bool eventStart(ChunkedMesh __instance) {
        if (__instance == null) {
            return true;
        }
        if (__instance.gameObject == null) {
            return true;
        }
        Deco deco = __instance.gameObject.GetComponent<Deco>();
        if (deco == null) {
            return true;
        }
        PairBase pairBase = LMUtils.GetPairBaseFromSerializedMonoBehaviour(deco);
        if (pairBase != null) {
            LinkedMovement.LinkedMovement.Log($"ChunkedMesh.eventStart deco {deco.getName()} is pairBase, don't start");
            return false;
        }
        PairTarget pairTarget = LMUtils.GetPairTargetFromSerializedMonoBehaviour(deco);
        if (pairTarget != null) {
            LinkedMovement.LinkedMovement.Log($"ChunkedMesh.eventStart deco {deco.getName()} is pairTarget, don't start");
            return false;
        }
        return true;
    }
}
