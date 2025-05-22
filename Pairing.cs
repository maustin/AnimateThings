using System;
using System.Collections.Generic;
using UnityEngine;

namespace LinkedMovement
{
    public class Pairing {
        public GameObject baseGO;
        public List<GameObject> targetGOs = new List<GameObject>();
        public string pairingId;

        public Pairing() {
            LinkedMovement.Log("Pairing DEFAULT CONTSTRUCTOR");
        }

        public Pairing(GameObject baseGO, List<GameObject> targetGOs, string pId = null) {
            LinkedMovement.Log("Pairing contstructor with options");
            this.baseGO = baseGO;
            this.targetGOs = new List<GameObject>(targetGOs);

            if (pId != null) {
                pairingId = pId;
            }
            else {
                pairingId = Guid.NewGuid().ToString();
            }
            LinkedMovement.Log("Pairing ID: " + pairingId);

            LinkedMovement.GetController().addPairing(this);
        }

        public void connect() {
            LinkedMovement.Log("Pairing connect, # targets: " + targetGOs.Count);

            if (baseGO == null) {
                LinkedMovement.Log("MISSING BASE GO!");
                return;
            }

            LinkedMovement.Log("iterate targetGOs");
            foreach (GameObject targetGO in targetGOs) {
                // If ChunkedMesh, it's a built-in object and we need to handle it
                var targetChunkedMesh = targetGO.GetComponent<ChunkedMesh>();

                if (targetChunkedMesh != null) {
                    //LinkedMovement.Log("Target is built-in deco object, enable movement");
                    targetChunkedMesh.enabled = false;
                    var targetMeshRenderer = targetGO.GetComponent<MeshRenderer>();
                    if (targetMeshRenderer != null) {
                        targetMeshRenderer.enabled = true;
                    } else {
                        LinkedMovement.Log("MESHRENDERER NULL!");
                    }
                }

                var targetBO = targetGO.GetComponent<BuildableObject>();
                if (targetBO == null) {
                    LinkedMovement.Log("MISSING BO!");
                    return;
                }
                PairTarget pairTarget;
                targetBO.tryGetCustomData(out pairTarget);

                if (pairTarget == null) {
                    LinkedMovement.Log("MISSING PAIRTARGET!");
                    return;
                }

                targetGO.transform.position = baseGO.transform.position + new Vector3(pairTarget.offsetX, pairTarget.offsetY, pairTarget.offsetZ);
                LinkedMovement.Log($"TP x: {targetGO.transform.position.x}, y: {targetGO.transform.position.y}, z: {targetGO.transform.position.z}");
                LinkedMovementController.AttachTargetToBase(baseGO.transform, targetGO.transform);
            }
        }

        public void setCustomData(bool useOffset = false) {
            LinkedMovement.Log("Pairing.setCustomData useOffset: " + useOffset);
            var baseBO = baseGO.GetComponent<BuildableObject>();
            baseBO.addCustomData(getPairBase());

            foreach (GameObject targetGO in targetGOs) {
                var offset = Vector3.zero;
                if (useOffset) {
                    var baseP = baseGO.transform.position;
                    var targetP = targetGO.transform.position;
                    offset = targetP;
                    LinkedMovement.Log($"Base pos x: {baseP.x}, y: {baseP.y}, z: {baseP.z}");
                    LinkedMovement.Log($"Targ pos x: {targetP.x}, y: {targetP.y}, z: {targetP.z}");
                    LinkedMovement.Log($"Offs pos x: {offset.x}, y: {offset.y}, z: {offset.z}");
                    LinkedMovement.Log("---");
                }
                var targetBO = targetGO.GetComponent<BuildableObject>();
                targetBO.addCustomData(getPairTarget(offset.x, offset.y, offset.z));
            }
        }

        public PairBase getPairBase() {
            return new PairBase(pairingId);
        }

        public PairTarget getPairTarget(float offsetX, float offsetY, float offsetZ) {
            return new PairTarget(pairingId, offsetX, offsetY, offsetZ);
        }

    }
}
