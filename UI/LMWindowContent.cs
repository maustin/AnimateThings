using RapidGUI;
using UnityEngine;
using static UnityEngine.GUILayout;

namespace LinkedMovement.UI {
    public class LMWindowContent : IDoGUI {
        internal WindowManager windowManager;
        internal LMWindow window;
        internal string title;

        public void RenderGUI() {
            Space(8f);
            using (new GUILayout.VerticalScope(RGUIStyle.popupWindowContentNew)) {
                using (new GUILayout.VerticalScope(RGUIStyle.popupWindowContentInnerNew)) {
                    DoGUI();
                }
            }
        }

        virtual public void DoGUI() {}
    }
}
