using UnityEngine;

namespace RapidGUI
{
    public static class RGUIStyle
    {
        public static GUIStyle flatButton;
        public static GUIStyle flatButtonLeft;
        public static GUIStyle popupFlatButton;
        public static GUIStyle popupFlatButtonSelected;
        public static GUIStyle popup;
        public static GUIStyle popupTitle;
        public static GUIStyle darkWindow;
        public static GUIStyle alignLeftBox;

        public static GUIStyle popupWindowNew;
        public static GUIStyle popupWindowTitleNew;
        public static GUIStyle popupWindowContentNew;

        public static GUIStyle warningLabel;
        public static GUIStyle warningLabelNoStyle;

        public static GUIStyle animationStep;
        public static GUIStyle dimText;
        public static GUIStyle infoText;

        // GUIStyleState.background will be null 
        // if it set after secound scene load and don't use a few frame
        // to keep textures, set it to other member. at unity2019
        private static Texture2D flatButtonTex;
        private static Texture2D flatButtonLeftTex;
        private static Texture2D popupTex;
        private static Texture2D popupTitleTex;
        private static Texture2D darkWindowTexNormal;
        private static Texture2D darkWindowTexOnNormal;
        private static Texture2D animationStepTex;

        private static Texture2D popupTexNew;
        private static Texture2D popupWindowContentTexNew;

        static RGUIStyle()
        {
            CreateStyles();
        }

        public static void CreateStyles()
        {
            CreateFlatButton();
            CreateFlatButtonLeft();
            CreatePopupFlatButton();
            CreatePopupFlatButtonSelected();
            CreatePopup();
            CreatePopupTitle();
            CreateDarkWindow();
            CreateAlignLeftBox();
            CreateWarningLabel();
            CreateWarningLabelNoStyle();
            CreateAnimationStep();
            CreateDimText();
            CreateInfoText();

            CreatePopupWindowNew();
            CreatePopupWindowTitleNew();
            CreatePopupWindowContentNew();
        }

        static void CreateFlatButton()
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                wordWrap = false, 
                alignment = TextAnchor.MiddleCenter
            };

            var toggle = GUI.skin.toggle;
            style.normal.textColor = toggle.normal.textColor;
            style.hover.textColor = toggle.hover.textColor;

            flatButtonTex = new Texture2D(1, 1);
            flatButtonTex.SetPixels(new[] { new Color(0.5f, 0.5f, 0.5f, 0.5f) });
            flatButtonTex.Apply();
            style.hover.background = flatButtonTex;

            style.name = nameof(flatButton);
            flatButton = style;
        }

        static void CreateDimText() {
            var style = new GUIStyle(GUI.skin.label);

            style.normal.textColor = new Color(0.3f, 0.3f, 0.3f);
            style.hover.textColor = new Color(0.3f, 0.3f, 0.3f);

            style.name = nameof(dimText);
            dimText = style;
        }

        static void CreateInfoText() {
            var style = new GUIStyle(GUI.skin.label) {
                richText = true,
                name = nameof(infoText),
            };

            style.normal.textColor = new Color(0.9f, 0.9f, 0.9f);
            style.hover.textColor = new Color(0.9f, 0.9f, 0.9f);

            style.name = nameof(infoText);
            infoText = style;
        }

        static void CreateFlatButtonLeft() {
            var style = new GUIStyle(GUI.skin.label) {
                wordWrap = false,
                alignment = TextAnchor.MiddleLeft
            };

            var toggle = GUI.skin.toggle;
            style.normal.textColor = toggle.normal.textColor;
            style.hover.textColor = toggle.hover.textColor;

            flatButtonLeftTex = new Texture2D(1, 1);
            flatButtonLeftTex.SetPixels(new[] { new Color(0.5f, 0.5f, 0.5f, 0.5f) });
            flatButtonLeftTex.Apply();
            style.hover.background = flatButtonLeftTex;

            style.name = nameof(flatButtonLeft);
            flatButtonLeft = style;
        }

        static void CreatePopupFlatButton()
        {
            var style = new GUIStyle(flatButton)
            {
                alignment = GUI.skin.label.alignment,
                padding = new RectOffset(24, 48, 2, 2),
                name = nameof(popupFlatButton)
            };

            popupFlatButton = style;
        }

        static void CreatePopupFlatButtonSelected() {
            var style = new GUIStyle(flatButton) {
                alignment = GUI.skin.label.alignment,
                padding = new RectOffset(24, 48, 2, 2),
                fontStyle = FontStyle.Bold,
                name = nameof(popupFlatButtonSelected),
            };
            style.normal.textColor = Color.yellow;

            popupFlatButtonSelected = style;
        }

        static void CreateAnimationStep() {
            var style = new GUIStyle(GUI.skin.box) {
                //margin = new RectOffset(0, 0, -10, 0),
                border = new RectOffset(0, 0, -15, 0),
                //padding = new RectOffset(0, 0, -10, 0),
            };

            animationStepTex = new Texture2D(1, 1);
            animationStepTex.SetPixels(new[] { new Color(0.2f, 0.2f, 0.25f, 0.9f)});
            animationStepTex.Apply();

            style.normal.background = style.hover.background = animationStepTex;

            style.name = nameof(animationStep);
            animationStep = style;
        }

        static void CreatePopup()
        {
            var style = new GUIStyle(GUI.skin.box)
            {
                border = new RectOffset()
            };

            popupTex = new Texture2D(1, 1);
            var brightness = 0.207f;
            popupTex.SetPixel(0, 0, new Color(brightness, brightness, brightness));
            popupTex.Apply();

            style.normal.background = style.hover.background = popupTex;

            style.name = nameof(popup);
            popup = style;
        }

        static void CreatePopupTitle() {
            var style = new GUIStyle(GUI.skin.label);

            //style.fontSize = 18;
            style.alignment = TextAnchor.MiddleCenter;
            //style.fontStyle = FontStyle.Bold;
            //style.normal.textColor = Color.white;
            //style.normal.background = Texture2D.blackTexture;

            popupTitleTex = new Texture2D(1, 1);
            var bgB = 0.238f;
            popupTitleTex.SetPixel(0, 0, new Color(bgB, bgB, bgB));
            popupTitleTex.Apply();

            var textB = 0.129f;
            style.normal.textColor = new Color(textB, textB, textB);
            style.normal.background = popupTitleTex;

            popupTitle = style;
        }

        static void CreatePopupWindowNew() {
            var style = new GUIStyle(GUI.skin.window);

            popupTexNew = new Texture2D(1, 1);
            var colorValue = 0.63f;
            popupTexNew.SetPixel(0, 0, new Color(colorValue, colorValue, colorValue));
            popupTexNew.Apply();

            style.normal.background = popupTexNew;
            style.onNormal.background = popupTexNew;

            style.name = nameof(popupWindowNew);
            popupWindowNew = style;
        }

        static void CreatePopupWindowTitleNew() {
            var style = new GUIStyle(GUI.skin.label);
            //style.alignment = TextAnchor.MiddleCenter;

            //popupTitleTexNew = new Texture2D(1, 1);
            //var bgColorVal = 0.63f;
            //popupTitleTexNew.SetPixel(0, 0, new Color(bgColorVal, bgColorVal, bgColorVal));
            //popupTitleTexNew.Apply();

            var textColorVal = 0.32f;
            style.normal.textColor = new Color(textColorVal, textColorVal, textColorVal);
            //style.normal.background = popupTitleTexNew;

            style.name = nameof(popupWindowTitleNew);
            popupWindowTitleNew = style;
        }

        static void CreatePopupWindowContentNew() {
            //var style = new GUIStyle(GUI.skin.box);

            var style = new GUIStyle(GUI.skin.box);
            LinkedMovement.LinkedMovement.Log("CreatePopupWindowContentNew");
            LinkedMovement.LinkedMovement.Log("padding: " + style.padding.ToString());
            LinkedMovement.LinkedMovement.Log("maring: " + style.margin.ToString());
            LinkedMovement.LinkedMovement.Log("border: " + style.border.ToString());
            //style.padding = new RectOffset();
            style.padding = new RectOffset(0, 0, 10, 0);
            style.margin = new RectOffset();
            //style.margin = new RectOffset(0, 0, 10, 0);
            //style.border = new RectOffset();
            style.border = new RectOffset(-10, -10, 0, -10);
            LinkedMovement.LinkedMovement.Log("padding: " + style.padding.ToString());
            LinkedMovement.LinkedMovement.Log("maring: " + style.margin.ToString());
            LinkedMovement.LinkedMovement.Log("border: " + style.border.ToString());

            var bgValue = 0.54f;
            //var bgValue = 0.11f;
            popupWindowContentTexNew = new Texture2D(1, 1);
            popupWindowContentTexNew.SetPixel(0, 0, new Color(bgValue, bgValue, bgValue));
            //popupWindowContentTexNew.SetPixel(0, 0, new Color(0f, 0f, 0.5f));
            popupWindowContentTexNew.Apply();

            style.normal.background = style.hover.background = popupWindowContentTexNew;

            style.name = nameof(popupWindowContentTexNew);
            popupWindowContentNew = style;
        }

        static void CreateDarkWindow()
        {
            var style = new GUIStyle(GUI.skin.window);

            style.normal.background = darkWindowTexNormal = CreateTexDark(style.normal.background, 0.6f, 2f);
            style.onNormal.background = darkWindowTexOnNormal = CreateTexDark(style.onNormal.background, 0.5f, 1.9f);

            style.name = nameof(darkWindow);

            darkWindow = style;
        }

        static void CreateAlignLeftBox()
        {
            var style = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleLeft,
                name = nameof(alignLeftBox)
            };

            alignLeftBox = style;
        }

        static Texture2D CreateTexDark(Texture2D src, float colorRate, float alphaRate)
        {
            LinkedMovement.LinkedMovement.Log($"RGUIStyle.CreateTexDark w: {src.width}, h: {src.height}");
            // copy texture trick.
            // Graphics.CopyTexture(src, dst) must same format src and dst.
            // but src format can't call GetPixels().
            var tmp = RenderTexture.GetTemporary(src.width, src.height);
            Graphics.Blit(src, tmp);

            var prev = RenderTexture.active;
            RenderTexture.active = prev;

            var dst = new Texture2D(src.width, src.height, TextureFormat.RGBA32, false);
            dst.ReadPixels(new Rect(0f, 0f, src.width, src.height), 0, 0);
            

            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(tmp);


            var pixels = dst.GetPixels();
            for (var i = 0; i < pixels.Length; ++i)
            {
                var col = pixels[i];
                col.r *= colorRate;
                col.g *= colorRate;
                col.b *= colorRate;
                col.a *= alphaRate;

                pixels[i] = col;
            }

            dst.SetPixels(pixels);
            dst.Apply();

            return dst;
        }


        static void CreateWarningLabel()
        {
            var style = new GUIStyle(GUI.skin.box)
            {
                alignment = GUI.skin.label.alignment,
                richText = true, 
                name = nameof(warningLabel)
            };

            warningLabel = style;
        }

        static void CreateWarningLabelNoStyle()
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                richText = true,
                name = nameof(warningLabelNoStyle)
            };

            warningLabelNoStyle = style;
        }
    }
}
