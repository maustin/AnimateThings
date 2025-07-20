using LinkedMovement.UI.Utils;
using RapidGUI;
using UnityEngine;
using static UnityEngine.GUILayout;

namespace LinkedMovement.UI.Content {
    internal class ModeDeterminationContent : IDoGUI {
        private LinkedMovementController controller;
        private string title;

        public ModeDeterminationContent(string title) {
            controller = LinkedMovement.GetController();
            this.title = title;
        }

        public void DoGUI() {
            using (Scope.Vertical()) {
                GUILayout.Label(title, RGUIStyle.popupTitle);
                Space(10f);

                if (Button("View Existing Animatronics")) {
                    LinkedMovement.Log("Clicked View Existing");
                    //controller.showExistingAnimatronics();
                    // TODO: Close window
                }

                Space(10f);

                if (Button("Create New Animatronic")) {
                    LinkedMovement.Log("Clicked Create New");
                    //controller.createNewAnimatronic();
                    // TODO: Close window
                }
            }
        }
    }
}
