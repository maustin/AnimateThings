using LinkedMovement.UI.Utils;
using LinkedMovement.Utils;
using RapidGUI;
using UnityEngine;
using static UnityEngine.GUILayout;

namespace LinkedMovement.UI.NewContent {
    internal class ViewExistingAnimationsContentNew : LMWindowContent {
        private LMController controller;
        private Vector2 targetsScrollPosition;

        public ViewExistingAnimationsContentNew() {
            controller = LinkedMovement.GetLMController();
        }

        public override void DoGUI() {
            base.DoGUI();

            using (Scope.Vertical()) {
                var animations = controller.getAnimations();
                targetsScrollPosition = BeginScrollView(targetsScrollPosition, Height(400f));

                foreach (var animation in animations) {
                    using (Scope.Horizontal()) {
                        if (Button(animation.name, RGUIStyle.flatButtonLeft)) {
                            windowManager.removeWindow(this.window);
                            windowManager.createWindow(WindowManager.WindowType.EditAnimationNew, animation);
                        }
                        using (Scope.GuiEnabled(false)) {
                            if (Button("✕", Width(40f))) {
                                // TODO delete animation
                            }
                        }
                    }
                }

                EndScrollView();
            }
        }
    }
}
