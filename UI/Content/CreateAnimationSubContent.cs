using DG.Tweening;
using LinkedMovement.UI.Components;
using LinkedMovement.UI.Utils;
using RapidGUI;
using UnityEngine;
using static UnityEngine.GUILayout;

namespace LinkedMovement.UI.Content {
    internal class CreateAnimationSubContent : IDoGUI {
        private LinkedMovementController controller;
        private BuildableObject originBO;
        private LMAnimationParams animationParams;
        private Sequence sequence;

        public CreateAnimationSubContent() {
            controller = LinkedMovement.GetController();
        }

        public void DoGUI() {
            originBO = controller.originObject;
            animationParams = controller.animationParams;

            using (Scope.Vertical()) {
                GUILayout.Label("Animate", RGUIStyle.popupTitle);

                Space(10f);

                using (Scope.Horizontal()) {
                    GUILayout.Label("Is Triggerable");
                    var newIsTriggerable = RGUI.Field(animationParams.isTriggerable);
                    if (newIsTriggerable != animationParams.isTriggerable) {
                        animationParams.isTriggerable = newIsTriggerable;
                        rebuildSequence();
                    }
                }

                using (Scope.GuiEnabled(false)) {
                    GUILayout.Label("Origin Position: " + originBO.transform.position.ToString());
                }
                using (Scope.Horizontal()) {
                    GUILayout.Label("Target Position Offset");
                    var newTargetPosition = RGUI.Field(animationParams.targetPosition);
                    if (!animationParams.targetPosition.Equals(newTargetPosition)) {
                        animationParams.targetPosition = newTargetPosition;
                        rebuildSequence();
                    }
                }

                // TODO: Target rotation
                using (Scope.GuiEnabled(false)) {
                    GUILayout.Label("Origin Rotation: " + originBO.transform.rotation.ToString());
                }
                using (Scope.Horizontal()) {
                    GUILayout.Label("Target Rotation Offset");
                }

                // TO Duration
                using (Scope.Horizontal()) {
                    GUILayout.Label("Animate To Duration");
                }

                // TO Ease
                using (Scope.Horizontal()) {
                    GUILayout.Label("Animate To Easing");
                }

                HorizontalLine.DrawHorizontalLine(Color.grey);

                // FROM Delay
                using (Scope.Horizontal()) {
                    GUILayout.Label("Pause at Target Duration");
                }

                // FROM Duration
                using (Scope.Horizontal()) {
                    GUILayout.Label("Return Duration");
                }

                // FROM Ease
                using (Scope.Horizontal()) {
                    GUILayout.Label("Return Easing");
                }

                GUILayout.FlexibleSpace();
            }
        }

        private void killSequence() {
            if (sequence == null) {
                LinkedMovement.Log("No sequence to kill");
                return;
            }

            // TODO: Reset?
            sequence.Kill();
            originBO.transform.position = animationParams.startingPosition;
            // TODO: rotation
            sequence = null;
        }

        private void rebuildSequence(bool isSaving = false) {
            LinkedMovement.Log("rebuildSequence");
            killSequence();

            sequence = DOTween.Sequence();
            var toTween = DOTween.To(() => originBO.transform.position, x => originBO.transform.position = x, animationParams.startingPosition + animationParams.targetPosition, animationParams.toDuration);
            //toTween.SetEase()
            // delay
            var fromTween = DOTween.To(() => originBO.transform.position, x => originBO.transform.position = x, animationParams.startingPosition, animationParams.fromDuration);
            //fromTween.SetEase()
            // delay
            sequence.Append(toTween);
            sequence.Append(fromTween);
            sequence.SetLoops(-1);
            //if (isSaving && animationParams.isTriggerable) {
            //    sequence.SetLoops(0);
            //    sequence.Pause();
            //} else {
            //    sequence.SetLoops(-1);
            //}
        }
    }
}
