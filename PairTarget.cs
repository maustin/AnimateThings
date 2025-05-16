using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedMovement
{
    public class PairTarget : SerializedRawObject {
        [Serialized]
        public string pairId;

        public bool IsBase {
            get => false;
        }

        public PairTarget() {
            LinkedMovement.Log("PairTarget DEFAULT CONSTRUCTOR: " + pairId + ", IsBase: " + IsBase);
            LinkedMovement.GetController().addPairTarget(this);
        }

        public PairTarget(string pId) {
            pairId = pId;
        }

        //override protected void Awake() {
        //    base.Awake();
        //    LinkedMovement.Log("PairTarget Awake");
        //}

        //public void Start() {
        //    LinkedMovement.Log("PairTarget Start");
        //}
    }
}
