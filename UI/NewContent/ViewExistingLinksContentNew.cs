using LinkedMovement.UI.Utils;
using LinkedMovement.Utils;
using RapidGUI;
using System;
using System.Linq;
using UnityEngine;
using static UnityEngine.GUILayout;

namespace LinkedMovement.UI.NewContent {
    internal class ViewExistingLinksContentNew : LMWindowContent {
        private LMController controller;
        private Vector2 targetsScrollPosition;
        private string searchText = string.Empty;

        public ViewExistingLinksContentNew() {
            controller = LinkedMovement.GetLMController();
        }

        public override void DoGUI() {
            base.DoGUI();

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

                var links = controller.getLinks().Where(l => LMUtils.FuzzyMatch(searchText, l.name)).OrderBy(l => l.name, StringComparer.OrdinalIgnoreCase);
                targetsScrollPosition = BeginScrollView(targetsScrollPosition, Height(400f));

                foreach (var link in links) {
                    using (Scope.Horizontal()) {
                        var forceShowHighlight = link.ForceShowHighlight;
                        if (forceShowHighlight) {
                            if (LMStyles.GetIconEyeButton()) {
                                link.ForceShowHighlight = false;
                            }
                        } else {
                            if (LMStyles.GetIconEyeStrikeButton()) {
                                link.ForceShowHighlight = true;
                            }
                        }
                        Space(2f);

                        if (Button(link.name, RGUIStyle.roundedFlatButtonLeft)) {
                            windowManager.createWindow(WindowManager.WindowType.EditLinkNew, link);
                        }
                        Space(3f);
                        if (Button("✕", RGUIStyle.roundedFlatButton, Width(40f))) {
                            controller.queueLinkToRemove(link);
                        }
                    }
                }

                EndScrollView();
            }
        }
    }
}
