using LinkedMovement.BLERGUI.Utils;
using RapidGUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GUILayout;

namespace LinkedMovement.BLERGUI.InGame {
    internal sealed class MainContent : IDoGUI {
        private LinkedMovementController controller;
        private string[] selectionModeNames = { Selection.Mode.Individual.ToString(), Selection.Mode.Box.ToString() };
        private Selection.Mode[] selectionModes = { Selection.Mode.Individual, Selection.Mode.Box };
        private int selectedSelectionMode = 0;
        private Vector2 targetsScrollPosition;
        private string selectedBlueprintName;

        static private string[] GetBlueprintNames(List<BlueprintFile> blueprints) {
            var names = new List<string>();
            foreach (BlueprintFile blueprint in blueprints) {
                names.Add(blueprint.getName());
            }
            return names.ToArray();
        }

        public MainContent(LinkedMovementController controller) {
            this.controller = controller;
        }

        public void DoGUI() {
            if (controller == null) {
                LinkedMovement.Log("NO CONTROLLER SET!");
                return;
            }
            using (Scope.Vertical()) {
                ShowBaseSelect();
                Space(10f);
                ShowTargetsSelect();
                ShowJoin();
            }
        }

        private void ShowBaseSelect() {
            using (Scope.Vertical()) {
                using (Scope.Horizontal()) {
                    Label("Base");
                    if (Button("Select", Width(65))) {
                        controller.pickBaseObject();
                    }
                }

                var baseObject = controller.baseObject;
                if (baseObject != null) {
                    using (Scope.Horizontal()) {
                        Label(baseObject.getName());
                        if (Button("Clear", Width(65)))
                            controller.clearBaseObject();
                    }
                }
            }
        }

        private void ShowTargetsSelect() {
            using (Scope.Vertical()) {
                using (Scope.Horizontal()) {
                    Label("Targets");
                    if (Button("Select", Width(65))) {
                        controller.pickTargetObject(selectionModes[selectedSelectionMode]);
                    }
                }

                using (Scope.Horizontal()) {
                    Label("Selection mode");
                    selectedSelectionMode = Toolbar(selectedSelectionMode, selectionModeNames);
                }

                // TODO: Calculate less often
                var prints = BlueprintManager.Instance.getAllBlueprints();
                //LinkedMovement.Log("# prints: " + prints.Count);
                var decoPrints = LinkedMovementController.FindDecoBlueprints(prints);
                //LinkedMovement.Log("# deco prints: " + decoPrints.Count);
                var decoPrintNames = GetBlueprintNames(decoPrints);
                //LinkedMovement.Log("# deco print names: " + decoPrintNames.Length);

                using (Scope.Horizontal()) {
                    if (selectedBlueprintName != null && controller.selectedBlueprintName != selectedBlueprintName) {
                        controller.selectedBlueprintName = selectedBlueprintName;
                        selectedBlueprintName = null;
                    }
                    Label("Blueprint");
                    //selectedBlueprintName = RGUI.SelectionPopup(selectedBlueprintName, decoPrintNames);
                    selectedBlueprintName = RGUI.SelectionPopup(controller.selectedBlueprintName, decoPrintNames);
                }

                var targetObjects = controller.targetObjects;
                targetsScrollPosition = BeginScrollView(targetsScrollPosition);
                foreach (var targetObject in targetObjects) {
                    using (Scope.Horizontal()) {
                        Label(targetObject.getName());
                        if (Button("Clear", Width(65)))
                            controller.clearTargetObject(targetObject);
                    }
                }
                EndScrollView();
            }
        }

        private void ShowJoin() {
            if (controller.baseObject != null && (controller.targetObjects.Count > 0 || controller.selectedBlueprintName != null)) {
                Space(10f);
                if (Button("Join Objects!"))
                    controller.joinObjects();
            }
        }
    }
}
