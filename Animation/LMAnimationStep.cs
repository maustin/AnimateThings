using LinkedMovement.UI.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LinkedMovement.Animation {
    public class LMAnimationStep : SerializedRawObject {
        [Serialized]
        public string name = "";
        [Serialized]
        public float duration = 1f;
        [Serialized]
        public string ease = LMEase.Values.InOutQuad.ToString();
        [Serialized]
        public float startDelay = 0f;
        [Serialized]
        public float endDelay = 0f;

        [Serialized]
        public Vector3 targetPosition = Vector3.zero;
        [Serialized]
        public Vector3 targetRotation = Vector3.zero;
        [Serialized]
        public Vector3 targetScale = Vector3.zero;

        [Serialized]
        public List<Color> targetColors = null;

        [NonSerialized]
        public bool uiIsOpen = true;

        public static LMAnimationStep CreateInvertedStep(LMAnimationStep step) {
            var newStep = Duplicate(step);

            // TODO: Naming algo
            newStep.name = newStep.name + " inv";
            newStep.targetPosition = -newStep.targetPosition;
            newStep.targetRotation = -newStep.targetRotation;
            newStep.targetScale = -newStep.targetScale;

            return newStep;
        }

        public static LMAnimationStep Duplicate(LMAnimationStep step) {
            var newAnimationStep = new LMAnimationStep();
            newAnimationStep.name = step.name;
            newAnimationStep.duration = step.duration;
            newAnimationStep.ease = step.ease;
            newAnimationStep.startDelay = step.startDelay;
            newAnimationStep.endDelay = step.endDelay;
            newAnimationStep.targetPosition = step.targetPosition;
            newAnimationStep.targetRotation = step.targetRotation;
            newAnimationStep.targetScale = step.targetScale;
            newAnimationStep.targetColors = step.targetColors != null ? new List<Color>(step.targetColors) : null;
            return newAnimationStep;
        }

        public LMAnimationStep() { }

        public LMAnimationStep(LMAnimationParams animationParams) {
            if (animationParams.startingCustomColors != null) {
                targetColors = new List<Color>(animationParams.startingCustomColors);
            }
        }

        public override string ToString() {
            var sb = new StringBuilder("LMAnimationStep\n");
            sb.AppendLine("name: " + name);
            sb.AppendLine("duration: " + duration.ToString());
            sb.AppendLine("ease: " + ease);
            sb.AppendLine("startDelay: " + startDelay.ToString());
            sb.AppendLine("endDelay: " + endDelay.ToString());
            if (targetPosition != Vector3.zero)
                sb.AppendLine("targetPosition: " + targetPosition.ToString());
            if (targetRotation != Vector3.zero)
                sb.AppendLine("targetRotation: " + targetRotation.ToString());
            if (targetScale != Vector3.zero)
                sb.AppendLine("targetScale: " + targetScale.ToString());
            // TODO: Colors
            return sb.ToString();
        }
    }
}
