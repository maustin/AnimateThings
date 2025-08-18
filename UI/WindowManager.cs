using LinkedMovement.UI.InGame;
using RapidGUI;
using System.Collections.Generic;
using UnityEngine;

namespace LinkedMovement.UI {
    public class WindowManager {

        // TODO: Move out to class
        public enum WindowType {
            ModeDetermination,
            CreateNewAnimatronic,
            ShowExistingAnimatronics,
            EditAnimatronic,
            EditAnimation,
            Information,
            Error,
        }

        private List<LMWindow> activeWindows = new List<LMWindow>();

        public WindowManager() {
            LinkedMovement.Log("Create WindowManager");
        }

        public void destroy() {
            LinkedMovement.Log("Destroy WindowManager");
        }

        public bool uiPresent() {  return activeWindows.Count > 0; }

        public void createWindow(WindowType type, object data) {
            LMWindow window = LMWindowFactory.BuildWindow(type, data, this);
            if (window != null) {
                activeWindows.Add(window);
            } else {
                LinkedMovement.Log("Failed to create window");
            }
        }

        public bool hasWindowOfType(WindowType type) {
            return activeWindows.Exists(window => window.type == type);
        }

        public void DoGUI()
        {
            // TODO: Do we need to clone the list to prevent reorder?
            for (var i = 0; i < activeWindows.Count; i++) {
                if (activeWindows[i].alwaysRender || i == activeWindows.Count - 1)
                    activeWindows[i].DoGUI();
            }
        }

        public void removeWindow(LMWindow window) {
            activeWindows.Remove(window);
        }

        public void removeWindowOfType(WindowType type) {
            foreach (var window in activeWindows) {
                if (window.type == type) {
                    removeWindow(window);
                    break;
                }
            }
        }
    }
}
