using LinkedMovement.Utils;
using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LinkedMovement {
    public class LMTrigger : MonoBehaviour, IEffect {
        private SerializedMonoBehaviour effectBehaviour;
        private List<EffectBoxHandle> effectBoxHandles;
        private Sequence sequence;

        public LMAnimationParams animationParams;

        public LMTrigger() {
            LinkedMovement.Log("LMTrigger constructor");
        }

        private void Awake() {
            LinkedMovement.Log("LMTrigger.Awake");
            effectBehaviour = GetComponent<SerializedMonoBehaviour>();
        }

        public SerializedMonoBehaviour getEffectBehaviour() => this.effectBehaviour;

        public EffectRunner.ExecutionHandle execute(EffectEntry effectEntry) {
            LinkedMovement.Log($"LMTrigger.execute sequence name: {animationParams.name}");
            if (sequence.isAlive) {
                LinkedMovement.Log("Sequence is already running, reset");
                sequence.SetRemainingCycles(false);
                sequence.Complete();
            }

            EffectRunner.ExecutionHandle andExecute = new EffectRunner.ExecutionHandle((MonoBehaviour)this, this.playEffect());
            //andExecute.onComplete += new EffectRunner.ExecutionHandle.OnComplete(this.onCompleteHandler);
            return andExecute;
        }

        public EffectBoxHandle linkEffectBox(EffectBox effectBox) {
            LinkedMovement.Log("LMTrigger.linkEffectBox");
            if (effectBoxHandles == null)
                effectBoxHandles = new List<EffectBoxHandle>();
            EffectBoxHandle effectBoxHandle = new EffectBoxHandle(effectBox);
            effectBoxHandles.Add(effectBoxHandle);
            return effectBoxHandle;
        }

        public void unlinkEffectBox(EffectBoxHandle effectBoxHandle) {
            LinkedMovement.Log("LMTrigger.unlinkEffectBox");
            if (effectBoxHandles == null)
                return;
            effectBoxHandles.Remove(effectBoxHandle);
            if (effectBoxHandles.Count != 0)
                return;
            effectBoxHandles = null;
        }

        public AbstractEditorPanel createEditorPanel(EffectEntry effectEntry, RectTransform parentRectTransform) {
            AnimationTriggerEffectEditorPanel editorPanel = Instantiate<AnimationTriggerEffectEditorPanel>(ScriptableSingleton<UIAssetManager>.Instance.animationTriggerEffectEditorPanel, (Transform)parentRectTransform);
            editorPanel.setEffectEntry(effectEntry);
            return (AbstractEditorPanel)editorPanel;
        }

        public void initializeOnFirstAssignment(EffectEntry effectEntry) {
            effectEntry.setDuration(LMUtils.GetSequenceDuration(animationParams));
        }

        public string getName(EffectEntry effectEntry) => animationParams.name;

        public Sprite getSprite(EffectEntry effectEntry) {
            // TODO: Could we have a custom icon?
            return ScriptableSingleton<UIAssetManager>.Instance.effectIconGeneric;
        }

        private IEnumerator playEffect() {
            LinkedMovement.Log($"LMTrigger.playEffect sequence name: {animationParams.name}");
            //LinkedMovement.Log("Time: " + DateTime.Now.ToString("HH:mm:ss.fff"));
            sequence = LMUtils.BuildAnimationSequence(gameObject.transform, animationParams);
            
            yield return null;
        }

    }
}
