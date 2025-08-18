// ATTRIB: TransformAnarchy
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LinkedMovement.Utils {
    static class LMUtils {
        static private HighlightOverlayController.HighlightHandle CurrentHighlightHandle;
        static private BuildableObject CurrentHighlightedObject;
        static private IEnumerator CurrentHightlightCoroutine;

        static public void AttachTargetToBase(Transform baseObject, Transform targetObject) {
            LinkedMovement.Log("LMUtils.AttachTargetToBase, parent: " + baseObject.name + ", target: " + targetObject.name);
            var baseTransform = baseObject;
            targetObject.SetParent(baseTransform);
        }

        static public void UpdateMouseColliders(BuildableObject bo) {
            if (bo.mouseColliders != null) {
                foreach (MouseCollider mouseCollider in bo.mouseColliders)
                    mouseCollider.updatePosition();
            }
        }

        static public string GetGameObjectBuildableName(GameObject go) {
            var buildableObject = GetBuildableObjectFromGameObject(go);
            if (buildableObject != null)
                return buildableObject.getName();
            return go.name;
        }

        static public BuildableObject GetBuildableObjectFromGameObject(GameObject go) {
            var buildableObject = go.GetComponent<BuildableObject>();
            return buildableObject;
        }

        static public PairBase GetPairBaseFromSerializedMonoBehaviour(SerializedMonoBehaviour smb) {
            PairBase pairBase;
            smb.tryGetCustomData(out pairBase);
            return pairBase;
        }

        static public PairTarget GetPairTargetFromSerializedMonoBehaviour(SerializedMonoBehaviour smb) {
            PairTarget pairTarget;
            smb.tryGetCustomData(out pairTarget);
            return pairTarget;
        }

        static public void HighlightBuildableObject(BuildableObject bo) {
            if (CurrentHighlightedObject != null) {
                CurrentHighlightedObject.OnKilled -= new SerializedMonoBehaviour.OnKilledHandler(OnHighlightedObjectKilled);
                if (CurrentHightlightCoroutine != null) {
                    CurrentHighlightedObject.StopCoroutine(CurrentHightlightCoroutine);
                }
            }
            CurrentHighlightHandle?.remove();

            CurrentHighlightHandle = HighlightOverlayController.Instance.add(bo.getRenderersToHighlight());
            CurrentHighlightedObject = bo;
            CurrentHighlightedObject.OnKilled += new SerializedMonoBehaviour.OnKilledHandler(OnHighlightedObjectKilled);

            CurrentHightlightCoroutine = ClearHighlightOnBuildableObject(bo);
            bo.StartCoroutine(CurrentHightlightCoroutine);
        }

        static public Sequence BuildAnimationSequence(Transform transform, LMAnimationParams animationParams, bool isEditing = false) {
            LinkedMovement.Log("LMUtils.BuildAnimationSequence");
            // Parse easings
            Ease toEase;
            Ease fromEase;

            if (Enum.TryParse(animationParams.toEase, out toEase)) {
                LinkedMovement.Log($"Sucessfully parsed toEase {animationParams.toEase}");
            } else {
                LinkedMovement.Log($"Failed to parse toEase {animationParams.toEase}");
                toEase = Ease.InOutQuad;
            }

            if (Enum.TryParse(animationParams.fromEase, out fromEase)) {
                LinkedMovement.Log($"Sucessfully parsed fromEase {animationParams.fromEase}");
            } else {
                LinkedMovement.Log($"Failed to parse fromEase {animationParams.fromEase}");
                fromEase = Ease.InOutQuad;
            }

            Sequence sequence = DOTween.Sequence();

            var toPositionTween = DOTween.To(() => transform.position, value => transform.position = value, animationParams.startingPosition + animationParams.targetPosition, animationParams.toDuration).SetEase(toEase);
            var toRotationTween = DOTween.To(() => transform.rotation, value => transform.rotation = value, animationParams.targetRotation, animationParams.toDuration).SetOptions(false).SetEase(toEase);

            var fromPositionTween = DOTween.To(() => transform.position, value => transform.position = value, animationParams.startingPosition, animationParams.fromDuration).SetEase(fromEase);
            var fromRotationTween = DOTween.To(() => transform.rotation, value => transform.rotation = value, -animationParams.targetRotation, animationParams.fromDuration).SetOptions(false).SetRelative(true).SetEase(fromEase);

            sequence.Append(toPositionTween);
            sequence.Join(toRotationTween);

            sequence.AppendInterval(animationParams.fromDelay);

            sequence.Append(fromPositionTween);
            sequence.Join(fromRotationTween);

            var restartDelay = animationParams.isTriggerable ? 0 : animationParams.restartDelay;
            sequence.AppendInterval(restartDelay);

            // TODO: Ability to set loops for triggered?
            if (isEditing || !animationParams.isTriggerable) {
                sequence.SetLoops(-1);
            } else {
                sequence.SetLoops(0);
                sequence.Pause();
            }

            return sequence;
        }

        private static void OnHighlightedObjectKilled(SerializedMonoBehaviour smb) {
            CurrentHighlightedObject.OnKilled -= new SerializedMonoBehaviour.OnKilledHandler(OnHighlightedObjectKilled);
            CurrentHighlightedObject = null;
            CurrentHighlightHandle?.remove();
            CurrentHighlightHandle = null;
        }

        private static IEnumerator ClearHighlightOnBuildableObject(BuildableObject bo) {
            yield return new WaitForSecondsRealtime(2f);
            if (bo == CurrentHighlightedObject) {
                OnHighlightedObjectKilled(bo);
                CurrentHightlightCoroutine = null;
            }
        }
    }
}
