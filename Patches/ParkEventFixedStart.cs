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
                //PairTarget pairTarget = FindPairTarget(pairBase);
                //PairTarget pairTaget = FindPairTargetSO(so, pairBase);
                //Pairing pairing = new Pairing(so.gameObject)
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

    //static PairTarget FindPairTarget(PairBase pairBase) {
    //    var sos = GameController.Instance.getSerializedObjects();
    //    foreach (var so in sos) {
    //        PairTarget pairTarget;
    //        so.tryGetCustomData(out pairTarget);
    //        if (pairTarget != null) {
    //            LinkedMovement.LinkedMovement.Log("Found PairTarget!");
    //            if (pairBase.pairId == pairTarget.pairId) {
    //                LinkedMovement.LinkedMovement.Log("Same pairId!");
    //                return pairTarget;
    //            } else {
    //                LinkedMovement.LinkedMovement.Log("Different pairId, continuing");
    //            }
    //        }
    //    }
    //    return null;
    //}

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
