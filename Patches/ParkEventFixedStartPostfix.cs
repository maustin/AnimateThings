using HarmonyLib;
using LinkedMovement;
using LinkedMovement.Animation;
using LinkedMovement.Utils;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

#nullable disable
[HarmonyPatch]
class ParkEventFixedStartPostfix {
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

    // Post-park load, all objects should be created and we can now find pairings
    [HarmonyPostfix]
    static void eventFixedStart() {
        LinkedMovement.LinkedMovement.Log("Park.eventFixedStart Postfix");
        // Ensure Controller has been created
        LinkedMovement.LinkedMovement.GetController();

        var sos = GameController.Instance.getSerializedObjects();
        LinkedMovement.LinkedMovement.Log("SerializedObjects count: " + sos.Count);
        
        var createdPairings = new List<Pairing>();
        var createdAnimations = new List<LMAnimation>();

        for (int i = sos.Count - 1; i >= 0; i--) {
            var so = sos[i];

            // NEW
            LMAnimationParams animationParams = LMUtils.GetAnimationParamsFromSerializedMonoBehaviour(so);
            if (animationParams != null) {
                LinkedMovement.LinkedMovement.Log("Found animationParams");
                var animation = LinkedMovement.LinkedMovement.GetLMController().addAnimation(animationParams, so.gameObject);
                createdAnimations.Add(animation);
            }

            // OLD
            PairBase pairBase = LMUtils.GetPairBaseFromSerializedMonoBehaviour(so);
            if (pairBase != null) {
                LinkedMovement.LinkedMovement.Log("Found pairBase");

                var pairTargets = LMUtils.FindPairTargetSOs(pairBase);
                if (pairTargets.Count > 0) {
                    var pairTargetGOs = new List<GameObject>();
                    foreach (var pairTarget in pairTargets) {
                        pairTargetGOs.Add(pairTarget.gameObject);
                        //LMUtils.LogComponents(LMUtils.GetBuildableObjectFromGameObject(pairTarget.gameObject));
                    }

                    LinkedMovement.LinkedMovement.Log($"Creating Pairing with {pairTargetGOs.Count} targets");
                    var pairing = new Pairing(so.gameObject, pairTargetGOs, pairBase.pairId, pairBase.pairName);
                    pairBase.animParams.setStartingValues(so.transform);
                    pairing.connect(false);
                    createdPairings.Add(pairing);
                } else {
                    LinkedMovement.LinkedMovement.Log("No pair matches found, remove PairBase");
                    so.removeCustomData<PairBase>();
                }
            }
        }

        // NEW
        LinkedMovement.LinkedMovement.GetLMController().onParkStarted();

        // OLD
        var sortedPairings = LMUtils.SortPairings(createdPairings);
        foreach (var pairing in sortedPairings) {
            pairing.createSequence();
        }

        // TODO: Do we need to find orphaned PairTargets?
    }
}
