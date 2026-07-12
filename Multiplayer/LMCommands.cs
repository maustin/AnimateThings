using LinkedMovement.Animation;
using LinkedMovement.Utils;
using MiniJSON;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LinkedMovement.Multiplayer {
    // Multiplayer command layer.
    //
    // Parkitect multiplayer is deterministic lockstep with a host-ordered command stream. Any change to
    // shared mod state (animations / links) must run identically on every peer at the same ordered point,
    // so the change cannot be applied locally from a UI callback. Instead the UI sends a CustomCommand and
    // the registered handler performs the actual mutation on every peer (including the sender).
    //
    // CustomCommand is the multiplayer-safe channel: it has a stable built-in command type id, so it never
    // perturbs the reflection-ordered protobuf command table (which would mis-decode across peers).
    //
    // Objects are referenced across peers by SerializedMonoBehaviour.objectID (uint), which is allocated
    // deterministically and identically on every peer. GameObject / GUID references are not cross-peer stable.
    static class LMCommands {
        public const string ANIM_UPSERT = "LM.AnimUpsert";
        public const string ANIM_DELETE = "LM.AnimDelete";
        public const string LINK_UPSERT = "LM.LinkUpsert";
        public const string LINK_DELETE = "LM.LinkDelete";

        // The CommandController instance we last registered our handlers against. If it changes (a new game /
        // session recreates the controller and its handler table), we re-register.
        private static CommandController registeredController;

        // registerCustomCommand only adds to a dictionary and must happen identically on every peer before
        // any command flows. Safe to call repeatedly: it no-ops once registered against the current controller,
        // and tolerates a duplicate-registration exception defensively.
        public static void EnsureRegistered() {
            var controller = CommandController.Instance;
            if (controller == null) {
                LMLogger.Debug("LMCommands.EnsureRegistered: no CommandController yet, defer");
                return;
            }
            if (registeredController == controller) return;

            LMLogger.Info("LMCommands.EnsureRegistered: registering custom commands");
            register(controller, ANIM_UPSERT, handleAnimUpsert);
            register(controller, ANIM_DELETE, handleAnimDelete);
            register(controller, LINK_UPSERT, handleLinkUpsert);
            register(controller, LINK_DELETE, handleLinkDelete);
            registeredController = controller;
        }

        private static void register(CommandController controller, string identifier, CommandController.CustomCommandHandler handler) {
            try {
                controller.registerCustomCommand(identifier, handler);
            } catch (System.Exception e) {
                // Already registered against this controller (e.g. EnsureRegistered raced from Awake + setupPark).
                LMLogger.Debug($"LMCommands.register: {identifier} already registered ({e.Message})");
            }
        }

        // ===== Multiplayer-safe target boundary =====
        // Only objects whose transform is NOT part of the per-tick checksum are safe to animate / link in
        // multiplayer. Pure Deco scenery is not checksummed; rides, paths, track, trains and stations are.
        // Animation playback is frame-time interpolated, so any per-frame transform difference on a
        // checksummed object would desync. Outside multiplayer there is no such constraint.
        public static bool IsMultiplayerSafeTarget(BuildableObject bo) {
            if (bo == null) return false;
            if (CommandController.Instance == null || !CommandController.Instance.isInMultiplayerMode()) return true;
            return bo is Deco;
        }

        // ===== Send helpers (called from the UI / controller in place of direct mutation) =====

        public static void SendAnimationUpsert(BuildableObject targetBO, LMAnimationParams animationParams) {
            if (targetBO == null || animationParams == null) {
                LMLogger.Error("LMCommands.SendAnimationUpsert: missing target or params");
                return;
            }
            LMLogger.Debug($"LMCommands.SendAnimationUpsert objectID: {targetBO.objectID}");

            var payload = new Dictionary<string, object>();
            payload["objectID"] = (long)targetBO.objectID;
            payload["params"] = SerializeRawObject(animationParams);
            send(ANIM_UPSERT, payload);
        }

        public static void SendAnimationDelete(BuildableObject targetBO) {
            if (targetBO == null) {
                LMLogger.Error("LMCommands.SendAnimationDelete: missing target");
                return;
            }
            LMLogger.Debug($"LMCommands.SendAnimationDelete objectID: {targetBO.objectID}");

            var payload = new Dictionary<string, object>();
            payload["objectID"] = (long)targetBO.objectID;
            send(ANIM_DELETE, payload);
        }

        public static void SendLinkUpsert(string name, string id, BuildableObject parentBO, List<BuildableObject> targetBOs) {
            if (parentBO == null || targetBOs == null) {
                LMLogger.Error("LMCommands.SendLinkUpsert: missing parent or targets");
                return;
            }
            LMLogger.Debug($"LMCommands.SendLinkUpsert parent objectID: {parentBO.objectID}, targets: {targetBOs.Count}");

            var targetIds = new List<object>();
            foreach (var targetBO in targetBOs) {
                if (targetBO != null) targetIds.Add((long)targetBO.objectID);
            }

            var payload = new Dictionary<string, object>();
            payload["name"] = name;
            payload["id"] = id;
            payload["parentObjectID"] = (long)parentBO.objectID;
            payload["targetObjectIDs"] = targetIds;
            send(LINK_UPSERT, payload);
        }

        public static void SendLinkDelete(BuildableObject parentBO) {
            if (parentBO == null) {
                LMLogger.Error("LMCommands.SendLinkDelete: missing parent");
                return;
            }
            LMLogger.Debug($"LMCommands.SendLinkDelete parent objectID: {parentBO.objectID}");

            var payload = new Dictionary<string, object>();
            payload["parentObjectID"] = (long)parentBO.objectID;
            send(LINK_DELETE, payload);
        }

        // ===== Command handlers (run on every peer, ordered) =====

        private static void handleAnimUpsert(string identifier, byte[] data) {
            LMLogger.Debug("LMCommands.handleAnimUpsert");
            var payload = Decode(data);
            if (payload == null) return;

            var targetBO = ResolveObject(payload, "objectID");
            if (targetBO == null) {
                LMLogger.Error("handleAnimUpsert: couldn't resolve target object");
                return;
            }

            var animationParams = DeserializeRawObject<LMAnimationParams>((string)payload["params"]);
            if (animationParams == null) {
                LMLogger.Error("handleAnimUpsert: couldn't deserialize params");
                return;
            }

            LinkedMovement.GetLMController().applyAnimationUpsert(targetBO, animationParams);
        }

        private static void handleAnimDelete(string identifier, byte[] data) {
            LMLogger.Debug("LMCommands.handleAnimDelete");
            var payload = Decode(data);
            if (payload == null) return;

            var targetBO = ResolveObject(payload, "objectID");
            if (targetBO == null) {
                LMLogger.Error("handleAnimDelete: couldn't resolve target object");
                return;
            }

            LinkedMovement.GetLMController().applyAnimationDelete(targetBO);
        }

        private static void handleLinkUpsert(string identifier, byte[] data) {
            LMLogger.Debug("LMCommands.handleLinkUpsert");
            var payload = Decode(data);
            if (payload == null) return;

            var parentBO = ResolveObject(payload, "parentObjectID");
            if (parentBO == null) {
                LMLogger.Error("handleLinkUpsert: couldn't resolve parent object");
                return;
            }

            var targetBOs = new List<BuildableObject>();
            var targetIds = payload["targetObjectIDs"] as List<object>;
            if (targetIds != null) {
                foreach (var targetId in targetIds) {
                    var targetBO = ResolveObject(targetId);
                    if (targetBO != null) targetBOs.Add(targetBO);
                }
            }

            var name = payload["name"] as string;
            var id = payload["id"] as string;

            LinkedMovement.GetLMController().applyLinkUpsert(name, id, parentBO, targetBOs);
        }

        private static void handleLinkDelete(string identifier, byte[] data) {
            LMLogger.Debug("LMCommands.handleLinkDelete");
            var payload = Decode(data);
            if (payload == null) return;

            var parentBO = ResolveObject(payload, "parentObjectID");
            if (parentBO == null) {
                LMLogger.Error("handleLinkDelete: couldn't resolve parent object");
                return;
            }

            LinkedMovement.GetLMController().applyLinkDelete(parentBO);
        }

        // ===== Helpers =====

        private static void send(string identifier, Dictionary<string, object> payload) {
            EnsureRegistered();
            var json = Json.Serialize(payload);
            var bytes = Encoding.UTF8.GetBytes(json);

            if (CommandController.Instance == null) {
                // No command system available (should not happen in-game). Apply directly as a fallback.
                LMLogger.Error("LMCommands.send: no CommandController, applying directly");
                dispatchDirect(identifier, bytes);
                return;
            }

            CommandController.Instance.addCustomCommand(identifier, bytes);
        }

        private static void dispatchDirect(string identifier, byte[] data) {
            switch (identifier) {
                case ANIM_UPSERT: handleAnimUpsert(identifier, data); break;
                case ANIM_DELETE: handleAnimDelete(identifier, data); break;
                case LINK_UPSERT: handleLinkUpsert(identifier, data); break;
                case LINK_DELETE: handleLinkDelete(identifier, data); break;
            }
        }

        private static Dictionary<string, object> Decode(byte[] data) {
            if (data == null) return null;
            var json = Encoding.UTF8.GetString(data);
            var decoded = Json.Deserialize(json) as Dictionary<string, object>;
            if (decoded == null) LMLogger.Error("LMCommands.Decode: payload not a dictionary");
            return decoded;
        }

        private static BuildableObject ResolveObject(Dictionary<string, object> payload, string key) {
            object value;
            if (!payload.TryGetValue(key, out value)) return null;
            return ResolveObject(value);
        }

        private static BuildableObject ResolveObject(object idValue) {
            if (idValue == null) return null;
            uint objectID = System.Convert.ToUInt32(idValue);
            return GameController.Instance.getSerializedObject<BuildableObject>(objectID);
        }

        // Serialize a SerializedRawObject to a JSON string using Parkitect's own serializer, so it round-trips
        // identically to the custom-data the object persists. Our raw objects have canBeReferenced() == false,
        // so this produces no object-id side effects.
        private static string SerializeRawObject(SerializedRawObject obj) {
            var context = new SerializationContext(SerializationContext.Context.Multiplayer);
            var values = Serializer.serialize(context, obj);
            return Json.Serialize(values);
        }

        private static T DeserializeRawObject<T>(string json) where T : SerializedRawObject, new() {
            if (string.IsNullOrEmpty(json)) return null;
            var values = Json.Deserialize(json) as Dictionary<string, object>;
            if (values == null) return null;
            var context = new SerializationContext(SerializationContext.Context.Multiplayer);
            var obj = new T();
            Serializer.deserialize(context, obj, values);
            return obj;
        }
    }
}
