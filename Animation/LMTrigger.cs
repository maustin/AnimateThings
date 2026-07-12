using LinkedMovement.Utils;
using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LinkedMovement {
    public class LMTrigger : MonoBehaviour, IEffect {
        private SerializedMonoBehaviour effectBehaviour;
        private List<EffectBoxHandle> effectBoxHandles;
        // This trigger shadows the object's built-in effect (lights, doors). Linking is delegated to it so its automatic
        // behavior (night/day cycle, peep doors) is suspended while an Effects Controller owns the object, without it ever executing.
        private Dictionary<EffectBoxHandle, EffectBoxHandle> decoEffectBoxHandles;
        private Sequence sequence;
        private EffectEntry effectEntry;

        public LMAnimationParams animationParams;

        public LMTrigger() {
            LMLogger.Debug("LMTrigger constructor");
        }

        private void Awake() {
            LMLogger.Debug("LMTrigger.Awake");
            effectBehaviour = GetComponent<SerializedMonoBehaviour>();
        }

        private void OnDestroy() {
            LMLogger.Debug("LMTrigger.OnDestroy");
            if (GameController.Instance != null && GameController.Instance.isQuittingGame) return;

            // Release without applying values or callbacks. Completing the sequence here can throw against objects that are
            // mid-destruction, which corrupts PrimeTween's update loop ("updateDepth != 0" every frame afterwards).
            var wasPlaying = sequence.isAlive;
            if (wasPlaying) sequence.emergencyStop();

            // On park/scene teardown there is no gameplay state left to unwind
            if (!gameObject.scene.isLoaded) return;

            if (wasPlaying && animationParams != null) {
                LMUtils.ResetTransformLocals(transform, animationParams.startingLocalPosition, animationParams.startingLocalRotation, animationParams.startingLocalScale);
            }

            // Restore the shadowed built-in effect's automatic behavior if this trigger is removed while still linked
            if (decoEffectBoxHandles != null) {
                var effectDecoObject = GetComponent<EffectDecoObject>();
                if (effectDecoObject != null) {
                    foreach (var decoEffectBoxHandle in decoEffectBoxHandles.Values) {
                        effectDecoObject.unlinkEffectBox(decoEffectBoxHandle);
                    }
                }
                decoEffectBoxHandles = null;
            }
        }

        public void update(LMAnimationParams animationParams) {
            LMLogger.Debug("LMTrigger.update");
            this.animationParams = animationParams;
            if (effectEntry != null) {
                initializeOnFirstAssignment(effectEntry);
            }
        }

        public SerializedMonoBehaviour getEffectBehaviour() => this.effectBehaviour;

        public EffectRunner.ExecutionHandle execute(EffectEntry effectEntry) {
            LMLogger.Debug($"LMTrigger.execute sequence name: {animationParams.name}");
            this.effectEntry = effectEntry;
            if (sequence.isAlive) {
                LMLogger.Debug("Sequence is already running, reset");
                sequence.SetRemainingCycles(false);
                sequence.Complete();
            }

            EffectRunner.ExecutionHandle andExecute = new EffectRunner.ExecutionHandle((MonoBehaviour)this, this.playEffect());
            //andExecute.onComplete += new EffectRunner.ExecutionHandle.OnComplete(this.onCompleteHandler);
            return andExecute;
        }

        public EffectBoxHandle linkEffectBox(EffectBox effectBox) {
            LMLogger.Debug("LMTrigger.linkEffectBox");
            if (effectBoxHandles == null)
                effectBoxHandles = new List<EffectBoxHandle>();
            EffectBoxHandle effectBoxHandle = new EffectBoxHandle(effectBox);
            effectBoxHandles.Add(effectBoxHandle);

            var effectDecoObject = GetComponent<EffectDecoObject>();
            if (effectDecoObject != null) {
                if (decoEffectBoxHandles == null)
                    decoEffectBoxHandles = new Dictionary<EffectBoxHandle, EffectBoxHandle>();
                decoEffectBoxHandles[effectBoxHandle] = effectDecoObject.linkEffectBox(effectBox);
            }

            return effectBoxHandle;
        }

        public void unlinkEffectBox(EffectBoxHandle effectBoxHandle) {
            LMLogger.Debug("LMTrigger.unlinkEffectBox");

            if (decoEffectBoxHandles != null && decoEffectBoxHandles.TryGetValue(effectBoxHandle, out var decoEffectBoxHandle)) {
                GetComponent<EffectDecoObject>()?.unlinkEffectBox(decoEffectBoxHandle);
                decoEffectBoxHandles.Remove(effectBoxHandle);
                if (decoEffectBoxHandles.Count == 0)
                    decoEffectBoxHandles = null;
            }

            if (effectBoxHandles == null)
                return;
            effectBoxHandles.Remove(effectBoxHandle);
            if (effectBoxHandles.Count != 0)
                return;
            effectBoxHandles = null;
        }

        public AbstractEditorPanel createEditorPanel(EffectEntry effectEntry, RectTransform parentRectTransform) {
            LMLogger.Debug("LMTrigger.createEditorPanel");
            this.effectEntry = effectEntry;
            AnimationTriggerEffectEditorPanel editorPanel = Instantiate<AnimationTriggerEffectEditorPanel>(ScriptableSingleton<UIAssetManager>.Instance.animationTriggerEffectEditorPanel, (Transform)parentRectTransform);
            editorPanel.setEffectEntry(effectEntry);
            return (AbstractEditorPanel)editorPanel;
        }

        public void initializeOnFirstAssignment(EffectEntry effectEntry) {
            LMLogger.Debug("LMTrigger.initializeOnFirstAssignment");
            this.effectEntry = effectEntry;
            effectEntry.setDuration(LMUtils.GetSequenceDuration(animationParams));
        }

        public string getName(EffectEntry effectEntry) => animationParams.name;

        public Sprite getSprite(EffectEntry effectEntry) {
            // TODO: Could we have a custom icon?
            LMLogger.Debug("LMTrigger.getSprite");
            this.effectEntry = effectEntry;
            return ScriptableSingleton<UIAssetManager>.Instance.effectIconGeneric;
        }

        private IEnumerator playEffect() {
            LMLogger.Debug($"LMTrigger.playEffect sequence name: {animationParams.name}");
            sequence = LMUtils.BuildAnimationSequence(gameObject.transform, animationParams);
            
            yield return null;
        }

    }
}
