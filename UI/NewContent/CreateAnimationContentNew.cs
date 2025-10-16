using LinkedMovement.UI.Components;
using LinkedMovement.UI.Utils;
using RapidGUI;
using UnityEngine;
using static UnityEngine.GUILayout;

namespace LinkedMovement.UI.NewContent {
    public class CreateAnimationContentNew : LMWindowContent {
        private LMController controller;

        private IDoGUI selectSubContent;
        private IDoGUI animateSubContent;

        public CreateAnimationContentNew() {
            controller = LinkedMovement.GetLMController();
            controller.editAnimation();

            selectSubContent = new SelectAnimationTargetSubContentNew();
            animateSubContent = new CreateAnimationSubContentNew();
        }

        public override void DoGUI() {
            if (controller.currentAnimation == null) {
                return;
            }

            base.DoGUI();

            using (Scope.Vertical()) {
                // Select target subcontent
                selectSubContent.DoGUI();

                // Animation subcontent
                var canEditAnimation = controller.currentAnimation.hasTarget();
                if (canEditAnimation) {
                    HorizontalLine.DrawHorizontalLine(Color.grey);
                    animateSubContent.DoGUI();
                }

                FlexibleSpace();

                HorizontalLine.DrawHorizontalLine(Color.grey);

                using (Scope.Horizontal()) {
                    var canFinish = false;
                    FlexibleSpace();
                    using (Scope.GuiEnabled(canFinish)) {
                        if (Button("Save ✓", Width(65))) {
                            // TODO: Can this call be moved to LMWindowContent?
                            windowManager.removeWindow(window);

                            controller.commitEdit();
                        }
                    }
                }
            }
        }
    }
}
