using LinkedMovement.UI.Utils;
using RapidGUI;
using static UnityEngine.GUILayout;

namespace LinkedMovement.UI.NewContent {
    internal class ModeDeterminationContentNew : LMWindowContent {
        public ModeDeterminationContentNew() {}

        override public void DoGUI() {
            base.DoGUI();

            using (Scope.Vertical()) {
                //Label(LMStringSystem.GetText(LMStringKey.MODE_SELECT), RGUIStyle.popupTextNew);
                //Space(5f);

                if (Button("Create Animation", RGUIStyle.roundedFlatButton)) {
                    LMLogger.Debug("Clicked Create Animation");
                    windowManager.removeWindow(this.window);
                    windowManager.createWindow(WindowManager.WindowType.CreateAnimationNew, null);
                }

                Space(3f);

                if (Button("View Animations", RGUIStyle.roundedFlatButton)) {
                    LMLogger.Debug("Clicked View Animations");
                    windowManager.removeWindow(this.window);
                    windowManager.createWindow(WindowManager.WindowType.ViewAnimationsNew, null);
                }

                Space(15f);

                if (Button("Create Link", RGUIStyle.roundedFlatButton)) {
                    LMLogger.Debug("Clicked Create Link");
                    windowManager.removeWindow(this.window);
                    windowManager.createWindow(WindowManager.WindowType.CreateLinkNew, null);
                }

                Space(3f);

                if (Button("View Links", RGUIStyle.roundedFlatButton)) {
                    LMLogger.Debug("Clicked View Links");
                    windowManager.removeWindow(this.window);
                    windowManager.createWindow(WindowManager.WindowType.ViewLinksNew, null);
                }
            }
        }
    }
}
