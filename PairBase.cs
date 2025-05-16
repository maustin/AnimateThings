using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedMovement
{
    public class PairBase : SerializedRawObject {
        [Serialized]
        public string pairId;

        public bool IsBase {
            get => true;
        }

        public PairBase() {
            LinkedMovement.Log("PairBase DEFAULT CONTRUCTOR: " + pairId + ", IsBase: " + IsBase);
            LinkedMovement.GetController().addPairBase(this);
        }

        public PairBase(string pId) {
            pairId = pId;
        }

        //override protected void Awake() {
        //    base.Awake();
        //    LinkedMovement.Log("PairBase Awake");
        //}

        //public void Start() {
        //    LinkedMovement.Log("PairBase Start");
        //}
    }
}
