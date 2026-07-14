using LinkedMovement.UI.Utils;
using LinkedMovement.Utils;
using RapidGUI;
using System;
using System.Linq;
using UnityEngine;
using static UnityEngine.GUILayout;

namespace LinkedMovement.UI.NewContent {
    internal class ViewExistingAnimationsContentNew : LMWindowContent {
        private LMController controller;
        private Vector2 targetsScrollPosition;
        private string searchText = string.Empty;

        public ViewExistingAnimationsContentNew() {
            controller = LinkedMovement.GetLMController();
        }

        public override void DoGUI() {
            base.DoGUI();

            //using (Scope.Vertical()) {
            //    if (Button("Restart All Animations", RGUIStyle.roundedFlatButton)) {
            //        controller.restartAllAnimations();
            //    }
            //}

            using (Scope.Vertical()) {
                using (Scope.Horizontal()) {
                    Label("Search", RGUIStyle.popupTextNew);
                    searchText = RGUI.Field(searchText);
                    Space(3f);
                    if (Button("✕", RGUIStyle.roundedFlatButton, Width(40f))) {
                        searchText = string.Empty;
                        GUIUtility.keyboardControl = 0; // drop focus so the cleared field redraws immediately
                    }
                }
                Space(5f);

                var animations = controller.getAnimations().Where(a => LMUtils.FuzzyMatch(searchText, a.name)).OrderBy(a => a.name, StringComparer.OrdinalIgnoreCase);
                targetsScrollPosition = BeginScrollView(targetsScrollPosition, Height(400f));

                foreach (var animation in animations) {
                    using (Scope.Horizontal()) {
                        if (Button("@", RGUIStyle.roundedFlatButton, Width(26f))) {
                            LMUtils.JumpToObject(animation.targetGameObject);
                        }
                        Space(2f);

                        var forceShowHighlight = animation.ForceShowHighlight;
                        if (forceShowHighlight) {
                            if (LMStyles.GetIconEyeButton()) {
                                animation.ForceShowHighlight = false;
                            }
                        } else {
                            if (LMStyles.GetIconEyeStrikeButton()) {
                                animation.ForceShowHighlight = true;
                            }
                        }
                        Space(2f);

                        if (Button(animation.name, RGUIStyle.roundedFlatButtonLeft)) {
                            windowManager.createWindow(WindowManager.WindowType.EditAnimationNew, animation);
                        }
                        Space(3f);
                        if (Button("✕", RGUIStyle.roundedFlatButton, Width(40f))) {
                            controller.queueAnimationToRemove(animation);
                        }
                    }
                }

                EndScrollView();
            }
        }
    }
}
