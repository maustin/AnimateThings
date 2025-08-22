using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LinkedMovement.Utils {
    public static class DOTweenShims {
        public static Tweener DOLocalMove(this Transform t, Vector3 endValue, float duration) {
            return DOTween
                .To(() => t.localPosition,
                v => t.localPosition = v,
                endValue,
                duration)
            .SetTarget(t);
        }

        public static Tweener DOLocalRotate(this Transform t, Vector3 endValue, float duration, RotateMode mode = RotateMode.Fast) {
            switch (mode) {
                case RotateMode.Fast:
                default: {
                        // Absolute orientation: Slerp from current to target Quaternion.Euler(endValue)
                        Quaternion startQ = t.localRotation;
                        Quaternion endQ = Quaternion.Euler(endValue);
                        float p = 0f;
                        return DOTween
                            .To(() => p, v => {
                                p = v;
                                t.localRotation = Quaternion.Slerp(startQ, endQ, p);
                            }, 1f, duration)
                            .SetTarget(t);
                    }

                case RotateMode.LocalAxisAdd:
                case RotateMode.FastBeyond360: {
                        // Additive spin: apply a fraction of the desired delta each update
                        Quaternion startQ = t.localRotation;
                        float p = 0f;
                        return DOTween
                            .To(() => p, v => {
                                p = v;
                                // Progressively apply the Euler delta on top of the original start
                                t.localRotation = startQ * Quaternion.Euler(endValue * p);
                            }, 1f, duration)
                            .SetTarget(t);
                    }
            }
        }
    }
}
