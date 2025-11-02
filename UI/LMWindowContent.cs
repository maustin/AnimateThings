using RapidGUI;
using UnityEngine;
using static UnityEngine.GUILayout;

namespace LinkedMovement.UI {
    public class LMWindowContent : IDoGUI {
        internal WindowManager windowManager;
        internal LMWindow window;
        internal string title;

        public void RenderGUI() {
            var originalScrollBackgroundStyle = GUI.skin.verticalScrollbar;
            var originalScrollThumbStyle = GUI.skin.verticalScrollbarThumb;

            GUI.skin.verticalScrollbar = RGUIStyle.scrollBackground;
            GUI.skin.verticalScrollbarThumb = RGUIStyle.scrollThumb;

            Space(8f);
            using (new GUILayout.VerticalScope(RGUIStyle.popupWindowContentNew)) {
                using (new GUILayout.VerticalScope(RGUIStyle.popupWindowContentInnerNew)) {
                    DoGUI();
                }
            }

            GUI.skin.verticalScrollbar = originalScrollBackgroundStyle;
            GUI.skin.verticalScrollbarThumb = originalScrollThumbStyle;
        }

        virtual public void DoGUI() {}
    }
}
