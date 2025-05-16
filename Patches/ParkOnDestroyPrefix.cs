//using HarmonyLib;
//using System.Reflection;
//using UnityEngine;

//#nullable disable
//[HarmonyPatch]
//class ParkOnDestroyPrefix {
//    [HarmonyTargetMethod]
//    static MethodBase TargetMethod() {
//        MethodBase methodBase = (MethodBase)AccessTools.Method(typeof(Park), "OnDestroy");
//        if (methodBase != null) {
//            LinkedMovement.LinkedMovement.Log("Park.OnDestroy method found");
//        }
//        else {
//            LinkedMovement.LinkedMovement.Log("Park.OnDestroy method NOT FOUND");
//        }
//        return methodBase;
//    }

//    [HarmonyPrefix]
//    static bool OnDestroy() {
//        LinkedMovement.LinkedMovement.Log("Park.OnDestroy Prefix");
//        GameObject.Destroy(LinkedMovement.LinkedMovement.GetController().gameObject);
//        LinkedMovement.LinkedMovement.Controller = null;
//        return true;
//    }
//}
