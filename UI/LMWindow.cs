using RapidGUI;
using UnityEngine;

namespace LinkedMovement.UI {
    public class LMWindow : WindowLauncher {
        private LMWindowContent content;
        public WindowManager.WindowType type;
        public bool alwaysRender;

        public LMWindow(WindowManager.WindowType type, string title, LMWindowContent content, bool alwaysRender, int width) : base(title, width) {
            LinkedMovement.Log("LMWindow: " + title);
            this.type = type;
            this.content = content;
            this.alwaysRender = alwaysRender;
            this.content.title = title;
        }

        public void Configure(Vector2 position, int fixedHeight, WindowManager windowManager) {
            this.content.windowManager = windowManager;
            this.content.window = this;

            if (fixedHeight > 0) {
                this.SetHeight(fixedHeight);
            }

            this.rect.position = position;
            this.Add(content.RenderGUI);
            this.Open();
            this.onClose += (WindowLauncher launcher) => {
                LinkedMovement.Log("LMWindow.onClose");
                
                // TODO: Refactor so controller subscribes to onClose and handles this

                if (type == WindowManager.WindowType.CreateAnimationNew) {
                    LinkedMovement.Log("CLOSE CreateAnimationNew window");
                    LinkedMovement.GetLMController().clearEditMode();
                }
                if (type == WindowManager.WindowType.EditAnimationNew) {
                    LinkedMovement.Log("CLOSE EditAnimationNew window");
                    LinkedMovement.GetLMController().clearEditMode();
                }
                if (type == WindowManager.WindowType.CreateLinkNew) {
                    LinkedMovement.Log("CLOSE CreateLinkNew window");
                    LinkedMovement.GetLMController().clearEditMode();
                }
                if (type == WindowManager.WindowType.EditLinkNew) {
                    LinkedMovement.Log("CLOSE EditLinkNew window");
                    LinkedMovement.GetLMController().clearEditMode();
                }

                windowManager.removeWindow(launcher as LMWindow);
            };
        }
    }
}
