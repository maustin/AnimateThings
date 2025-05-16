using System;
using UnityEngine;

namespace LinkedMovement
{
    public class Pairing : SerializedRawObject {
        [NonSerialized]
        public GameObject baseGO;
        [NonSerialized]
        public GameObject targetGO;

        [Serialized]
        public string pairingId;

        public Pairing() {
            LinkedMovement.Log("Pairing DEFAULT CONTSTRUCTOR");
        }

        public Pairing(GameObject baseGO, GameObject targetGO, string pId = null) {
            LinkedMovement.Log("Pairing contstructor with options");
            this.baseGO = baseGO;
            this.targetGO = targetGO;

            targetGO.transform.position = baseGO.transform.position;
            LinkedMovementController.AttachTargetToBase(baseGO.transform, targetGO.transform);

            if (pId != null) {
                pairingId = pId;
            } else {
                pairingId = Guid.NewGuid().ToString();
            }
            LinkedMovement.Log("Pairing ID: " + pairingId);
        }

        public PairBase getPairBase() {
            return new PairBase(pairingId);
        }

        public PairTarget getPairTarget() {
            return new PairTarget(pairingId);
        }
    }
}
