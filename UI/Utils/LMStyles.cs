using RapidGUI;
using UnityEngine;
using static UnityEngine.GUILayout;

namespace LinkedMovement.UI.Utils {
    public static class LMStyles {
        // TODO: Find a home for these 2
        public static bool GetIconEyeButton() {
            return GUILayout.Button(RGUIStyle.iconEyeContent, RGUIStyle.iconEyeButton, Width(24f), Height(20f));
        }
        public static bool GetIconEyeStrikeButton() {
            return GUILayout.Button(RGUIStyle.iconEyeStrikeContent, RGUIStyle.iconEyeButton, Width(24f), Height(20f));
        }

        public static GUIStyle HorizontalLine;
        private static void InitializeHorizontalLine() {
            HorizontalLine = new GUIStyle();
            HorizontalLine.normal.background = Texture2D.whiteTexture;
            HorizontalLine.margin = new RectOffset(4, 4, 4, 4);
            HorizontalLine.fixedHeight = 1;
        }

        static LMStyles() {
            InitializeHorizontalLine();
        }

        public static GUIStyle GetColoredBackgroundLabelStyle(Texture2D texture, Color color, GUIStyle fromStyle = null) {
            GUIStyle style = new GUIStyle(fromStyle ?? GUI.skin.label);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            style.normal.background = texture;
            return style;
        }

    }
}
