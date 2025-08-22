using LinkedMovement.UI.Utils;
using LinkedMovement.Utils;
using System.Text;
using UnityEngine;

namespace LinkedMovement {
    public class LMAnimationParams : SerializedRawObject {
        [Serialized]
        public string name = string.Empty;
        [Serialized]
        public Vector3 startingPosition = Vector3.zero;
        [Serialized]
        public Vector3 targetPosition = Vector3.zero;
        [Serialized]
        public Vector3 startingRotation = Vector3.zero;
        [Serialized]
        public Vector3 targetRotation = Vector3.zero;
        [Serialized]
        public bool isTriggerable = false;
        [Serialized]
        public float toDuration = 1f;
        [Serialized]
        public string toEase = LMEase.InOutQuad.ToString();
        [Serialized]
        public float fromDelay = 0f;
        [Serialized]
        public float fromDuration = 1f;
        [Serialized]
        public string fromEase = LMEase.InOutQuad.ToString();
        [Serialized]
        public float restartDelay = 0f;
        [Serialized]
        public bool useInitialStartDelay = false;
        [Serialized]
        public float initialStartDelayMin = 0f;
        [Serialized]
        public float initialStartDelayMax = 0f;

        public LMAnimationParams() {
            LinkedMovement.Log("LMAnimationParams base constructor");
        }

        // TODO: Can we do away with starting values now that tweens are all relative?
        public LMAnimationParams(Vector3 startingPosition, Vector3 startingRotation) {
            LinkedMovement.Log("LMAnimationParams constructor with starting position and rotation");
            this.startingPosition = startingPosition;
            this.startingRotation = startingRotation;
        }

        public override string ToString() {
            var sb = new StringBuilder("LMAnimationParams\n");
            sb.AppendLine("name: " + name);
            sb.AppendLine("startingPosition: " + startingPosition.ToString());
            sb.AppendLine("startingRotation: " + startingRotation.ToString());
            sb.AppendLine("targetPosition: " + targetPosition.ToString());
            sb.AppendLine("targetRotation: " + targetRotation.ToString());
            sb.AppendLine("isTriggerable: " + isTriggerable.ToString());
            sb.AppendLine("toDuration: " + toDuration.ToString());
            sb.AppendLine("toEase: " + toEase);
            sb.AppendLine("fromDelay: " + fromDelay.ToString());
            sb.AppendLine("fromDuration: " + fromDuration.ToString());
            sb.AppendLine("fromEase: " + fromEase);
            sb.AppendLine("restartDelay: " + restartDelay.ToString());
            sb.AppendLine("useInitialStartDelay: " + useInitialStartDelay.ToString());
            sb.AppendLine("initialStartDelayMin: " + initialStartDelayMin.ToString());
            sb.AppendLine("initialStartDelayMax: " + initialStartDelayMax.ToString());
            return sb.ToString();
        }
    }
}
