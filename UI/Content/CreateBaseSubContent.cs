using LinkedMovement.UI.Utils;
using RapidGUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GUILayout;

namespace LinkedMovement.UI.Content {
    internal class CreateBaseSubContent : IDoGUI {
        private LinkedMovementController controller;

        public CreateBaseSubContent() {
            controller = LinkedMovement.GetController();
        }

        public void DoGUI() {
            using (Scope.Vertical()) {
                GUILayout.Label("Assemble", RGUIStyle.popupTitle);

                Space(10f);

                // TODO
                bool hasBase = false;
                string generateButtonLabel = hasBase ? "Re-Generate Origin" : "Generate Origin";
                if (Button(generateButtonLabel)) {
                    // TODO
                }

                Space(10f);

                // position

                // offset
            }
        }
    }
}
