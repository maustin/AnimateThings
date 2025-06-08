using LinkedMovement.UI.Utils;
using LinkedMovement.Utils;
using RapidGUI;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GUILayout;

namespace LinkedMovement.UI.InGame {
    internal sealed class ExistingLinksContent : IDoGUI {
        private LinkedMovementController controller;
        private Vector2 scrollPosition;

        private Dictionary<Pairing, string> selectedPairingsAndNames;

        public ExistingLinksContent() {
            controller = LinkedMovement.GetController();
            selectedPairingsAndNames = new Dictionary<Pairing, string>();
        }

        public void DoGUI() {
            var pairings = controller.getPairings();
            scrollPosition = BeginScrollView(scrollPosition, GUILayout.Height(500f));
            foreach (var pairing in pairings) {
                buildPairingUI(pairing);
            }
            EndScrollView();
        }

        private void buildPairingUI(Pairing pairing) {
            using (Scope.Vertical()) {
                var name = pairing.getPairingName();
                using (Scope.Horizontal()) {
                    if (GUILayout.Button(name, RGUIStyle.flatButtonLeft)) {
                        LinkedMovement.Log("Click Pairing");
                        var hasPairingNameField = selectedPairingsAndNames.ContainsKey(pairing);
                        if (hasPairingNameField == true) {
                            selectedPairingsAndNames.Remove(pairing);
                        } else {
                            selectedPairingsAndNames.Add(pairing, name);
                        }
                    }
                }
                using (Scope.Horizontal()) {
                    var hasPairingNameField = selectedPairingsAndNames.ContainsKey(pairing);
                    if (hasPairingNameField == true) {
                        var origPairName = selectedPairingsAndNames[pairing];
                        var newPairName = RGUI.Field(origPairName, "Pair Name: ");
                        if (newPairName != origPairName) {
                            LinkedMovement.Log("Update pair name: " + newPairName);
                            selectedPairingsAndNames[pairing] = newPairName;

                            pairing.updatePairingName(newPairName);
                        }
                    }
                }
                using (Scope.Horizontal()) {
                    GUILayout.Space(10f);
                    var baseName = TAUtils.GetGameObjectBuildableName(pairing.baseGO);
                    if (GUILayout.Button(baseName, RGUIStyle.flatButtonLeft)) {
                        LinkedMovement.Log("Focus base " + baseName);
                        GameController.Instance.cameraController.focusOn(pairing.baseGO.transform.position);
                    }
                }
                foreach (var target in pairing.targetGOs) {
                    var targetName = TAUtils.GetGameObjectBuildableName(target);
                    using (Scope.Horizontal()) {
                        GUILayout.Space(20f);
                        GUILayout.Label(targetName);
                    }
                }
                using (Scope.Vertical()) {
                    GUILayout.Space(5f);
                }
            }
        }
    }
}
