using LinkedMovement.Animation;
using LinkedMovement.Links;
using LinkedMovement.UI.Components;
using LinkedMovement.UI.Utils;
using RapidGUI;
using UnityEngine;
using static UnityEngine.GUILayout;

namespace LinkedMovement.UI.NewContent {
    internal class EditLinkContentNew : LMWindowContent {
        private LMController controller;

        private IDoGUI selectTargetsSubContent;

        public EditLinkContentNew(LMLink link) {
            controller = LinkedMovement.GetLMController();

            controller.editLink(link);

            selectTargetsSubContent = new SelectLinkTargetsSubContentNew();
        }

        public override void DoGUI() {
            base.DoGUI();

            using (Scope.Vertical()) {
                // Name
                using (Scope.Horizontal()) {
                    //InfoPopper.DoInfoPopper(LMStringKey.CREATE_NEW_ANIM_NAME);
                    // TODO: Info i
                    Label("Link name");
                    var newName = RGUI.Field(controller.currentLink.name);
                    if (newName != controller.currentLink.name) {
                        controller.currentLink.name = newName;
                    }
                }

                Space(5f);

                HorizontalLine.DrawHorizontalLine(Color.grey);

                selectTargetsSubContent.DoGUI();

                FlexibleSpace();

                HorizontalLine.DrawHorizontalLine(Color.grey);

                using (Scope.Horizontal()) {
                    var canFinish = controller.currentLink.isValid();
                    FlexibleSpace();
                    using (Scope.GuiEnabled(canFinish)) {
                        if (Button("Save ✓", Width(65))) {
                            controller.commitEdit();

                            // TODO: Can this call be moved to LMWindowContent?
                            windowManager.removeWindow(window);
                        }
                    }
                }
            }
        }
    }
}
