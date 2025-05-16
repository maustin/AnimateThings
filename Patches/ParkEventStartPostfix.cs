//using HarmonyLib;
//using System.Reflection;
//using UnityEngine;

//#nullable disable
//[HarmonyPatch]
//class ParkEventStartPostfix {
//    [HarmonyTargetMethod]
//    static MethodBase TargetMethod() {
//        MethodBase methodBase = (MethodBase)AccessTools.Method(typeof(Park), "eventStart");
//        if (methodBase != null) {
//            LinkedMovement.LinkedMovement.Log("Park.eventStart method found");
//        }
//        else {
//            LinkedMovement.LinkedMovement.Log("Park.eventStart method NOT FOUND");
//        }
//        return methodBase;
//    }

//    [HarmonyPostfix]
//    static void eventStart() {
//        LinkedMovement.LinkedMovement.Log("Park.eventStart Postfix");

//        //GameObject go = new GameObject();
//        //go.name = "LinkedMovementController";
//        //LinkedMovement.LinkedMovement.Controller = go.AddComponent<LinkedMovement.LinkedMovementController>();


//    }
//}
