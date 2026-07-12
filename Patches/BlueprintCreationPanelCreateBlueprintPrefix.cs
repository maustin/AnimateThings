//using HarmonyLib;
//using LinkedMovement;
//using Parkitect.UI;
//using System.Collections.Generic;
//using System.Reflection;

//#nullable disable
//[HarmonyPatch]
//class BlueprintCreationPanelCreateBlueprintPrefix {
//    static MethodBase TargetMethod() {
//        MethodBase methodBase = (MethodBase)AccessTools.Method(typeof(BlueprintCreationPanel), "createBlueprint");
//        if (methodBase != null) {
//            LMLogger.Info("BlueprintCreationPanel.createBlueprint method found");
//        } else {
//            LMLogger.Info("BlueprintCreationPanel.createBlueprint method NOT FOUND");
//        }
//        return methodBase;
//    }

//    [HarmonyPrefix]
//    static bool createBlueprint(ref ObjectSelectionTool ___objectSelectionTool) {
//        List<SerializedMonoBehaviour> serializedObjects = BlueprintCreationUtility.collectBlueprintedObjects(___objectSelectionTool.getSelectedObjects());
//        long number = (long)(new BlueprintSerializer(serializedObjects, "A rather long name to take up some space in the serialized data, just to make sure there's enough space").getSerialized().Length * 8);
//        LMLogger.Info("BlueprintCreationPanel.createBlueprint Prefix, serialized size: " + number.ToString());
//        return true;
//    }
//}

