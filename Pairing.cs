using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            //LinkedMovement.Log("bId: " + baseID + ", tId: " + targetID);
        }

        public Pairing(GameObject baseGO, GameObject targetGO, string pId = null) {
            LinkedMovement.Log("Pairing contstructor with options");
            this.baseGO = baseGO;
            this.targetGO = targetGO;

            targetGO.transform.position = baseGO.transform.position;
            LinkedMovementController.AttachTargetToBase(baseGO.transform, targetGO.transform);

            //targetObject.transform.position = baseObject.transform.position;
            //AttachTargetToBase(baseObject.transform, targetObject.transform);

            //baseID = Guid.NewGuid().ToString();

            //LinkedMovement.Log($"base Id: {baseID}, target Id: {targetID}");
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
