using LinkedMovement.UI.Utils;
using RapidGUI;
using UnityEngine;
using static UnityEngine.GUILayout;

namespace LinkedMovement.UI.NewContent {
    internal class SelectLinkParentSubContentNew : IDoGUI {
        private LMController controller;

        public SelectLinkParentSubContentNew() {
            controller = LinkedMovement.GetLMController();
        }

        public void DoGUI() {
            using (Scope.Vertical()) {
                Label(LMStringSystem.GetText(LMStringKey.CREATE_LINK_PARENT_INTRO), RGUIStyle.popupTextNew);

                Space(5f);

                var parentBO = controller.currentLink.getParentBuildableObject();
                var hasParent = parentBO != null;

                if (hasParent) {
                    using (Scope.Horizontal()) {
                        var name = parentBO.getName();
                        Label(name, RGUIStyle.popupTextNew);
                        if (Button("✕", RGUIStyle.roundedFlatButton, Width(40f))) {
                            controller.currentLink.removeParentObject();
                        }
                    }
                    Space(5f);
                }

                //Space(5f);

                using (Scope.GuiEnabled(!hasParent)) {
                    if (Button("Select Parent", RGUIStyle.roundedFlatButton)) {
                        GUI.FocusControl(null);
                        controller.currentLink.startPickingParent();
                    }
                }

                Space(3f);
            }
        }
    }
}
