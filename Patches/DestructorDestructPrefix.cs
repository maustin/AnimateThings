using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;

#nullable disable
[HarmonyPatch]
class DestructorDestructPrefix {
    static MethodBase TargetMethod() {
        MethodBase methodBase = (MethodBase)AccessTools.Method(typeof(Destructor), "destruct");
        if (methodBase != null) {
            LinkedMovement.LinkedMovement.Log("Destructor.destruct method found");
        } else {
            LinkedMovement.LinkedMovement.Log("Destructor.destruct method NOT FOUND");
        }
        return methodBase;
    }

    // This handles objects destroyed with Bulldozer tool
    [HarmonyPrefix]
    static bool destruct(IList<BuildableObject> toDestroy) {
        //LinkedMovement.LinkedMovement.Log($"Destructor.destruct PREFIX got {toDestroy.Count} items");
        foreach (BuildableObject buildableObject in toDestroy) {
            LinkedMovement.LinkedMovement.GetLMController().handleBuildableObjectDestruct(buildableObject);
        }
        
        return true;
    }
}
