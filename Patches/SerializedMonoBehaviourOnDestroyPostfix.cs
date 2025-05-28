using HarmonyLib;
using LinkedMovement;
using System;
using System.Reflection;
using UnityEngine;

#nullable disable
[HarmonyPatch]
class SerializedMonoBehaviourOnDestroyPostfix {
    [HarmonyTargetMethod]
    static MethodBase TargetMethod() {
        MethodBase methodBase = (MethodBase)AccessTools.Method(typeof(SerializedMonoBehaviour), "OnDestroy");
        if (methodBase != null) {
            LinkedMovement.LinkedMovement.Log("SerializedMonoBehaviour.OnDestroy method found");
        } else {
            LinkedMovement.LinkedMovement.Log("SerializedMonoBehaviour.OnDestroy method NOT FOUND");
        }
        return methodBase;
    }

    [HarmonyPostfix]
    static void OnDestroy(SerializedMonoBehaviour __instance) {
        var bo = __instance as BuildableObject;
        if (bo != null) {
            var baseTransform = bo.transform;
            bool foundPlatform = false;

            var baseChildrenCount = baseTransform.childCount;
            for (var i = 0; i < baseChildrenCount; i++) {
                var child = baseTransform.GetChild(i);
                var childName = child.gameObject.name;
                if (childName.Contains("[Platform]")) {
                    foundPlatform = true;
                    break;
                }
            }
            if (foundPlatform) {
                LinkedMovement.LinkedMovement.Log("Found Platform, OnDestroy");
                LinkedMovement.LinkedMovement.GetController().removePlatformObject(bo);
            }
        }
    }
}
