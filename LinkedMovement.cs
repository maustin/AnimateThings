using HarmonyLib;
using UnityEngine;

namespace LinkedMovement {
    public class LinkedMovement : AbstractMod {
        public const string VERSION_NUMBER = "1.0";
        public override string getIdentifier() => "artex.linkedMovement";
        public override string getName() => "Linked Movement";
        public override string getDescription() => "move things";
        public override string getVersionNumber() {
            return VERSION_NUMBER;
        }

        //public override bool isMultiplayerModeCompatible() => true;
        //public override bool isRequiredByAllPlayersInMultiplayerMode() => false;

        public static LinkedMovement Instance;
        public static Harmony Harmony;
        private static LinkedMovementController Controller;

        public static LinkedMovementController GetController() {
            if (Controller == null) {
                Log("Create Controller!");
                GameObject go = new GameObject();
                go.name = "LinkedMovementController";
                Controller = go.AddComponent<LinkedMovementController>();
            }
            return Controller;
        }

        public static void ClearController() {
            Log("ClearController");
            GameObject.Destroy(Controller);
            Controller = null;
        }

        private KeybindManager _keybindManager;

        public static void Log(string msg) {
            Debug.Log("LinkedMovement: " + msg);
        }

        public LinkedMovement() {
            registerHotkeys();
            Log("Done register hotkeys");
        }

        public override void onEnabled() {
            Log("Starting v" + VERSION_NUMBER);
            Instance = this;

            Harmony = new Harmony(getIdentifier());

            Log("Patching...");
            Harmony.PatchAll();
            Log("Startup complete");
        }

        public override void onDisabled() {
            Log("onDisabled");
            unregisterHotkeys();
            destroyController();

            if (Harmony == null)
                return;
            Harmony.UnpatchAll(getIdentifier());
            Harmony = null;
        }

        public void destroyController() {
            Log("destroyController");
            if (Controller != null) {
                GameObject.Destroy(Controller.gameObject);
                Controller = null;
            }
        }

        private void registerHotkeys() {
            Log("register hotkeys");
            _keybindManager = new KeybindManager(getIdentifier(), getName());
            _keybindManager.AddKeybind("LM_toggleGUI", "Toggle Linker UI", "Toggles whether the Linker UI is visible", KeyCode.Keypad3);
            _keybindManager.RegisterAll();
        }

        private void unregisterHotkeys() {
            Log("unregister hotkeys");
            _keybindManager.UnregisterAll();
        }
    }
}
