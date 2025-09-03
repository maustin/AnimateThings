using LinkedMovement.UI.Utils;
using RapidGUI;
using UnityEngine;
using static UnityEngine.GUILayout;

namespace LinkedMovement.UI {
    public class LMWindowContent : IDoGUI {
        internal WindowManager windowManager;
        internal LMWindow window;
        internal string title;

        virtual public void DoGUI() {
            using (Scope.Vertical()) {
                GUILayout.Label(title, RGUIStyle.popupTitle);
                Space(10f);
            }
        }
    }
}
