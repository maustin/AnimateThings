using LinkedMovement.BLERGUI.Utils;
using RapidGUI;
using System;
using UnityEngine;
using static UnityEngine.GUILayout;

namespace LinkedMovement.BLERGUI.InGame {
    internal sealed class MainContent : IDoGUI {
        private LinkedMovementController controller;
        private string[] selectionModeNames = { Selection.Mode.Individual.ToString(), Selection.Mode.Box.ToString() };
        private Selection.Mode[] selectionModes = { Selection.Mode.Individual, Selection.Mode.Box };
        private int selectedSelectionMode = 0;
        private Vector2 targetsScrollPosition;

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
            if (controller.baseObject != null && controller.targetObjects.Count > 0) {
                Space(10f);
                if (Button("Join Objects!"))
                    controller.joinObjects();
            }
        }
    }
}
