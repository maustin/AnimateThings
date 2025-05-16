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
    }
}
