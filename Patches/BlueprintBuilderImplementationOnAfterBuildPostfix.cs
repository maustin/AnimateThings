using HarmonyLib;
using LinkedMovement;
using LinkedMovement.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

#nullable disable
[HarmonyPatch]
class BlueprintBuilderImplementationOnAfterBuildPostfix {
    static MethodBase TargetMethod() {
        Type[] p = new[] { typeof(List<BuildableObject>) };
        MethodBase methodBase = (MethodBase)AccessTools.Method(typeof(BlueprintBuilderImplementation), "onAfterBuild", p);
        if (methodBase != null) {
            LMLogger.Info("BlueprintBuilderImplementation.onAfterBuild method found");
        } else {
            LMLogger.Info("BlueprintBuilderImplementation.onAfterBuild method NOT FOUND");
        }
        return methodBase;
    }

    [HarmonyPostfix]
    static void onAfterBuild(BlueprintBuilderImplementation __instance, List<BuildableObject> builtObjectInstances) {
        LMLogger.Debug("BlueprintBuilderImplementation.onAfterBuild post @ " + DateTime.Now);
        
        var forward = __instance.data.forward;
        Quaternion Angle = Quaternion.LookRotation(forward);
        LMLogger.Debug("FORWARD: " + forward.ToString());
        LMLogger.Debug("ANGLE: " + Angle.ToString());
        LMLogger.Debug("ANGLE euler: " + Angle.eulerAngles.ToString());
        if (builtObjectInstances == null) {
            LMLogger.Debug("NULL BUILT INSTANCES");
            return;
        }
        if (builtObjectInstances.Count == 0) {
            LMLogger.Debug("Empty built instances!");
            return;
        }

        LMLogger.Debug("# created instances: " + builtObjectInstances.Count);
        
        LMUtils.BuildLinksAndAnimationsFromBlueprint(builtObjectInstances, forward);
    }
}
