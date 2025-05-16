using HarmonyLib;
using LinkedMovement;
using System.Reflection;
using UnityEngine;

#nullable disable
[HarmonyPatch]
class ParkEventFixedStartPostfix {
    [HarmonyTargetMethod]
    static MethodBase TargetMethod() {
        MethodBase methodBase = (MethodBase)AccessTools.Method(typeof(Park), "eventFixedStart");
        if (methodBase != null) {
            LinkedMovement.LinkedMovement.Log("Park.eventFixedStart method found");
        }
        else {
            LinkedMovement.LinkedMovement.Log("Park.eventFixedStart method NOT FOUND");
        }
        return methodBase;
    }

    [HarmonyPostfix]
    static void eventFixedStart() {
        LinkedMovement.LinkedMovement.Log("Park.eventFixedStart Postfix");

        var sos = GameController.Instance.getSerializedObjects();
        LinkedMovement.LinkedMovement.Log("SerializedObjects count: " + sos.Count);

        foreach (var so in sos) {
            PairBase pairBase;
            so.tryGetCustomData(out pairBase);
            if (pairBase != null) {
                LinkedMovement.LinkedMovement.Log("Found pairBase");
                SerializedMonoBehaviour smb = FindPairTargetSO(pairBase);
                if (smb != null) {
                    LinkedMovement.LinkedMovement.Log("Creating Pairing");
                    Pairing pair = new Pairing(so.gameObject, smb.gameObject, pairBase.pairId);
                    LinkedMovement.LinkedMovement.GetController().addPairing(pair);
                }
                else {
                    LinkedMovement.LinkedMovement.Log("No pair matches found");
                }
            }
        }
    }

    static SerializedMonoBehaviour FindPairTargetSO(PairBase pairBase) {
        var sos = GameController.Instance.getSerializedObjects();
        foreach (var so in sos) {
            PairTarget pairTarget;
            so.tryGetCustomData(out pairTarget);
            if (pairTarget != null) {
                LinkedMovement.LinkedMovement.Log("Found PairTarget");
                if (pairTarget.pairId == pairBase.pairId) {
                    LinkedMovement.LinkedMovement.Log("Same pairId!");
                    return so;
                }
            }
        }
        return null;
    }
}
