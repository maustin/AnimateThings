using LinkedMovement.UI.Utils;
using UnityEngine;

namespace LinkedMovement.UI.Components {
    static class HorizontalLine {
        public static void DrawHorizontalLine(Color color) {
            var c = GUI.color;
            GUI.color = color;
            GUILayout.Box(GUIContent.none, LMStyles.HorizontalLine);
            GUI.color = c;
        }
    }
}
