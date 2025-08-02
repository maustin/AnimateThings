using LinkedMovement.UI.Components;
using LinkedMovement.UI.Utils;
using RapidGUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GUILayout;

namespace LinkedMovement.UI.Content {
    internal class CreateAnimatronicContent : LMWindowContent {
        private enum CreationSteps {
            Select,
            Assemble,
            Animate,
        }

        private LinkedMovementController controller;

        private CreationSteps creationStep = CreationSteps.Select;
        private string animatronicName = string.Empty;

        private IDoGUI selectSubContent;
        private IDoGUI assembleSubContent;
        private IDoGUI animateSubContent;

        public CreateAnimatronicContent() {
            controller = LinkedMovement.GetController();

            selectSubContent = new CreateAnimatronicSelectSubContent();
            //
            //
        }

        public override void DoGUI() {
            base.DoGUI();

            using (Scope.Vertical()) {
                using (Scope.Horizontal()) {
                    Label("Name");
                    animatronicName = RGUI.Field(animatronicName);
                }

                Space(5f);

                Label((creationStep == CreationSteps.Select ? "> " : "") + "Select Objects");
                Label((creationStep == CreationSteps.Assemble ? "> " : "") + "Assemble");
                Label((creationStep == CreationSteps.Animate ? "> " : "") + "Animate");

                Space(5f);
                HorizontalLine.DrawHorizontalLine(Color.grey);
                Space(5f);
            }

            if (creationStep == CreationSteps.Select) {
                selectSubContent.DoGUI();

                HorizontalLine.DrawHorizontalLine(Color.grey);

                using (Scope.Horizontal()) {
                    using (Scope.GuiEnabled(false)) {
                        Button("< Back", Width(65));
                    }

                    using (Scope.GuiEnabled(controller.targetObjects.Count > 0)) {
                        if (Button("Next >", Width(65))) {
                            // do thing
                        }
                    }
                }
            }
            if (creationStep == CreationSteps.Assemble) {
                //

                HorizontalLine.DrawHorizontalLine(Color.grey);
                // PREV NEXT
            }
            if (creationStep == CreationSteps.Animate) {
                //

                HorizontalLine.DrawHorizontalLine(Color.grey);
                // PREV NEXT
            }
        }
    }
}
