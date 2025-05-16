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
    }
}
