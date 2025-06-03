using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
[HarmonyPatch]
class BlueprintBuilderImplementationOnAfterBuildPostfix {
    static MethodBase TargetMethod() {
        Type[] p = new[] { typeof(List<BuildableObject>) };
        MethodBase methodBase = (MethodBase)AccessTools.Method(typeof(BlueprintBuilderImplementation), "onAfterBuild", p);
        if (methodBase != null) {
            LinkedMovement.LinkedMovement.Log("BlueprintBuilderImplementation.onAfterBuild method found");
        }
        else {
            LinkedMovement.LinkedMovement.Log("BlueprintBuilderImplementation.onAfterBuild method NOT FOUND");
        }
        return methodBase;
    }

    [HarmonyPostfix]
    static void onAfterBuild(List<BuildableObject> builtObjectInstances) {
        LinkedMovement.LinkedMovement.Log("BlueprintBuilderImplementation.onAfterBuild post @ " + DateTime.Now);
        if (builtObjectInstances == null) {
            LinkedMovement.LinkedMovement.Log("NULL PARAMETER");
            return;
        }
        if (builtObjectInstances.Count == 0) {
            LinkedMovement.LinkedMovement.Log("Empty parameter!");
            return;
        }
        if (LinkedMovement.LinkedMovement.GetController().catchCreatedObjects) {
            LinkedMovement.LinkedMovement.Log("Catch created # objs: " + builtObjectInstances.Count);
            foreach (BuildableObject obj in builtObjectInstances) {
                LinkedMovement.LinkedMovement.GetController().addBlueprintCreatedObject(obj.gameObject);
            }
        } else {
            LinkedMovement.LinkedMovement.Log("Not catching created objects");
        }
    }
}
