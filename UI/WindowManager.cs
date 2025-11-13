using RapidGUI;
using System.Collections.Generic;
using UnityEngine;

namespace LinkedMovement.UI {
    public class WindowManager {

        // TODO: Move out
        public enum WindowType {
            Information,
            Error,
            ConfirmAction,
            // NEW
            ModeDeterminationNew,
            ViewAnimationsNew,
            CreateAnimationNew,
            EditAnimationNew,
            ViewLinksNew,
            CreateLinkNew,
            EditLinkNew,
        }

        private List<LMWindow> activeWindows = new List<LMWindow>();
        private List<LMWindow> renderWindows;
        private bool dirtyActiveWindows = true;

        public WindowManager() {
            LMLogger.Debug("WindowManager constructor");
        }

        public void destroy() {
            LMLogger.Debug("WindowManager destructor");
        }

        public bool uiPresent() {
            foreach (LMWindow window in activeWindows) {
                if (!window.alwaysRender) return true;
            }
            return false;
        }

        public void createWindow(WindowType type, object data) {
            LMLogger.Debug($"WindowManager.createWindow {type.ToString()}");
            LMWindow window = LMWindowFactory.BuildWindow(type, data, this);
            if (window != null) {
                activeWindows.Add(window);
                dirtyActiveWindows = true;
            } else {
                LMLogger.Debug("Failed to create window");
            }
        }

        public bool hasWindowOfType(WindowType type) {
            return activeWindows.Exists(window => window.type == type);
        }

        public void DoGUI()
        {
            if (activeWindows.Count == 0) return;

            if (dirtyActiveWindows) {
                dirtyActiveWindows = false;

                renderWindows = new List<LMWindow>();
                var didFindNonOverlay = false;

                for (var i = activeWindows.Count - 1; i >= 0; i--) {
                    var window = activeWindows[i];
                    if (window.alwaysRender) {
                        renderWindows.Insert(0, window);
                    } else {
                        if (!didFindNonOverlay) {
                            didFindNonOverlay = true;
                            renderWindows.Insert(0, window);
                        }
                    }
                }
            }

            foreach (var window in renderWindows) {
                window.DoGUI();
            }
        }

        public void removeWindow(LMWindow window) {
            activeWindows.Remove(window);
            dirtyActiveWindows = true;
        }

        public void removeWindowOfType(WindowType type) {
            foreach (var window in activeWindows) {
                if (window.type == type) {
                    removeWindow(window);
                    break;
                }
            }
        }

        public int getNumberOfWindowsOfType(WindowType type) {
            int count = 0;
            foreach (var window in activeWindows) {
                if (window.type == type) {
                    count++;
                }
            }
            return count;
        }
    }
}
