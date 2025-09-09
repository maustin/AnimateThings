using LinkedMovement.UI.Utils;
using RapidGUI;
using static UnityEngine.GUILayout;

namespace LinkedMovement.UI.Content {
    internal class EditAnimatronicContent : LMWindowContent {
        private LinkedMovementController controller;

        private IDoGUI selectSubContent;
        private IDoGUI animateSubContent;

        // These are the temporary params
        private LMAnimationParams animationParams;
        private bool selectUIOpen = false;
        private bool animateUIOpen = false;

        public EditAnimatronicContent(Pairing pairing) {
            controller = LinkedMovement.GetController();

            controller.setTargetPairing(pairing);
            animationParams = controller.animationParams;

            foreach (var step in animationParams.animationSteps) {
                step.uiIsOpen = false;
            }

            selectSubContent = new EditAnimatronicSelectSubContent(pairing);
            animateSubContent = new EditAnimatronicAnimationSubContent();
        }

        public override void DoGUI() {
            base.DoGUI();

            using (Scope.Vertical()) {
                using (Scope.Horizontal()) {
                    Label("Name");
                    var newPairingName = RGUI.Field(animationParams.name);
                    if (newPairingName != animationParams.name) {
                        animationParams.name = newPairingName;
                        title = "Edit Animatronic: " + newPairingName;
                    }
                }
                Space(5f);

                using (Scope.Horizontal()) {
                    if (Button($"{(selectUIOpen ? "▼" : "►")} Targets", RGUIStyle.flatButtonLeft)) {
                        selectUIOpen = !selectUIOpen;
                        if (selectUIOpen) animateUIOpen = false;
                        controller.disableSelectionHandler();
                    }
                }
                if (selectUIOpen) {
                    selectSubContent.DoGUI();
                }
                using (Scope.Horizontal()) {
                    if (Button($"{(animateUIOpen ? "▼" : "►")} Animation", RGUIStyle.flatButtonLeft)) {
                        animateUIOpen = !animateUIOpen;
                        if (animateUIOpen) selectUIOpen = false;
                        controller.disableSelectionHandler();
                    }
                }
                if (animateUIOpen) {
                    animateSubContent.DoGUI();
                }
            }

            if (Button("Save Changes")) {
                windowManager.removeWindow(window);
                controller.saveChanges();
            }
        }

    }
}
