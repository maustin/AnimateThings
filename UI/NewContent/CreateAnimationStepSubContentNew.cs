using LinkedMovement.Animation;
using LinkedMovement.UI.Utils;
using RapidGUI;
using UnityEngine;
using static UnityEngine.GUILayout;

namespace LinkedMovement.UI.NewContent {
    internal class CreateAnimationStepSubContentNew : IDoGUI {
        private LMController controller;
        private LMAnimationParams animationParams;
        private LMAnimationStep animationStep;
        private int stepIndex;

        public CreateAnimationStepSubContentNew(LMAnimationParams animationParams, LMAnimationStep animationStep, int stepIndex) {
            controller = LinkedMovement.GetLMController();
            this.animationParams = animationParams;
            this.animationStep = animationStep;
            this.stepIndex = stepIndex;
        }

        public void DoGUI() {
            using (Scope.Vertical()) {
                using (Scope.Horizontal()) {
                    if (Button($"{(animationStep.uiIsOpen ? "▼" : "►")} {(stepIndex + 1).ToString()} : {(animationStep.name == "" ? "Step" : animationStep.name)} ", RGUIStyle.flatButtonLeft)) {
                        animationStep.uiIsOpen = !animationStep.uiIsOpen;
                    }
                    if (Button("↑", Width(26f))) {
                        LinkedMovement.Log("Move AnimationStep UP");
                        var didChange = animationParams.moveAnimationStepUp(animationStep);
                        if (didChange)
                            controller.currentAnimationUpdated();
                    }
                    if (Button("↓", Width(26f))) {
                        LinkedMovement.Log("Move AnimationStep DOWN");
                        var didChange = animationParams.moveAnimationStepDown(animationStep);
                        if (didChange)
                            controller.currentAnimationUpdated();
                    }
                    Label("|", RGUIStyle.dimText, Width(3f));
                    if (Button("+Dup", Width(42f))) {
                        LinkedMovement.Log("Add duplicate step");
                        animationParams.addDuplicateAnimationStep(animationStep);
                        controller.currentAnimationUpdated();
                    }
                    if (Button("+Inv", Width(40f))) {
                        LinkedMovement.Log("Add inverse step");
                        animationParams.addInverseAnimationStep(animationStep);
                        controller.currentAnimationUpdated();
                    }
                    Label("|", RGUIStyle.dimText, Width(3f));
                    if (Button("✕", Width(25f))) {
                        LinkedMovement.Log("Delete AnimationStep");
                        animationParams.deleteAnimationStep(animationStep);
                        controller.currentAnimationUpdated();
                    }
                }

                if (animationStep.uiIsOpen) {
                    using (new GUILayout.VerticalScope(RGUIStyle.animationStep)) {
                        Space(-17f);
                        renderStepDetails();
                    }
                    Space(5f);
                } else {
                    Space(-10f);
                }
            }
        }

        private void renderStepDetails() {
            using (Scope.Horizontal()) {
                InfoPopper.DoInfoPopper(LMStringKey.ANIMATE_STEP_NAME);
                Label("Step name");
                animationStep.name = RGUI.Field(animationStep.name);
            }

            using (Scope.Horizontal()) {
                InfoPopper.DoInfoPopper(LMStringKey.ANIMATE_START_DELAY);
                GUILayout.Label("Start delay");
                var newStartDelay = RGUI.Field(animationStep.startDelay);
                if (animationStep.startDelay != newStartDelay) {
                    animationStep.startDelay = newStartDelay;
                    controller.currentAnimationUpdated();
                }
            }

            using (Scope.Horizontal()) {
                InfoPopper.DoInfoPopper(LMStringKey.ANIMATE_DURATION);
                GUILayout.Label("Duration");
                var newDuration = RGUI.Field(animationStep.duration);
                if (animationStep.duration != newDuration) {
                    animationStep.duration = newDuration;
                    controller.currentAnimationUpdated();
                }
            }

            using (Scope.Horizontal()) {
                InfoPopper.DoInfoPopper(LMStringKey.ANIMATE_EASE);
                GUILayout.Label("Ease");
                var newEase = RGUI.SelectionPopup(animationStep.ease, LMEase.Names);
                if (animationStep.ease != newEase) {
                    animationStep.ease = newEase;
                    controller.currentAnimationUpdated();
                }
            }

            using (Scope.Horizontal()) {
                InfoPopper.DoInfoPopper(LMStringKey.ANIMATE_CHANGE_POSITION);
                GUILayout.Label("Position change");
                var newPosition = RGUI.Field(animationStep.targetPosition);
                if (!animationStep.targetPosition.Equals(newPosition)) {
                    animationStep.targetPosition = newPosition;
                    controller.currentAnimationUpdated();
                }
            }

            using (Scope.Horizontal()) {
                InfoPopper.DoInfoPopper(LMStringKey.ANIMATE_CHANGE_ROTATION);
                GUILayout.Label("Rotation change");
                var newRotation = RGUI.Field(animationStep.targetRotation);
                if (!animationStep.targetRotation.Equals(newRotation)) {
                    animationStep.targetRotation = newRotation;
                    controller.currentAnimationUpdated();
                }
            }

            using (Scope.Horizontal()) {
                InfoPopper.DoInfoPopper(LMStringKey.ANIMATE_CHANGE_SCALE);
                GUILayout.Label("Scale change");
                var newScale = RGUI.Field(animationStep.targetScale);
                if (!animationStep.targetScale.Equals(newScale)) {
                    animationStep.targetScale = newScale;
                    controller.currentAnimationUpdated();
                }
            }

            using (Scope.Horizontal()) {
                InfoPopper.DoInfoPopper(LMStringKey.ANIMATE_END_DELAY);
                GUILayout.Label("End delay");
                var newEndDelay = RGUI.Field(animationStep.endDelay);
                if (animationStep.endDelay != newEndDelay) {
                    animationStep.endDelay = newEndDelay;
                    controller.currentAnimationUpdated();
                }
            }
        }
    }
}
