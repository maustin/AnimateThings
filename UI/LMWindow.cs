using RapidGUI;
using UnityEngine;
using static Mono.Security.X509.X520;

namespace LinkedMovement.UI {
    public class LMWindow : WindowLauncher {
        private IDoGUI content;
        public WindowManager.WindowType type;
        public bool alwaysRender;

        public LMWindow(WindowManager.WindowType type, string title, IDoGUI content, bool alwaysRender, int width) : base(title, width) {
            LinkedMovement.Log("LMWindow: " + title);
            this.type = type;
            this.content = content;
            this.alwaysRender = alwaysRender;
        }

        public void Configure(Vector2 position, int fixedHeight, WindowManager windowManager) {
            if (fixedHeight > 0) {
                this.SetHeight(fixedHeight);
            }

            this.rect.position = position;
            this.Add(content.DoGUI);
            this.Open();
            this.onClose += (WindowLauncher launcher) => windowManager.removeWindow(launcher as LMWindow);
        }
    }

    //title, position, always render, data, content, alwaysRender
    //public void showExistingLinksWindow() {
    //    if (existingLinksWindow == null) {
    //        LinkedMovement.Log("WindowManager Show existing links");
    //        var width = 400f;
    //        existingLinksWindow = new WindowLauncher("Animatronitect - Existing Links", width);
    //        existingLinksWindow.SetHeight(500f);
    //        existingLinksWindow.rect.position = new Vector2(Screen.width - 400.0f - width, 175.0f);
    //        var existingPairsContent = new ExistingLinksContent();
    //        existingLinksWindow.Add(existingPairsContent.DoGUI);
    //        existingLinksWindow.Open();
    //        existingLinksWindow.onClose += (WindowLauncher launcher) => existingLinksWindow = null;
    //    }
    //}
}
