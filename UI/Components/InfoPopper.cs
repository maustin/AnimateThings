using RapidGUI;
using static UnityEngine.GUILayout;

namespace LinkedMovement.UI.Utils {
    public class InfoPopper {
        public static void DoInfoPopper(LMStringKey key, params object[] args) {
            if (Button("ⓘ", RGUIStyle.infoPopperButtonNew, Width(15f))) {
                LinkedMovement.GetLMController().windowManager.createWindow(WindowManager.WindowType.Information, LMStringSystem.GetText(key, args));
            }
        }
    }
}
